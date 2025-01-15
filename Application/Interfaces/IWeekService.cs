namespace Application.Interfaces
{
    public interface IWeekService
    {
        DateTime GetStartOfWeek();
        DateTime GetEndOfWeek();
        List<DateTime> GetWeekDays();
    }
}
