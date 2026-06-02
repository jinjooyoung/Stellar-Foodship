using Fusion;
using UnityEngine;

public class PlayerLobbyData : NetworkBehaviour
{
    [Networked] public int SlotIndex { get; set; }
    [Networked] public int CharacterIndex { get; set; }
    [Networked] public NetworkBool IsReady { get; set; }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            CharacterIndex = 0;
            IsReady = false;
        }

        Debug.Log($"LobbyData Spawned / InputAuthority {Object.InputAuthority}");
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SelectCharacter(int index)
    {
        CharacterIndex = Mathf.Max(0, index);
        IsReady = false;

        Debug.Log($"{Object.InputAuthority} 캐릭터 선택 : {CharacterIndex}");
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetReady(NetworkBool ready)
    {
        IsReady = ready;
        Debug.Log($"{Object.InputAuthority} Ready : {IsReady}");
    }
}
