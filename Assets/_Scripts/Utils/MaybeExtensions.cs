using Utilities.Monads;

namespace _Scripts.Utils
{
    public static class MaybeExtensions
    {
        public static bool TryGetValue<T>(this IMaybe<T> maybe, out T value)
        {
            value = maybe.ValueOrDefault();
            return maybe.IsPresent;
        }
    }
}