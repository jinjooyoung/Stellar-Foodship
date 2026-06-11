using Fusion;
using TMPro;
using UnityEngine;

public class NetworkDishReturner : NewNonPickable
{
    [Header("Spawn")]
    [SerializeField] private NetworkObject dishPrefab;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI countText;

    [Networked]
    public int DishCount { get; set; }

    private const int MaxDishCount = 5;

    public override bool CanPlace => false;

    //================================================

    public override void Spawned()
    {
        base.Spawned();

        if (Object.HasStateAuthority)
        {
            DishCount = 0;
        }
    }

    public override void Render()
    {
        base.Render();

        if (countText != null)
        {
            countText.text =
                (MaxDishCount - DishCount).ToString();
        }
    }

    //================================================

    public override void Interact(NewPlayer player)
    {
        if (!Object.HasStateAuthority)
            return;

        if (player.HeldItem != null)
            return;

        if (DishCount >= MaxDishCount)
            return;

        SpawnDish(player);
    }

    public override void InteractSecondary(NewPlayer player)
    {
    }

    //================================================

    void SpawnDish(NewPlayer player)
    {
        NetworkObject dishObj =
            Runner.Spawn(
                dishPrefab,
                holdPoint.position,
                holdPoint.rotation);

        NetworkDish dish =
            dishObj.GetComponent<NetworkDish>();

        if (dish == null)
        {
            Runner.Despawn(dishObj);
            return;
        }

        dish.PickUp(player.Object.InputAuthority);

        player.SetHeldItem(dishObj);

        DishCount++;
    }

    //================================================

    public void ReturnDish()
    {
        if (!Object.HasStateAuthority)
            return;

        DishCount--;

        if (DishCount < 0)
            DishCount = 0;
    }
}