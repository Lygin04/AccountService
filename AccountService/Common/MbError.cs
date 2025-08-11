namespace AccountService.Common;

public class MbError
{
    public string Title { get; }
    public int Status { get; }
    public string Detail { get; }
    public IReadOnlyDictionary<string, string[]>? Errors { get; }

    public MbError(string title, int status, string detail, IReadOnlyDictionary<string, string[]>? errors = null)
    {
        Title = title;
        Status = status;
        Detail = detail;
        Errors = errors;
    }
}