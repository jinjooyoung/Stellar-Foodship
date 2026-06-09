using Fusion;
using UnityEngine;

public class NetworkCookware : NewPickable
{
    private NetworkTimer timer = new NetworkTimer();

    [Networked] public float CurrentCookTime { get; set; }
    [Networked] public float MaxCookTime { get; set; }
    [Networked] public NetworkBool IsCooking { get; set; }

    public override int ID => -1;

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (!Object.HasStateAuthority)
            return;

        timer.Tick(Runner.DeltaTime);

        CurrentCookTime = timer.CurrentTime;
        MaxCookTime = timer.MaxTime;
        IsCooking = timer.IsRunning;
    }
}
