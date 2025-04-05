using System;
using UnityEngine;

namespace _Scripts.Utils
{
    public static class Vector2IntExtensions
    {
        public static int ManhattanDistance(this Vector2Int from, Vector2Int to)
        {
            return Math.Abs(from.x - to.x) + Math.Abs(from.y - to.y);
        }
    }
}