using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Missions;
using _Scripts.Rooms;
using Unity.Mathematics;
using UnityEngine;

namespace _Scripts.Utils
{
    public static class Vector2IntExtensions
    {
        public static object ManhattanDistance(this Vector2Int vector)
        {
            return Math.Abs(vector.x) + Math.Abs(vector.y);
        }
        
        public static int ManhattanDistance(this Vector2Int from, Vector2Int to)
        {
            return Math.Abs(from.x - to.x) + Math.Abs(from.y - to.y);
        }

        public static Vector2Int ToVector2Int(this Vector3 vector)
        {
            return new Vector2Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y));
        }
    }
}