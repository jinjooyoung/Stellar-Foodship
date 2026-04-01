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
        // 플레이어가 아이템 들고 있는 경우
        if (player.heldItem != null)
        {
            // 상자 위에 올려두거나 바닥에 내려둠
            if (TryPlaceItem(player.heldItem))
            {
                player.heldItem = null;
            }

            return;
        }

        // 플레이어가 아무것도 안 들고 있는 경우
        if (heldItem != null)
        {
            // 재료상자 위에 아이템 있으면 집기
            player.heldItem = TakeItem(player);
        }
        else
        {
            // 없으면 새 재료 생성해서 지급
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