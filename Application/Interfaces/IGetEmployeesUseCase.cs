using Application.Dto;

namespace Application.Interfaces
{
    public interface IGetEmployeesUseCase
    {
        Task<List<EmployeeDto>> ExecuteAsync();
    }
}
