using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities;

namespace _Scripts.Rooms
{
    public enum RoomDirection
    {
        North = 0,
        West = 1,
        South = 2,
        East = 3
    }

    public static class RoomDirectionExtensions {
        
        private static int ToZDegrees(this RoomDirection direction)
        {
              return direction switch
              {
                  RoomDirection.North => 0,
                  RoomDirection.West => 90,
                  RoomDirection.South => 180,
                  RoomDirection.East => 270,
                  _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, $"Invalid Direction to convert: {direction}")
              };  
        }

        private static RoomDirection FromZDegrees(this int degrees)
        {
            var zDegrees = degrees % 360;
            if (zDegrees == 0) return RoomDirection.North;
            if (zDegrees == 90) return RoomDirection.West;
            if (zDegrees == 180) return RoomDirection.South;
            if (zDegrees == 270) return RoomDirection.East;
            throw new ArgumentOutOfRangeException(nameof(degrees), degrees, $"Invalid ZDegrees to convert: {degrees}");
        }
        public static Quaternion ToRotation(this RoomDirection direction)
        {
            var zDegrees = direction.ToZDegrees();
            return Quaternion.Euler(0, 0, zDegrees);
        }

        public static RoomDirection Rotate(this RoomDirection direction, RoomDirection rotation)
        {
            return (direction.ToZDegrees() + rotation.ToZDegrees()).FromZDegrees();
        }

        public static RoomDirection Invert(this RoomDirection direction)
        {
            return direction switch
            {
                RoomDirection.North => RoomDirection.South,
                RoomDirection.East => RoomDirection.West,
                RoomDirection.South => RoomDirection.North,
                RoomDirection.West => RoomDirection.East,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, $"Invalid Direction to invert: {direction}")
            };  
        }

        public static RoomDirection FromVector2Int(this Vector2Int vector)
        {
            if (vector == Vector2Int.up) return RoomDirection.North;
            if (vector == Vector2Int.right) return RoomDirection.East;
            if (vector == Vector2Int.left) return RoomDirection.West;
            if (vector == Vector2Int.down) return RoomDirection.South;
            throw new ArgumentOutOfRangeException(nameof(vector), vector, $"Invalid Vector to convert: {vector}");
        }

        public static List<RoomDirection> InvertList(IEnumerable<RoomDirection> directions)
        {
            return EnumExtensions.GetAllItems<RoomDirection>().Where(direction => !directions.Contains(direction))
                .ToList();
        }
    }
}