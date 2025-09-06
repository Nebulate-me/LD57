using UnityEngine;

namespace _Scripts.Game
{
    public class BasicCameraController : MonoBehaviour
    {
        private void Start()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }
    }
}