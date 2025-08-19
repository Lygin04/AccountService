namespace AccountService.Common;

public class MbError(string title, int status, string detail, IReadOnlyDictionary<string, string[]>? errors = null)
{
    public string Title { get; } = title;
    public int Status { get; } = status;
    public string Detail { get; } = detail;
    public IReadOnlyDictionary<string, string[]>? Errors { get; } = errors;
}