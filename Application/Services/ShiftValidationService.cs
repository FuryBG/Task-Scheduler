using Application.Contracts;
using Application.Dto;
using Application.Interfaces;
using Domain.Interfaces;
using Domain.Models;

namespace Application.Services
{
    public class ShiftValidationService : IShiftValidationService
    {
        private readonly IDbRepository<Role> _roleRepository;
        public ShiftValidationService(IDbRepository<Role> roleRepository)
        {
            _roleRepository = roleRepository;
        }
        public async Task<ValidationResult> ShiftIsValid(TimeSpan startTime, TimeSpan endTime, int EmployeeId, int selectedRoleId, List<ShiftDto> existingDayShifts)
        {
            ValidationResult result = new ValidationResult();
            IEnumerable<Role> employeeRoles = await _roleRepository.FilterAsync(x => x.Employees.Any(e => e.Id == EmployeeId && e.Roles.Any(r => r.Id == selectedRoleId)));

            if (employeeRoles.Count() == 0)
            {
                result.IsValid = false;
                result.Errors.Add("Invalid Role.");
            }

            if (startTime >= endTime)
            {
                result.IsValid = false;
                result.Errors.Add("Invalid Start and End time.");
            }

            foreach (var shift in existingDayShifts)
            {
                if (startTime < shift.EndTime && endTime > shift.StartTime)
                {
                    result.IsValid = false;
                    result.Errors.Add("Overlap schedule.");
                    break;
                }
            }

            return result;
        }
    }
}
