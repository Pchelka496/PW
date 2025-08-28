using UnityEngine;

public class MouseLocker : MonoBehaviour
{
    void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;                   
    }

    void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;   
        Cursor.visible = true;                    
    }
}
