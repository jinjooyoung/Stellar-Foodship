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

    public void Move(Vector2 input)
    {
        if (player.state == PlayerState.Uncontrollable)
        {
            Debug.LogWarning($"{this.name} 컨트롤 비활성화 상태");
            return;
        }

        Debug.Log($"{this.name} 컨트롤 호출됨");
        player.SetMoveInput(input);
    }

    public void OnInteractPrimary()
    {
        player.InteractPrimary();
    }
}
