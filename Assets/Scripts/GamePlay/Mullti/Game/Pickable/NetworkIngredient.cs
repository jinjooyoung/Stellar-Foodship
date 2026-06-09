using Fusion;
using UnityEngine;

public class NetworkIngredient : NewPickable
{
    [Header("Ingredient")]

    public IngredientSO ingredientData;

    [SerializeField]
    private int ingredientID;

    [Networked]
    public NetworkBool IsCut { get; set; }

    public GameObject currentModel;

    public override int ID => ingredientID;

    //------------------------------------------------

    public override void Spawned()
    {
        base.Spawned();

        UpdateVisual();
    }

    public override void Render()
    {
        base.Render();

        UpdateVisual();
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
        if (!IsCut)
            return;

        // 이미 잘린 모델이면 생성 안함
        if (currentModel != null &&
            currentModel.name.Contains("Cut"))
            return;

        if (currentModel != null)
            Destroy(currentModel);

        if (ingredientData != null &&
            ingredientData.cutModel != null)
        {
            currentModel =
                Instantiate(
                    ingredientData.cutModel,
                    transform);

            currentModel.transform.localPosition =
                Vector3.zero;

            currentModel.transform.localRotation =
                Quaternion.identity;
        }
    }
}