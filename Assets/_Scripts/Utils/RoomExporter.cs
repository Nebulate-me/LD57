using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using _Scripts.Rooms;
using UnityEngine;

namespace _Scripts.Utils
{
    public class RoomExporter : MonoBehaviour
    {
        [SerializeField] private List<Room> rooms;

        private void Start()
        {
            ExportRoomsToFile(rooms);
        }

        private static void ExportRoomsToFile(List<Room> rooms)
        {
            var sb = new StringBuilder();

            foreach (var room in rooms)
            {
                sb.AppendLine($"Комната: {room.RoomName}");

                if (room.RoomTypes.Count > 0)
                {
                    var translatedTypes = room.RoomTypes
                        .Select(rt => rt.Translate())
                        .Where(t => !string.IsNullOrWhiteSpace(t));

                    sb.AppendLine($"Типы: {string.Join(", ", translatedTypes)}");
                }
                else
                {
                    sb.AppendLine("Типы: (не указаны)");
                }

                sb.AppendLine(); // blank line between rooms
            }

            string filePath = Path.Combine(Application.persistentDataPath, "rooms.txt");
            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
            Debug.Log($"Rooms exported to {filePath}");
        }
    }
}