namespace AppLaunch.Services.Utils;

public class Sanitize
{
    public static string? RemoveSpaces(string? input)
    {
        return string.IsNullOrEmpty(input) ? input : input.Replace(" ", string.Empty);
    }    
    
    public static string? RemoveSpacesAndHyphens(string? input)
    {
        return string.IsNullOrEmpty(input) ? input : input.Replace(" ", string.Empty).Replace("-", string.Empty);
    }
}