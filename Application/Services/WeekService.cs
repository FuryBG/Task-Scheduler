using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class WeekService : IWeekService
    {
        public DateTime GetStartOfWeek()
        {
            DateTime today = DateTime.Today;
            int diff = CalculateDiff();
            return today.AddDays(-diff);
        }

        public DateTime GetEndOfWeek()
        {
            DateTime startOfWeek = GetStartOfWeek();
            return startOfWeek.AddDays(6);
        }

        public List<DateTime> GetWeekDays()
        {
            DateTime startOfWeek = GetStartOfWeek();
            var weekDays = new List<DateTime>();

            for (int i = 0; i < 7; i++)
            {
                weekDays.Add(startOfWeek.AddDays(i));
            }
            return weekDays;
        }

        private int CalculateDiff()
        {
            int diff = (int)DateTime.Today.DayOfWeek - (int)DayOfWeek.Monday;

            if (diff < 0)
            {
                diff += 7;
            }
            return diff;
        }
    }
}
