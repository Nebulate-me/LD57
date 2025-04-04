using System.Collections.Generic;
using Utilities.Monads;
using Utilities.RandomService;

namespace _Scripts.Utils
{
    public static class RandomServiceExtensions
    {
        // TODO: Move into Utilities SDK
        public static IMaybe<T> SampleOrEmpty<T>(this IRandomService randomService, IList<T> elements)
        {
            return randomService.SampleOrDefault(elements).ToMaybe();
        }
    }
}