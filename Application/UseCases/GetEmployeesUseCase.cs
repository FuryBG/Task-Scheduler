using Application.Dto;
using Application.Interfaces;
using Application.Services;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases
{
    public class GetEmployeesUseCase : IGetEmployeesUseCase
    {
        private readonly IDbRepository<Employee> _employeeRepository;
        private readonly IWeekService _weekService;

        public GetEmployeesUseCase(IDbRepository<Employee> employeeRepository, IWeekService weekService)
        {
            _employeeRepository = employeeRepository;
            _weekService = weekService;
        }

        public async Task<List<EmployeeDto>> ExecuteAsync()
        {
            DateTime startOfWeek = _weekService.GetStartOfWeek();
            DateTime endOfWeek = _weekService.GetEndOfWeek();

            var employees = await _employeeRepository.QueryAsync(e =>
                    e.Include(emp => emp.Roles)
                     .Include(emp => emp.Shifts)
                     .ThenInclude(s => s.Role)
                     .Select(emp => new EmployeeDto
                     {
                         Id = emp.Id,
                         Name = emp.Name,
                         Roles = emp.Roles,
                         Shifts = emp.Shifts.Where(s => s.Date >= startOfWeek && s.Date <= endOfWeek)
                         .Select(s => new ShiftDto
                         {
                             Id = s.Id,
                             EmployeeId = s.EmployeeId,
                             RoleName = s.Role.Name,
                             RoleId = s.RoleId,
                             StartTime = s.StartTime,
                             EndTime = s.EndTime,
                             Date = s.Date
                         }).ToList()
                     }));

            return employees.ToList();
        }
    }
}
