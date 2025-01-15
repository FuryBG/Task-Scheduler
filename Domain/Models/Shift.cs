namespace Domain.Models
{
    public class Shift
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int EmployeeId { get; set; }
        public int RoleId { get; set; }
        public virtual Role Role { get; set; }
    }
}
