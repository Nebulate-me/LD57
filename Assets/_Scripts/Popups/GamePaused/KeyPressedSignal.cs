using UnityEngine;

namespace _Scripts.Popups.GamePaused
{
    public struct KeyPressedSignal
    {
        public KeyCode KeyCode { get; }

        public KeyPressedSignal(KeyCode keyCode)
        {
            KeyCode = keyCode;
        }
    }
}