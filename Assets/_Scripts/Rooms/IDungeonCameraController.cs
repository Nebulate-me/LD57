using UnityEngine;

namespace _Scripts.Rooms
{
    public interface IDungeonCameraController
    {
        Vector3 GetMouseWorldPosition();
        Vector3 GetMouseUIPosition();
    }
}