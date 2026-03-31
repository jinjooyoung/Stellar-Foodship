using UnityEngine;

public class Ingredient : Pickable
{
    public IngredientSO ingredientData;
    public int ingredientID;
    public bool isCut = false;
    public GameObject currentModel;

    public override int ID => ingredientID;

    public void OnCutComplete()
    {
        isCut = true;
        

        //currentModel 파괴
        if (currentModel != null)
        {
            Destroy(currentModel);
        }
        else
        {
            Debug.LogWarning($"{gameObject.name}의 currentModel이 존재하지 않습니다!");
        }

        //cutModel 생성
        if (ingredientData != null && ingredientData.cutModel != null)
        {
            GameObject cut = Instantiate(ingredientData.cutModel, transform);
            cut.transform.localPosition = Vector3.zero;
            cut.transform.localRotation = Quaternion.identity;
            currentModel = cut;
            Debug.Log($"{gameObject.name}의 cutModel 생성 완료");
        }
        else
        {
            Debug.LogWarning($"{ingredientData?.name}의 cutModel이 존재하지 않습니다!");
        }
    }
}
