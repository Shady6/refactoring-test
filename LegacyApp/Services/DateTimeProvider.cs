using System;

namespace LegacyApp.Services
{
    public interface IDateTimeProvider
    {
        public DateTime GetCurrentDate();
    }

    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime GetCurrentDate() => DateTime.Now;
    }
}