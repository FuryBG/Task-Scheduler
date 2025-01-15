using Application.Contracts;
using Application.Dto;
using Application.Interfaces;
using Application.Services;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases
{
    public class UpdateShiftUseCase : IUpdateShiftUseCase
    {
        private readonly IDbRepository<Shift> _shiftRepository;
        private readonly IShiftValidationService _shiftValidationService;
        public UpdateShiftUseCase(IDbRepository<Shift> shiftRepository, IShiftValidationService shiftValidationService)
        {
            _shiftRepository = shiftRepository;
            _shiftValidationService = shiftValidationService;
        }

        public async Task ExecuteAsync(ShiftDto shiftDto)
        {
            Shift savedShift = await _shiftRepository.GetByIdAsync(shiftDto.Id);

            if(savedShift == null)
            {
                throw new Exception("Shift not found");
            }

            IEnumerable<ShiftDto> existingShifts = await _shiftRepository.QueryAsync(s =>
                s.Where(shift => shift.EmployeeId == savedShift.EmployeeId && shift.Date == shiftDto.Date && shift.Id != savedShift.Id)
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

            savedShift.RoleId = shiftDto.RoleId;
            savedShift.StartTime = shiftDto.StartTime;
            savedShift.EndTime = shiftDto.EndTime;

            await _shiftRepository.UpdateAsync(savedShift);
            await _shiftRepository.SaveAsync(CancellationToken.None);
        }
    }
}
