using UnityEngine;

namespace GameObjects
{
    public class CursorManager
    {
        public static void ShowCursor()
        {
            Cursor.visible = true;
        }

        public static void HideCursor()
        {
            Cursor.visible = false;
        }

        public static void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        public static void UnlockCursor()
        {
            Cursor.lockState = CursorLockMode.None;
        }

        public static void ConfineCursor()
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
}