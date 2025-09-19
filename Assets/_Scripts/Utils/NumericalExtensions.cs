using UnityEngine;

namespace _Scripts.Utils
{
    public static class NumericalExtensions
    {
        public static string ToIntString(this float number)
        {
            return Mathf.FloorToInt(number).ToString();
        }

        public static string DeclinePoints(this int number)
        {
            return number.Decline("очко", "очка", "очков");
        }
        /// <summary>
        /// Возвращает правильную форму слова для данного числа.
        /// Например: Decline(23, "очко", "очка", "очков") → "очка".
        /// </summary>
        public static string Decline(this int number, string singular, string few, string many)
        {
            int n = Mathf.Abs(number);
            int lastTwo = n % 100;
            int lastOne = n % 10;

            if (lastTwo is >= 11 and <= 14)
                return many;

            return lastOne switch
            {
                1 => singular,
                2 or 3 or 4 => few,
                _ => many
            };
        }
    }
}