namespace MangaHome.Api.Common;

public readonly struct Result<T, E>
{
    public readonly bool Success;
    public readonly T? Value = default;
    public readonly E? Errors = default;

    private Result(T v, E e, bool success)
    {
        Value = v;
        Errors = e;
        Success = success;
    }

    public static implicit operator Result<T, E?>(T v)
    {
        return new(v, default, true);
    }

    public static implicit operator Result<T?, E>(E e)
    {
        return new(default, e, false);
    }
}