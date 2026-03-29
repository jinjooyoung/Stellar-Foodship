using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //[NonSerialized] 
    public Player player;

    void Awake()
    {
        player = GetComponent<Player>();
    }

    //========================해당 로직 호출========================

    // 이동 : WASD / Left Stick
    public void ControlMove(Vector2 input)
    {
        if (player.state == PlayerState.Uncontrollable)
        {
            Debug.LogWarning($"{this.name} 컨트롤 비활성화 상태");
            return;
        }
        else if (player.state == PlayerState.IsAiming)
        {
            player.Aiming(input);
        }
        else
        {
            player.SetMoveInput(input);
        }
    }

    // 상호작용1 : J / Button South
    public void ControlInteractPrimary()
    {
        player.InteractPrimary();
    }

    // 상호작용2 : K / Button West
    public void ControlInteractSecondary()
    {
        Debug.Log($"{this.name} 상호작용2 컨트롤 입력 시작됨");
        player.StartInteractSecondary();
    }

    public void ControlInteractSecondaryEnd()
    {
        Debug.Log($"{this.name} 상호작용2 컨트롤 입력 종료됨");
        player.EndSecondaryAction();
    }

    // 대쉬 : Space / Button East
    public void ControlDash()
    {
        Debug.Log($"{this.name} 대쉬 컨트롤 호출됨");
        player.Dash();
    }
}
