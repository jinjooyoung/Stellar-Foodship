using UnityEngine;

public class IngredientBox : NonPickable
{
    [Header("설정")]
    [SerializeField] private int targetIngredientId;
    [SerializeField] private IngredientDatabaseSO database;
    [SerializeField] private GameObject ingredientBasePrefab;

    [Header("아이템 배치 설정")]
    [SerializeField] private Transform itemSlot; 
    private Ingredient placedItem; 

    private void Awake()
    {
        if (DataManager.instance != null)
        {
            database = DataManager.instance.IngredientDatabase;
        }
    }

    
    public override void Interact(Player player)
    {
        
        if (player.heldItem == null)
        {
            
            if (placedItem != null)
            {
                PickUpPlacedItem(player);
            }
            
            else
            {
                TakeOutIngredient(player);
            }
        }
       
        else
        {
           
            if (placedItem != null)
            {
                player.Drop(); 
            }
           
            else
            {
                TryPlaceItem(player);
            }
        }
    }

    
    private void PickUpPlacedItem(Player player)
    {
        if (placedItem.TryPickUp(player))
        {
            player.heldItem = placedItem;
            placedItem = null; // 선반 비우기
            Debug.Log("선반 위의 아이템을 집었습니다.");
        }
    }

    private void TryPlaceItem(Player player)
    {
        // 선반에 놓을 수 있는 타입(Ingredient)인지 확인
        if (player.heldItem is Ingredient itemToPlace)
        {
            
            itemToPlace.transform.SetParent(itemSlot != null ? itemSlot : transform);
            itemToPlace.transform.localPosition = Vector3.zero;
            itemToPlace.transform.localRotation = Quaternion.identity;

            if (itemToPlace.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.isKinematic = true;
            }

            placedItem = itemToPlace;
            player.heldItem = null; 

            Debug.Log("아이템을 선반에 놓았다.");
        }
        else
        {
            Debug.LogWarning("재료 아이템만 놓을 수 있다.");
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