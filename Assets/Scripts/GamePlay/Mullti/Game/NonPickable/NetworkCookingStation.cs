using Fusion;
using UnityEngine;

public class NetworkCookingStation : NewNonPickable
{
    [Header("Cooking")]
    public StationType stationType;

    [SerializeField] private GameObject smokeEffectPrefab;

    private GameObject spawnedSmoke;
    private bool prevHasItem;

    public float cookTime = 5f;

    public override bool CanPlace => true;

    //------------------------------------------------

    public override void Interact(NewPlayer player)
    {
        if (!Object.HasStateAuthority)
            return;

        // 손 비었음
        if (player.HeldItem == null)
        {
            if (HeldItem == null)
                return;

            NetworkCookware cookware =
                HeldItem.GetComponent<NetworkCookware>();

            if (cookware != null)
            {
                cookware.StopCooking();
            }

            TakeItem(player);

            return;
        }

        // 손에 들고 있음
        NewPickable pickable =
            player.HeldItem.GetComponent<NewPickable>();

        if (pickable == null)
            return;

        if (!TryPlaceItem(pickable))
            return;

        NetworkCookware cookwarePlaced =
            pickable.GetComponent<NetworkCookware>();

        if (cookwarePlaced == null)
            return;

        if (cookwarePlaced.GetRequiredStation() != stationType)
            return;

        if (cookwarePlaced.IsComplete)
            return;

        if (cookwarePlaced.IsBurnt)
            return;

        if (cookwarePlaced.IngredientCount == 0)
            return;

        // 이어서 조리
        if (cookwarePlaced.NetMaxTime > 0)
        {
            cookwarePlaced.ResumeCooking();
        }
        else
        {
            cookwarePlaced.StartCooking(cookTime);
        }
    }

    public override void InteractSecondary(NewPlayer player)
    {
    }

    public override void Render()
    {
        base.Render();

        bool hasItem = HeldItem != null;

        if (prevHasItem == hasItem)
            return;

        prevHasItem = hasItem;

        if (hasItem)
        {
            if (spawnedSmoke == null)
            {
                spawnedSmoke =
                    Instantiate(
                        smokeEffectPrefab,
                        holdPoint.position,
                        Quaternion.identity,
                        transform);
            }
        }
        else
        {
            if (spawnedSmoke != null)
            {
                Destroy(spawnedSmoke);
                spawnedSmoke = null;
            }
        }
    }
}