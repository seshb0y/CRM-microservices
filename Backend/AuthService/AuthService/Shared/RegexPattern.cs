namespace ControllerFirst.Shared;

public static class RegexPattern
{
    public const string Username = @"^[a-zA-Z0-9._-]{4,30}$";
    public const string Password = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$";

    
}