using ManageUsers.Application.Attributes;

namespace ManageUsers.Application.Common.Utilities;

public static class StaticDetail
{
    [Settings(100, "50", 1, 100, "حداکثر تعداد کاراکترهای نام اشخاص")]
    public static int NameMaxLength { get; private set; } = 50;


    [Settings(101, "50", 1, 100, "حداکثر تعداد کاراکترهای نام خانوادگی اشخاص")]
    public static int LastNameMaxLength { get; private set; } = 50;

    [Settings(102, "10", 1, 20, "تعداد کاراکترهای کد پستی")]
    public static int PostalCodeLength { get; private set; } = 10;
}
