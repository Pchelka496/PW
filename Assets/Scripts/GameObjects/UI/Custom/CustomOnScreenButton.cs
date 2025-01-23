using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

namespace GameObjects.UI.Custom
{
    public class CustomOnScreenButton : OnScreenControl, IPointerDownHandler, IPointerUpHandler
    {
        [InputControl(layout = "Button")] [SerializeField]
        private string m_ControlPath;

        [Header("Mouse Button Settings")] [SerializeField]
        PointerEventData.InputButton _reactToMouseButton;

        protected override string controlPathInternal
        {
            get => m_ControlPath;
            set => m_ControlPath = value;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (IsReactingTo(eventData))
            {
                SendValueToControl(1.0f);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (IsReactingTo(eventData))
            {
                SendValueToControl(0.0f);
            }
        }

        private bool IsReactingTo(PointerEventData eventData)
        {
            return eventData.button == _reactToMouseButton;
        }
    }
}