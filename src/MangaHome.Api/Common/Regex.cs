namespace MangaHome.Api.Common;

public static class Regex
{
    public const string Username = @"^(?=.*[a-zA-Z])[a-zA-Z0-9\._]{4,32}$";
    public const string Password = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&#]{8,32}$";
}