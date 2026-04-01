using UnityEngine;
using UnityEngine.UIElements;

public class IngredientBox : NonPickable
{
    [Header("설정")]
    [SerializeField] private int targetIngredientId;
    [SerializeField] private IngredientDatabaseSO database;
    [SerializeField] private GameObject ingredientBasePrefab;

    private void Start()
    {
        if (DataManager.instance != null)
        {
            database = DataManager.instance.ingredientDatabase;
        }
    }

    public override void Interact(Player player)
    {
        Debug.Log("재료상자 상호작용1 호출됨");

        // 플레이어가 아이템 들고 있는 경우
        if (player.heldItem != null)
        {
            Debug.Log("플레이어 들고 있는 아이템 있음");

            /*// 재료상자 위에 이미 아이템 있으면 아무것도 안함 (또는 실패 처리)
            if (heldItem != null)
            {
                Debug.Log("재료상자 위에 이미 아이템 있음 → 놓기 불가");
                return;
            }*/

            // 재료상자 위에 아이템 없으면 올려두기
            Debug.Log("재료상자 위에 아이템 올려둠");
            if (TryPlaceItem(player.heldItem))
            {
                player.heldItem = null;
            }

            return;
        }

        // 플레이어가 아무것도 안 들고 있는 경우
        Debug.Log("플레이어 들고 있는 아이템 없음");

        // 재료상자 위에 아이템 있으면 집기
        if (heldItem != null)
        {
            Debug.Log("재료상자 위 아이템 집기");
            player.heldItem = TakeItem(player);
        }
        else
        {
            // 없으면 새 재료 생성해서 지급
            Debug.Log("재료 생성해서 지급");
            TakeOutIngredient(player);
        }
    }

    private void TakeOutIngredient(Player player)
    {
        Ingredient resultComponent = CreateIngredient();

        if (resultComponent != null)
        {
            if (resultComponent.TryPickUp(player))
            {
                player.heldItem = resultComponent;
                Debug.Log($"{resultComponent.ingredientData.ingredientName} 생성 및 획득");
            }
            else
            {
                Destroy(resultComponent.gameObject);
            }
        }
    }
   
    private Ingredient CreateIngredient()
    {
        if (database == null || ingredientBasePrefab == null) return null;

        IngredientSO dataSO = database.GetIngredientById(targetIngredientId);
        if (dataSO == null) return null;

        GameObject ingredientObj = Instantiate(ingredientBasePrefab);
        Ingredient ingredientComponent = ingredientObj.GetComponent<Ingredient>();

        if (ingredientComponent != null)
        {
            // 데이터 할당
            ingredientComponent.ingredientData = dataSO;
            ingredientComponent.ingredientID = dataSO.id;
           
            ingredientComponent.isCut = false;

           
            if (dataSO.basicModel != null)
            {
                GameObject visualModel = Instantiate(dataSO.basicModel, ingredientObj.transform);
                visualModel.transform.localPosition = Vector3.zero;
                visualModel.transform.localRotation = Quaternion.identity;

                
                ingredientComponent.currentModel = visualModel;
            }
            return ingredientComponent;
        }

        Destroy(ingredientObj);
        return null;
    }

    public override void InteractSecondary(Player player) { }
}