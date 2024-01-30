using System.Globalization;

namespace TravelPlan.Common
{
    public class ConvertorDateTime
    {
        #region ConvertToPersian
        public static string ToPersian(DateTime InputDateTime, bool hour = true)
        {
            string DatePersian = String.Empty;
            try
            {
                var pc = new PersianCalendar();
                DatePersian = $"{pc.GetYear(InputDateTime)}/{pc.GetMonth(InputDateTime)}/{pc.GetDayOfMonth(InputDateTime)}";
                if (hour)
                {
                    DatePersian += $" {pc.GetHour(InputDateTime)}:{pc.GetMinute(InputDateTime)}:{pc.GetSecond(InputDateTime)}";
                }
            }
            catch { }
            return DatePersian;
        }
        #endregion
    }
}
