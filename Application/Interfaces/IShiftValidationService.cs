using Application.Contracts;
using Application.Dto;

namespace Application.Interfaces
{
    public interface IShiftValidationService
    {
        Task<ValidationResult> ShiftIsValid(TimeSpan startTime, TimeSpan endTime, int EmployeeId, int selectedRoleId, List<ShiftDto> existingDayShifts);
    }
}
