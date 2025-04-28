namespace SharedKernel.Abstractions;

public interface IDateTimeProvider
{
    DateTime Now { get; }
    DateTime Today { get; }
    DateTime Tomorrow { get; }
    DateTime Yesterday { get; }
    DateOnly CurrentDate { get; }
    TimeOnly CurrentTime { get; }

    DateTime ConvertToUserTimeZone(DateTime dateTime, string timeZoneId);
    DateTime ConvertFromUserTimeZone(DateTime dateTime, string timeZoneId);
    bool IsBusinessDay(DateTime date);
    int CalculateBusinessDays(DateTime startDate, DateTime endDate);
    string FormatDate(DateTime date, string? format = null);
}