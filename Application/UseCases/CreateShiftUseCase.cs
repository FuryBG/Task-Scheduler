using Application.Contracts;
using Application.Dto;
using Application.Interfaces;
using Application.Services;
using Domain.Interfaces;
using Domain.Models;

namespace Application.UseCases
{
    public class CreateShiftUseCase : ICreateShiftUseCase
    {
        private readonly IDbRepository<Shift> _shiftRepository;
        private readonly IShiftValidationService _shiftValidationService;
        public CreateShiftUseCase(IDbRepository<Shift> shiftRepository, IShiftValidationService shiftValidationService)
        {
            _shiftRepository = shiftRepository;
            _shiftValidationService = shiftValidationService;
        }
        public async Task ExecuteAsync(ShiftDto shiftDto)
        {
            IEnumerable<ShiftDto> existingShifts = await _shiftRepository.QueryAsync(s =>
                s.Where(shift => shift.EmployeeId == shiftDto.EmployeeId && shift.Date == shiftDto.Date)
                .Select(s => new ShiftDto
                {
                    Id = s.Id,
                    RoleName = s.Role.Name,
                    RoleId = s.RoleId,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime
                })
            );
            ValidationResult validationResult = await _shiftValidationService.ShiftIsValid(shiftDto.StartTime, shiftDto.EndTime, shiftDto.EmployeeId, shiftDto.RoleId, existingShifts.ToList());
            if (!validationResult.IsValid)
            {
                throw new Exception(String.Join("\n", validationResult.Errors));
            }

            Shift shift = new Shift() 
            { 
                Date = shiftDto.Date,
                EmployeeId = shiftDto.EmployeeId,
                StartTime = shiftDto.StartTime,
                EndTime = shiftDto.EndTime,
                RoleId = shiftDto.RoleId
            };

            await _shiftRepository.AddAsync(shift);
            await _shiftRepository.SaveAsync(CancellationToken.None);
        }
    }
}
