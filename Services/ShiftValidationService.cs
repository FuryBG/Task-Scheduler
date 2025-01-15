using Application.DataTransferObejts;

namespace Services
{
    public class ShiftValidationService
    {
        public bool ShiftIsValid(TimeSpan startTime, TimeSpan endTime, List<ShiftDto> existingDayShifts)
        {
            if (startTime >= endTime)
            {
                return false;
            }

            foreach (var shift in existingDayShifts)
            {
                if (startTime < shift.EndTime && endTime > shift.StartTime)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
