using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public enum PlayerInputType
{
    Keyboard,
    Gamepad
}

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerController controller;
    private Vector2 moveInput;

    void Awake()
    {
        controller = GetComponent<PlayerController>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!IsCorrectDevice(context)) return;          // 플레이어 컴포넌트의 inputType과 다른 입력은 받지 않음
        Vector2 input = context.ReadValue<Vector2>();
        controller.Move(input);
    }

    public void OnInteractPrimary(InputAction.CallbackContext context)
    {
        if (!IsCorrectDevice(context)) return;

        if (context.performed)
        {
            controller.OnInteractPrimary();
        }
    }

    bool IsCorrectDevice(InputAction.CallbackContext context)
    {
        var device = context.control.device;

        if (controller.player.inputType == PlayerInputType.Keyboard)
        {
            return device is Keyboard;
        }
        else if (controller.player.inputType == PlayerInputType.Gamepad)
        {
            return device is Gamepad;
        }

        return false;
    }
}
