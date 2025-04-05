using System;
using System.Collections.Generic;
using System.Linq;

namespace _Scripts.Utils
{
    public static class EnumExtensions
    {
        public static IEnumerable<T> GetAllValues<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}