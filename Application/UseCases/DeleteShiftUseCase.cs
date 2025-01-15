using Application.Dto;
using Application.Interfaces;
using Domain.Interfaces;
using Domain.Models;

namespace Application.UseCases
{
    public class DeleteShiftUseCase : IDeleteShiftUseCase
    {
        private readonly IDbRepository<Shift> _shiftRepository;
        public DeleteShiftUseCase(IDbRepository<Shift> shiftRepository)
        {
            _shiftRepository = shiftRepository;
        }

        public async Task ExecuteAsync(ShiftDto shiftDto)
        {
            Shift savedShift = await _shiftRepository.GetByIdAsync(shiftDto.Id);

            if (savedShift == null)
            {
                throw new Exception("Shift not found");
            }

            savedShift.RoleId = shiftDto.RoleId;
            savedShift.StartTime = savedShift.StartTime;
            savedShift.EndTime = savedShift.EndTime;

            await _shiftRepository.HardDeleteAsync(savedShift);
            await _shiftRepository.SaveAsync(CancellationToken.None);
        }

    }
}
