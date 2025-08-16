using System.Globalization;
using SharedKernel.Abstractions.Services;

namespace Infrastructure.Services;

public class DateTimeProvider : IDateTimeProvider
{
    private static readonly TimeZoneInfo _fixedTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");
                                                 //TimeZoneInfo.Utc;

    public DateTime Now => TimeZoneInfo.ConvertTime(DateTime.UtcNow, _fixedTimeZone);
    public DateTime Today => DateTime.Today;
    public DateTime Tomorrow => DateTime.Today.AddDays(1);
    public DateTime Yesterday => DateTime.Today.AddDays(-1);
    public DateOnly CurrentDate => DateOnly.FromDateTime(DateTime.Today);
    public TimeOnly CurrentTime => TimeOnly.FromDateTime(TimeZoneInfo.ConvertTime(DateTime.UtcNow, _fixedTimeZone));

    public DateTime ConvertToUserTimeZone(DateTime dateTime, string timeZoneId)
    {
        ArgumentNullException.ThrowIfNull(timeZoneId);
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        return TimeZoneInfo.ConvertTime(dateTime, timeZone);
    }

    public DateTime ConvertFromUserTimeZone(DateTime dateTime, string timeZoneId)
    {
        ArgumentNullException.ThrowIfNull(timeZoneId);

        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        return TimeZoneInfo.ConvertTime(dateTime, timeZone, TimeZoneInfo.Local);
    }

    public bool IsBusinessDay(DateTime date)
    {
        return date.DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday;
    }

    public int CalculateBusinessDays(DateTime startDate, DateTime endDate)
    {
        if (startDate > endDate)
        {
            (startDate, endDate) = (endDate, startDate);
        }

        int businessDays = 0;
        var currentDate = startDate.Date;

        while (currentDate <= endDate.Date)
        {
            if (IsBusinessDay(currentDate))
            {
                businessDays++;
            }

            currentDate = currentDate.AddDays(1);
        }

        return businessDays;
    }

    public string FormatDate(DateTime date, string? format = null)
    {
        format ??= "dd/MM/yyyy";
        return date.ToString(format, CultureInfo.InvariantCulture);
    }
}
