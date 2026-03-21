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
            return;

        player.SetMoveInput(input);
    }

    public void OnInteractPrimary()
    {
        player.InteractPrimary();
    }
}
