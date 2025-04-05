using System;

namespace _Scripts.Rooms
{
    public enum RoomDirection
    {
        North = 0,
        East = 1,
        South = 2,
        West = 3
    }
    
    public static class RoomDirectionExtensions {
        public static int ToRotation(this RoomDirection direction)
        {
            switch (direction)
            {
                case RoomDirection.North:
                    return 0;
                case RoomDirection.East:
                    return 90;
                case RoomDirection.South:
                    return 180;
                case RoomDirection.West:
                    return 270;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        } 
    }
}