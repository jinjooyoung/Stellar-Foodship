using System;
using System.Linq;
using UnityEngine;

public class Cookware : Pickable
{
    [Header("Cooking Settings")]
    public int resultId; 
    public CookwareType cookwareType;
    public int?[] currentIngredientIds = new int?[4];
    public Timer timer;
    public CookingIconUI cookingIconUI;
    public bool isComplete;
    public GameObject visualObject;
    public GameObject checkImage;
   
    public override int ID => resultId;

    private void Awake()
    {
        visualObject.SetActive(false);
        checkImage.SetActive(false);
    }

    private void Start()
    {
        cookingIconUI.UpdateUI(currentIngredientIds);
    }

    /*private void Update()
    {
        Debug.Log($"{currentIngredientIds[0]},{currentIngredientIds[1]},{currentIngredientIds[2]},{currentIngredientIds[3]}");
    }*/

    public override void Interact(Player player)
    {
        Debug.Log("조리도구 인터랙트 호출됨");
        if (player.heldItem != null)
        {
            Debug.Log("조리도구 인터랙트 플레이어 헬드아이템 있음");
            if (player.heldItem is Ingredient ingredient)
            {
                Debug.Log("조리도구 인터랙트 플레이어가 재료를 들고있음");
                HandleIngredientInput(player, ingredient);
            }
        }
        else
        {
            Debug.Log("조리도구 인터랙트 플레이어 헬드아이템 없음");
            if (TryPickUp(player))
            {
                player.heldItem = this;
            }
        }
    }

    private void HandleIngredientInput(Player player, Ingredient ingredient)
    {
        Debug.Log("핸들 재료 인풋 호출됨");
        bool canAdd = !ingredient.ingredientData.isCutable || ingredient.isCut;
        Debug.Log($"조리도구 넣기 가능 여부 : {canAdd.ToString()}");
        if (canAdd)
        {
            AddIngredient(player, ingredient);
        }
        else
        {
            Debug.Log("재료가 썰리지 않아 넣을 수 없습니다.");
        }
    }

    private void AddIngredient(Player player, Ingredient ingredient)
    {
        
        if (!currentIngredientIds.Any(id => id == null))
        {
            Debug.Log("조리 도구가 이미 가득 찼습니다");
            return;
        }

        for (int i = 0; i < currentIngredientIds.Length; i++)
        {
            if (currentIngredientIds[i] == null)
            {
                currentIngredientIds[i] = ingredient.ingredientID;
                break;
            }
        }
        visualObject.SetActive(true);
        cookingIconUI.UpdateUI(currentIngredientIds);

        Destroy(ingredient.gameObject);
        player.heldItem = null;
    }

    public StationType GetRequiredStation()
    {
        return cookwareType switch
        {
            CookwareType.Pan => StationType.FirePit,
            CookwareType.Pot => StationType.FirePit,
            CookwareType.Steamer => StationType.FirePit,
            CookwareType.MixerCup => StationType.Blender,
            CookwareType.FryerBasket => StationType.Fryer,
            _ => throw new Exception("Unknown CookwareType")
        };
    }

    public bool HasAnyValue(int?[] arr)
    {
        for(int i = 0;i < arr.Length; i++)
        {
            if (arr[i].HasValue)
                return true;
        }
        return false;
    }

    public void OnCookingComplete()
    {
        isComplete = true;

        checkImage.SetActive(true);
    }

    public override void InteractSecondary(Player player)
    {
       
    }
}