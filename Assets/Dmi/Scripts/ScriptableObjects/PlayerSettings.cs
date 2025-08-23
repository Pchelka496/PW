using Dmi.Scripts.Player;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "Scriptable Objects/PlayerSettings")]
public class PlayerSettings : ScriptableObject
{
   [SerializeField] PlayerMovementController.Characteristics _movementCharacteristics;
    [SerializeField] PlayerFocusController.Characteristics _interactionCharacteristics;

    public PlayerMovementController.Characteristics MovementCharacteristics => _movementCharacteristics;

    public PlayerFocusController.Characteristics InteractionCharacteristics => _interactionCharacteristics;
}
