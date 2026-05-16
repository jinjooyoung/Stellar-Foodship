using Fusion;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(Rigidbody))]
public class NewPickable : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider itemCollider;

    [Networked] public NetworkBool IsHeld { get; set; }
    [Networked] public PlayerRef Holder { get; set; }

    private void Reset()
    {
        rb = GetComponent<Rigidbody>();
        itemCollider = GetComponent<Collider>();
    }

    public override void Spawned()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (itemCollider == null)
            itemCollider = GetComponent<Collider>();
    }

    public override void Render()
    {
        if (!IsHeld)
            return;

        if (Runner.TryGetPlayerObject(Holder, out NetworkObject playerObj))
        {
            NewPlayer player = playerObj.GetComponent<NewPlayer>();

            if (player == null)
                return;

            transform.position = player.HoldPointPos;

            transform.rotation =
                Quaternion.LookRotation(player.transform.forward, Vector3.up);
        }
    }

    public void PickUp(PlayerRef holder)
    {
        if (!Object.HasStateAuthority)
            return;

        Holder = holder;
        IsHeld = true;

        rb.isKinematic = true;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        itemCollider.enabled = false;
    }

    public void Drop(Vector3 impulse)
    {
        if (!Object.HasStateAuthority)
            return;

        Holder = default;
        IsHeld = false;

        rb.isKinematic = false;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        itemCollider.enabled = true;

        rb.AddForce(impulse, ForceMode.VelocityChange);
    }
}
