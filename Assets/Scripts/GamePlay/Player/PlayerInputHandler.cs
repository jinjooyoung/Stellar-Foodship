using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using UnityEngine.XR;

public enum PlayerInputType
{
    Keyboard,
    Gamepad
}

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerController controller;
    private PlayerInput playerInput;
    private Vector2 moveInput;

    void Awake()
    {
        controller = GetComponent<PlayerController>();
        playerInput = GetComponent<PlayerInput>();
    }

    void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    void OnDeviceChange(UnityEngine.InputSystem.InputDevice device, InputDeviceChange change)
    {
        Debug.Log($"디바이스 변화: {device} / {change}");

        // 게임패드 다시 연결됨
        if (change == InputDeviceChange.Reconnected && device is Gamepad)
        {
            TryReassignDevice(device);
        }

        // 연결 끊김 (필요하면 UI 처리)
        if (change == InputDeviceChange.Disconnected && device is Gamepad)
        {
            Debug.LogWarning("게임패드 연결 끊김");
        }
    }

    void TryReassignDevice(UnityEngine.InputSystem.InputDevice device)
    {
        if (controller.player.inputType != PlayerInputType.Gamepad)
            return;

        Debug.Log("게임패드 재할당 시도");

        // 기존 디바이스 제거 후 다시 연결
        playerInput.SwitchCurrentControlScheme(device);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!IsCorrectDevice(context)) return;          // 플레이어 컴포넌트의 inputType과 다른 입력은 받지 않음
        Vector2 input = context.ReadValue<Vector2>();
        Debug.Log($"{this.name} 인풋 호출됨");
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
        Debug.Log($"디바이스 : {device}");

        if (controller.player.inputType == PlayerInputType.Keyboard)
        {
            Debug.Log($"디바이스 상태 : {device is Keyboard}");
            return device is Keyboard;
        }
        else if (controller.player.inputType == PlayerInputType.Gamepad)
        {
            Debug.Log($"디바이스 상태 : {device is Gamepad}");
            return device is Gamepad;
        }

        return false;
    }
}
