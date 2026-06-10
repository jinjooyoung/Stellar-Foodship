using Fusion;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(Rigidbody))]
public abstract class NewPickable : NetworkBehaviour, INewInteractable
{
    public abstract int ID { get; }

    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected Collider itemCollider;

    [Networked] public NetworkBool IsHeld { get; set; }
    [Networked] public PlayerRef Holder { get; set; }

    [Networked] public NetworkBool IsPlcaed { get; set; }
    [Networked] public NewNonPickable NonP { get; set; }


    protected bool isFlying;

    protected virtual void Reset()
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
        if (!IsHeld && !IsPlcaed)
            return;

        if (IsHeld && Runner.TryGetPlayerObject(Holder, out NetworkObject playerObj))
        {
            NewPlayer player = playerObj.GetComponent<NewPlayer>();

            if (player == null)
                return;

            transform.position = player.HoldPointPos;

            transform.rotation =
                Quaternion.LookRotation(player.transform.forward, Vector3.up);
        }

        if (IsPlcaed && NonP != null)
        {
            transform.position = NonP.holdPoint.position;
            transform.rotation = Quaternion.identity;
        }
    }

    //================================================

    public virtual void Interact(NewPlayer player)
    {
        if (!Object.HasStateAuthority)
            return;

        // 이미 손에 들고 있으면 조합 시도
        if (player.HeldItem != null)
        {
            TryCombineWithHeld(player);
            return;
        }

        // 집기
        PickUp(player.Object.InputAuthority);
        player.SetHeldItem(this);
    }

    public virtual void InteractSecondary(NewPlayer player)
    {

    }

    //================================================

    public virtual bool TryPickUp(NewPlayer player)
    {
        if (!Object.HasStateAuthority)
            return false;

        Holder = player.Object.InputAuthority;
        IsHeld = true;

        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        itemCollider.enabled = false;

        return true;
    }

    public virtual void PickUp(PlayerRef holder)
    {
        if (!Object.HasStateAuthority)
            return;

        Holder = holder;
        IsHeld = true;

        IsPlcaed = false;
        NonP = null;

        rb.isKinematic = true;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        itemCollider.enabled = false;
    }

    public virtual void Drop(Vector3 impulse)
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

    public virtual void Place(NewNonPickable non)
    {
        if (!Object.HasStateAuthority)
            return;

        Debug.Log($"{Object.HasStateAuthority} Pickable : Place 호출됨!");

        Holder = default;
        IsHeld = false;
        IsPlcaed = true;
        NonP = non;

        rb.isKinematic = true;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        itemCollider.isTrigger = true;
    }

    //================================================

    protected virtual void TryCombineWithHeld(NewPlayer player)
    {
        NewPickable held =
        player.HeldItem.GetComponent<NewPickable>();

        if (held == null)
            return;

        if (held is NetworkIngredient ingredient &&
            this is NetworkCookware cookware)
        {
            if (cookware.TryAddIngredient(ingredient))
                player.SetHeldItem(null);

            return;
        }

        if (held is NetworkIngredient ingredient2 &&
            this is NetworkDish dish)
        {
            if (dish.TryAddIngredient(ingredient2))
                player.SetHeldItem(null);

            return;
        }

        if (held is NetworkCookware cookware2 &&
            this is NetworkDish dish2)
        {
            dish2.TryServe(cookware2);
            return;
        }

        if (held is NetworkDish dish3 &&
            this is NetworkCookware cookware3)
        {
            dish3.TryServe(cookware3);
            return;
        }
    }

    //================================================

    public virtual void OnThrown(
        Vector3 direction,
        float force,
        NewPlayer thrower)
    {
        if (!Object.HasStateAuthority)
            return;

        isFlying = true;

        Holder = default;
        IsHeld = false;

        rb.isKinematic = false;
        itemCollider.enabled = true;

        Vector3 velocity =
            direction.normalized * force +
            Vector3.up * (force * 0.5f);

        rb.linearVelocity = velocity;
    }

    //================================================

    /*public virtual void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
            return;

        if (!isFlying)
            return;
    }*/

    //================================================

    /*public virtual void OnCollisionEnter(Collision collision)
    {
        if (!Object.HasStateAuthority)
            return;

        if (!isFlying)
            return;

        int layer = collision.gameObject.layer;

        if (layer == 3)
        {
            NewPlayer player =
                collision.gameObject.GetComponent<NewPlayer>();

            if (player != null)
            {
                if (player.HeldItem == null)
                {
                    player.SetHeldItem(this);

                    Holder = player.Object.InputAuthority;
                    IsHeld = true;

                    rb.isKinematic = true;
                    itemCollider.enabled = false;

                    isFlying = false;
                    return;
                }
            }
        }

        if (layer == 6)
        {
            NewPickable other =
                collision.gameObject.GetComponent<NewPickable>();

            if (other != null)
            {
                HandlePickableCollision(other);
                return;
            }
        }

        if (layer == 7)
        {
            HandleNonPickableCollision(collision.gameObject);
            return;
        }

        isFlying = false;
    }*/

    //================================================

    protected virtual void HandlePickableCollision(NewPickable other)
    {

    }

    protected virtual void HandleNonPickableCollision(GameObject obj)
    {

    }

    //================================================

    public Transform GetTransform()
    {
        return transform;
    }
}