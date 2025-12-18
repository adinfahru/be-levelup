namespace LevelUp.API.Utilities;

public static class DateTimeHelper
{
    public static bool IsOverdue(DateTime targetDate)
    {
        return DateTime.UtcNow.Date > targetDate.Date;
    }
    public static DateTime AddWorkingDays(DateTime startDate, int workingDays)
    {
        var date = startDate;
        var addedDays = 0;

        while (addedDays < workingDays)
        {
            date = date.AddDays(1);

            if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                continue;

            addedDays++;
        }

        return date;
    }
}

