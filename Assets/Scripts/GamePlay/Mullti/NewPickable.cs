using Fusion;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(Rigidbody))]
public class NewPickable : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider itemCollider;
    [SerializeField] private Collider holderCollider;
    [SerializeField] private NetworkObject holderObj;

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

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;

        if (IsHeld && holderObj != null)
        {
            NewPlayer player = holderObj.GetComponent<NewPlayer>();

            if (player != null)
            {
                rb.isKinematic = true;
                transform.position = player.HoldPointPos;
                transform.rotation = Quaternion.LookRotation(player.transform.forward, Vector3.up);
            }
        }
    }

    public void PickUp(PlayerRef holder)
    {
        Debug.Log($"PickUp »£√‚µ  / holder = {holder}");

        if (!Object.HasStateAuthority)
        {
            Debug.Log("StateAuthority æ¯¿Ω");
            return;
        }

        Holder = holder;
        IsHeld = true;

        if (Runner.TryGetPlayerObject(holder, out NetworkObject playerObj))
        {
            holderObj = playerObj;
            holderCollider = holderObj.GetComponent<Collider>();

            if (holderCollider != null && itemCollider != null)
            {
                Physics.IgnoreCollision(holderCollider, itemCollider, true);
            }
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }

    public void Drop(Vector3 impulse)
    {
        Debug.Log("Drop »£√‚µ ");
        if (!Object.HasStateAuthority) return;

        if (holderCollider != null && itemCollider != null)
        {
            Physics.IgnoreCollision(holderCollider, itemCollider, false);
        }

        Holder = default;
        IsHeld = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = false;
            rb.AddForce(impulse, ForceMode.VelocityChange);
        }

        holderCollider = null;
        holderObj = null;
    }
}
