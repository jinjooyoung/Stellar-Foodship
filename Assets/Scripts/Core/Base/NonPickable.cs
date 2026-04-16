using UnityEngine;

public abstract class NonPickable : MonoBehaviour, IInteractable
{
    public Pickable heldItem;
    public Transform holdPoint;
    public abstract bool canPlace { get; }

    //==================================공통 기능======================================

    public virtual bool TryPlaceItem(Pickable item)
    {
        if (item == null) return false;

        // 이미 뭔가 있으면 → 합치기
        if (heldItem != null)
        {
            OnBeforeItemRemoved(heldItem);

            Pickable result = TryCombine(item);

            if (result != null)
            {
                heldItem = result;

                Transform t = result.transform;
                t.SetParent(holdPoint);
                t.localPosition = Vector3.zero;
                t.localRotation = Quaternion.identity;

                OnAfterItemPlaced(result);

                return true;
            }

            return false;
        }

        // 그냥 올리기
        heldItem = item;

        Transform tf = item.transform;
        tf.SetParent(holdPoint);
        tf.localPosition = Vector3.zero;
        tf.localRotation = Quaternion.identity;

        Collider col = tf.GetComponent<Collider>();
        if (col != null) col.enabled = true;

        return true;
    }

    //==================================합치기======================================

    protected virtual Pickable TryCombine(Pickable incoming)
    {
        // Ingredient + Cookware
        if (heldItem is Ingredient ingOn && incoming is Cookware cookIn)
        {
            if (TryAddIngredientToCookware(ingOn, cookIn))
                return cookIn; // 결과는 조리도구
        }

        if (heldItem is Cookware cookOn && incoming is Ingredient ingIn)
        {
            if (TryAddIngredientToCookware(ingIn, cookOn))
                return cookOn; // 결과는 조리도구
        }

        // Ingredient + Dish
        if (heldItem is Ingredient ingOn2 && incoming is Dish dishIn)
        {
            if (TryAddIngredientToDish(ingOn2, dishIn))
                return dishIn; // 결과는 접시
        }

        if (heldItem is Dish dishOn && incoming is Ingredient ingIn2)
        {
            if (TryAddIngredientToDish(ingIn2, dishOn))
                return dishOn; // 결과는 접시
        }

        // Cookware -> Dish
        if (heldItem is Cookware cook && incoming is Dish dish)
        {
            if (TryServeCookwareToDish(cook, dish))
                return dish; // 결과는 접시
        }

        if (heldItem is Dish dish2 && incoming is Cookware cook2)
        {
            if (TryServeCookwareToDish(cook2, dish2))
                return dish2; // 결과는 접시
        }

        return null;
    }

    bool TryAddIngredientToCookware(Ingredient ing, Cookware cook)
    {
        if (cook.isComplete || cook.isBurnt) return false;
        if (cook.currentIngredientIds.Count >= 4) return false;

        bool canAdd = !ing.ingredientData.isCutable || ing.isCut;
        if (!canAdd) return false;

        cook.AddIngredient(ing);

        Destroy(ing.gameObject); // 항상 삭제

        return true;
    }

    bool TryAddIngredientToDish(Ingredient ing, Dish dish)
    {
        if (!ing.ingredientData.isRawPlatable) return false;
        if (ing.ingredientData.isCutable && !ing.isCut) return false;
        if (dish.currentIngredientIds.Count >= 4) return false;

        dish.currentIngredientIds.Add(ing.ID);
        dish.cookingIconUI?.UpdateUI(dish.currentIngredientIds);

        Destroy(ing.gameObject); // 항상 삭제

        return true;
    }

    bool TryServeCookwareToDish(Cookware cook, Dish dish)
    {
        if (!cook.isComplete || cook.isBurnt) return false;
        if (dish.currentIngredientIds.Count >= 4) return false;
        if (cook.currentIngredientIds.Count == 0) return false;

        int resultId = CookingSystem.GetCookedIngredientId(
            cook.currentIngredientIds,
            cook.cookwareType,
            cook.isBurnt
        );

        dish.currentIngredientIds.Add(resultId);
        dish.cookingIconUI?.UpdateUI(dish.currentIngredientIds);

        cook.ClearIds(); // 조리도구 비움 (파괴 X)

        return true;
    }

    public virtual Pickable TakeItem(Player player)
    {
        if (heldItem == null) return null;

        OnBeforeItemRemoved(heldItem);

        Pickable item = heldItem;
        heldItem = null;

        Transform t = item.GetTransform();
        t.SetParent(player.holdPoint);
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;

        // 콜라이더 끄기
        Collider col = t.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // Rigidbody 끄기
        Rigidbody rb = t.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        return item;
    }

    //==================================개별 기능======================================

    // 상호작용1: J / Button South
    public abstract void Interact(Player player);
    // 상호작용2: K / Button West
    public abstract void InteractSecondary(Player player);

    protected virtual void OnBeforeItemRemoved(Pickable item) { }
    protected virtual void OnAfterItemPlaced(Pickable item) { }

    //=================================데이터 전달======================================

    public Transform GetTransform()
    {
        return transform;
    }
}
