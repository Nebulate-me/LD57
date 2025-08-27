using System;
using _Scripts.Rooms;

namespace _Scripts.Utils
{
    public static class RoomTypeExtensions
    {
        public static string Translate(this RoomType roomType)
        {
            return roomType switch
            {
                RoomType.None => "",
                RoomType.Toilet => "Санузел",
                RoomType.LivingRoom => "Гостиная",
                RoomType.BedRoom => "Спальня",
                RoomType.Bathroom => "Ванная",
                RoomType.Kitchen => "Кухня",
                RoomType.StorageRoom => "Кладовая",
                RoomType.Hallway => "Коридор",
                RoomType.Balcony => "Балкон",
                RoomType.Shared => "Общее пространство",
                RoomType.Childrens => "Детская",
                RoomType.Window => "Окно",
                _ => throw new ArgumentOutOfRangeException(nameof(roomType), roomType, null)
            };
        }
    }
}