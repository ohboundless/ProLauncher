namespace Boundless.Backend.Utils;

public static class UniqueUtils
{
    public static async Task<T> CreateUniqueAsync<T, TGetter>(Func<T> creator, Func<T, Task<TGetter?>> getter)
    {
        var result = default(T);
        while (result == null || await getter(result) != null)
        {
            result = creator();
        }

        return result;
    }
}