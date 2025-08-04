namespace AccountService.Common;

public class MbResult<T>
{
    public bool IsSuccess { get; }
    public T? Data { get; }
    public MbError? Error { get; }

    private MbResult(T data)
    {
        IsSuccess = true;
        Data = data;
        Error = null;
    }

    private MbResult(MbError error)
    {
        IsSuccess = false;
        Data = default;
        Error = error;
    }

    public static MbResult<T> Success(T data) => new MbResult<T>(data);
    public static MbResult<T> Failure(MbError error) => new MbResult<T>(error);
}