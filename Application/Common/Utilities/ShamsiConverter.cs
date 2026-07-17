namespace ManageUsers.Application.Common.Utilities;

public static class ShamsiConverter
{
    public static string ToShamsi(this DateTime date)
    {
        var pc = new System.Globalization.PersianCalendar();
        var year = pc.GetYear(date);
        var month = pc.GetMonth(date);
        var day = pc.GetDayOfMonth(date);
        return $"{year}/{month:D2}/{day:D2}";
    }

    public static string ToShamsi(this DateTime? date)
    {
        return date.HasValue ? date.Value.ToShamsi() : string.Empty;
    }
}
