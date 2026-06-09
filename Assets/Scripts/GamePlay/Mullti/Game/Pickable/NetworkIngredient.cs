using Fusion;
using UnityEngine;

public class NetworkIngredient : NewPickable
{
    [Header("Ingredient")]

    public IngredientSO ingredientData;

    [Networked]
    public int IngredientID { get; set; }

    [Networked]
    public NetworkBool IsCut { get; set; }

    public GameObject currentModel;

    private int cachedIngredientId = -1;
    private bool cachedCutState;

    public override int ID => IngredientID;

    //------------------------------------------------

    public override void Spawned()
    {
        base.Spawned();

        RefreshData();

        UpdateVisual();
    }

    public override void Render()
    {
        base.Render();

        if (cachedIngredientId != IngredientID ||
        cachedCutState != IsCut)
        {
            cachedIngredientId = IngredientID;
            cachedCutState = IsCut;

            RefreshData();
            UpdateVisual();
        }
    }

    public void Initialize(int ingredientId)
    {
        IngredientID = ingredientId;
        IsCut = false;
    }

    void RefreshData()
    {
        ingredientData =
            DataManager.instance
            .ingredientDatabase
            .GetIngredientById(IngredientID);
    }

    //------------------------------------------------

    public void OnCutComplete()
    {
        if (!Object.HasStateAuthority)
            return;

        if (IsCut)
            return;

        IsCut = true;
    }

    //------------------------------------------------

    void UpdateVisual()
    {
        if (ingredientData == null)
            return;

        if (currentModel != null)
            Destroy(currentModel);

        GameObject prefab =
            IsCut
            ? ingredientData.cutModel
            : ingredientData.basicModel;

        if (prefab == null)
            return;

        currentModel =
            Instantiate(prefab, transform);

        currentModel.transform.localPosition =
            Vector3.zero;

        currentModel.transform.localRotation =
            Quaternion.identity;
    }
}