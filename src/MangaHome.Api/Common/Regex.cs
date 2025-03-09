namespace MangaHome.Api.Common;

public static class Regex
{
    public const string Alphabet = @"^[a-zA-Z]*$";
    public const string AlphabetAndNumber = @"^[a-zA-Z0-9]*$";
    public const string Password = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,32}$";
}