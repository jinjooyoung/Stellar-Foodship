using UnityEngine;

public class IngredientBox : MonoBehaviour, IInteractable
{
    [Header("설정")]
    [SerializeField] private int targetIngredientId;
    [SerializeField] private IngredientDatabaseSO database;
    [SerializeField] private GameObject ingredientBasePrefab;

    public void Interact(Player player)
    {
        if (player.heldItem == null)
        {
            TakeOutIngredient(player);
        }
        else
        {
            player.Drop();
        }
    }

    private void TakeOutIngredient(Player player)
    {
        Ingredient resultComponent = CreateIngredient();

        if (resultComponent != null)
        {
            
            player.heldItem = resultComponent;

            
            Transform itemTransform = resultComponent.transform;
            itemTransform.SetParent(player.holdPoint);
            itemTransform.localPosition = Vector3.zero;
            itemTransform.localRotation = Quaternion.identity;

            // 물리 효과 잠금
            Rigidbody rb = itemTransform.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }

            Debug.Log($"{resultComponent.ingredientData.ingredientName} 추출 성공!");
        }
        else
        {
            Debug.LogError("재료 생성 실패로 꺼낼 수 없습니다.");
        }
    }

    private Ingredient CreateIngredient()
    {
        if (database == null) return null;

        IngredientSO dataSO = database.GetIngredientById(targetIngredientId);
        if (dataSO == null) return null;

        // 베이스 프리팹 생성
        GameObject ingredientObj = Instantiate(ingredientBasePrefab);
        Ingredient ingredientComponent = ingredientObj.GetComponent<Ingredient>();

        if (ingredientComponent != null)
        {
            
            ingredientComponent.ingredientData = dataSO;
            ingredientComponent.ingredientID = dataSO.id; 
        }
        else
        {
            Debug.LogError("프리팹에 Ingredient 스크립트가 없습니다!");
            Destroy(ingredientObj);
            return null;
        }

        // 오브젝트 모델 생성 (SO의 basicModel 프리팹 사용)
        if (dataSO.basicModel != null)
        {
            GameObject visualModel = Instantiate(dataSO.basicModel, ingredientObj.transform);
            visualModel.transform.localPosition = Vector3.zero;
            visualModel.transform.localRotation = Quaternion.identity;
            ingredientComponent.currentModel = visualModel;
        }

        return ingredientComponent;
    }

    public void InteractSecondary(Player player) { }
    public Transform GetTransform() => transform;
}