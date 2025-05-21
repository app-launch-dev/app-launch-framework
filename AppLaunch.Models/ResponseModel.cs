namespace AppLaunch.Models;

public class CoreResponse<T>
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public T Data { get; set; }
}
public class CoreResponse
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
}