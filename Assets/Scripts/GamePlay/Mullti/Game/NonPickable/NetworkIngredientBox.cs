using Fusion;
using UnityEngine;

public class NetworkIngredientBox : NewNonPickable
{
    [Header("Ingredient")]
    [SerializeField] private int targetIngredientId;

    [SerializeField] private NetworkObject ingredientPrefab;

    [SerializeField] private IngredientDatabaseSO database;

    public override bool CanPlace => false;

    public override void Spawned()
    {
        if (DataManager.instance != null)
        {
            database = DataManager.instance.ingredientDatabase;
        }
    }

    public override void Interact(NewPlayer player)
    {
        if (!Object.HasStateAuthority)
            return;

        // 손에 뭔가 들고 있으면 무시
        if (player.HeldItem != null)
            return;

        SpawnIngredient(player);
    }

    private void SpawnIngredient(NewPlayer player)
    {
        IngredientSO data =
            database.GetIngredientById(targetIngredientId);

        if (data == null)
            return;

        NetworkObject obj =
            Runner.Spawn(
                ingredientPrefab,
                player.HoldPointPos,
                Quaternion.identity);

        NetworkIngredient ingredient =
            obj.GetComponent<NetworkIngredient>();

        if (ingredient == null)
        {
            Runner.Despawn(obj);
            return;
        }

        ingredient.Initialize(targetIngredientId);

        ingredient.PickUp(player.Object.InputAuthority);

        NewPickable newPickable =
            obj.GetComponent<NewPickable>();

        player.SetHeldItem(newPickable);
    }

    public override void InteractSecondary(NewPlayer player)
    {
    }
}