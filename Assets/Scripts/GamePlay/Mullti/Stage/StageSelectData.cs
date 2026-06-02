using Fusion;
using UnityEngine;

public class StageSelectData : NetworkBehaviour
{
    [Networked] public int SelectedPlanet { get; set; }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SetPlanet(int index)
    {
        SelectedPlanet = index;
    }
}
