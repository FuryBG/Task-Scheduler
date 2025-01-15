namespace Application.Dto
{
    public class ShiftDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
