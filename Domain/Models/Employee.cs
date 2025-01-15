namespace Domain.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual List<Role> Roles { get; set; }
        public virtual List<Shift> Shifts { get; set; } = [];
    }
}
