using UnityEngine;

namespace _Scripts.Utils
{
    public static class NumericalExtensions
    {
        public static string ToIntString(this float number)
        {
            return Mathf.FloorToInt(number).ToString();
        }
    }
}