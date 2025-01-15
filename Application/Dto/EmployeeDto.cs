using Domain.Models;

namespace Application.Dto
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Role> Roles { get; set; }
        public List<ShiftDto> Shifts { get; set; }
    }
}
