using System.Collections.Generic;
using System.IO;
using System.Text;
using _Scripts.Missions.Apartment;
using UnityEngine;

namespace _Scripts.Utils
{
    public class MissionExporter : MonoBehaviour
    {
        [SerializeField] private List<ApartmentMission> missions;

        private void Start()
        {
            ExportMissionsToFile(missions);
        }

        private static void ExportMissionsToFile(List<ApartmentMission> missions)
        {
            var sb = new StringBuilder();

            foreach (var mission in missions)
            {
                sb.AppendLine($"Миссия: {mission.MissionName}");
                sb.AppendLine($"Награда: {mission.RewardScore} очков");

                // ✅ Room requirements
                if (mission.Requirements.Count > 0)
                {
                    sb.AppendLine("Требуемые комнаты:");
                    foreach (var req in mission.Requirements)
                    {
                        // Assuming RoomRequirement has fields like:
                        //   RoomType Type
                        sb.AppendLine($"  - {req.RoomType.Translate()} ");
                    }
                }
                else
                {
                    sb.AppendLine("Требуемые комнаты: (нет)");
                }

                // ✅ Windows requirement
                if (mission.RequiredWindows > 0)
                {
                    sb.AppendLine($"Требуемые окна: {mission.RequiredWindows}");
                }

                // ✅ Mission dependencies
                if (mission.MinCompletedMissions > 0 || mission.MaxCompletedMissions > 0)
                {
                    sb.AppendLine("Ограничения по прогрессу:");
                    if (mission.MinCompletedMissions > 0)
                        sb.AppendLine($"  - Минимум завершённых миссий: {mission.MinCompletedMissions}");
                    if (mission.MaxCompletedMissions > 0)
                        sb.AppendLine($"  - Максимум завершённых миссий: {mission.MaxCompletedMissions}");
                }

                sb.AppendLine(); // blank line between missions
            }

            string filePath = Path.Combine(Application.persistentDataPath, "missions.txt");
            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
            Debug.Log($"Missions exported to {filePath}");
        }
    }
}