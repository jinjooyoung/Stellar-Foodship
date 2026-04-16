using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements;

public abstract class Pickable : MonoBehaviour, IInteractable
{
    // 재료든 조리도구(도구오브젝트 자체에 ID가 있는건 아니지만 도구에 들어있는 조리된 1차 조합물에 ID가 있으니)든 요리든 ID 있어서 선언함
    public abstract int ID { get; }
    private bool isFlying = false;

    //==================================공통 기능======================================

    // 상호작용1: "집기 / 놓기" 공통 처리 | J / Button South
    public virtual void Interact(Player player)
    {
        if (player.heldItem != null)
        {
            TryCombineWithHeld(player);
            return;
        }

        if (TryPickUp(player))
        {
            player.heldItem = this;
        }
    }

    public virtual void InteractSecondary(Player player) { }

    // 픽커블 -> Player가 들기
    public virtual bool TryPickUp(Player player)
    {
        // NonPickable에서 떨어뜨리기
        NonPickable parentSlot = GetComponentInParent<NonPickable>();
        if (parentSlot != null)
        {
            parentSlot.TakeItem(player);
        }

        // 위치 이동
        Transform t = transform;
        t.SetParent(player.holdPoint);
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;

        // 물리 처리
        Rigidbody rb = GetComponent<Rigidbody>();
        Collider col = GetComponent<Collider>();

        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        if (col != null)
        {
            col.enabled = false;
        }

        return true;
    }

    void TryCombineWithHeld(Player player)
    {
        Pickable held = player.heldItem;

        // Ingredient → Cookware
        if (held is Ingredient ing && this is Cookware cook)
        {
            bool canAdd = !ing.ingredientData.isCutable || ing.isCut;

            if (canAdd && cook.currentIngredientIds.Count < 4)
            {
                cook.AddIngredient(ing);
                player.heldItem = null;
                return;
            }
        }

        // Ingredient → Dish
        if (held is Ingredient ing2 && this is Dish dish)
        {
            bool canAdd =
                ing2.ingredientData.isRawPlatable &&
                (!ing2.ingredientData.isCutable || ing2.isCut);

            if (canAdd && dish.currentIngredientIds.Count < 4)
            {
                dish.currentIngredientIds.Add(ing2.ID);
                dish.cookingIconUI?.UpdateUI(dish.currentIngredientIds);

                Destroy(ing2.gameObject);
                player.heldItem = null;
                return;
            }
        }

        // Cookware → Dish
        if (held is Cookware cook2 && this is Dish dish2)
        {
            if (!cook2.isComplete || cook2.isBurnt) return;
            if (dish2.currentIngredientIds.Count >= 4) return;
            if (cook2.currentIngredientIds.Count == 0) return;

            int resultId = CookingSystem.GetCookedIngredientId(
                cook2.currentIngredientIds,
                cook2.cookwareType,
                cook2.isBurnt
            );

            dish2.currentIngredientIds.Add(resultId);
            dish2.cookingIconUI?.UpdateUI(dish2.currentIngredientIds);

            cook2.ClearIds();
            player.heldItem = null;
            return;
        }
    }

    public void OnThrown(Vector3 direction, float force, Player thrower)
    {
        isFlying = true;

        transform.SetParent(null);

        Rigidbody rb = GetComponent<Rigidbody>();
        Collider col = GetComponent<Collider>();

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.None;

            // 포물선 핵심
            Vector3 velocity = direction.normalized * force + Vector3.up * (force * 0.5f);
            rb.linearVelocity = velocity;
        }

        if (col != null)
        {
            col.enabled = true;

            // 던진 플레이어랑 충돌 잠깐 무시
            Collider playerCol = thrower.GetComponent<Collider>();
            if (playerCol != null)
            {
                Physics.IgnoreCollision(col, playerCol, true);
                StartCoroutine(ReenableCollision(col, playerCol, 0.3f));
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isFlying) return;

        int layer = collision.gameObject.layer;

        // 1. 플레이어 맞음
        if (layer == 3)
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                if (player.heldItem == null)
                {
                    this.Interact(player);
                    isFlying = false;
                    return;
                }
            }
        }

        // 2. Pickable 맞음
        if (layer == 6)
        {
            Pickable otherPickable = collision.gameObject.GetComponent<Pickable>();
            if (otherPickable != null)
            {
                HandlePickableCollision(otherPickable);
                return;
            }
        }

        // 3. NonPickable 맞음
        if (layer == 7)
        {
            NonPickable nonPickable = collision.gameObject.GetComponent<NonPickable>();
            if (nonPickable != null)
            {
                HandleNonPickableCollision(nonPickable);
                return;
            }
        }

        // 3. 바닥 or 기타 → 그냥 떨어짐
        isFlying = false;
    }

    //=================================충돌 처리========================================
    void HandlePickableCollision(Pickable other)
    {
        // 재료 던졌을 때만 의미 있음
        if (this is Ingredient ingredient)
        {
            // 조리도구
            if (other is Cookware cookware)
            {
                bool canAdd = !ingredient.ingredientData.isCutable || ingredient.isCut;

                if (canAdd)
                {
                    cookware.AddIngredient(ingredient);
                    isFlying = false;
                    return;
                }
            }

            // 접시
            if (other is Dish dish)
            {
                bool canAdd =
                    ingredient.ingredientData.isRawPlatable &&
                    (!ingredient.ingredientData.isCutable || ingredient.isCut);

                if (canAdd && dish.currentIngredientIds.Count < 4)
                {
                    dish.currentIngredientIds.Add(ingredient.ID);
                    dish.cookingIconUI?.UpdateUI(dish.currentIngredientIds);
                    Destroy(ingredient.gameObject);
                    isFlying = false;
                    return;
                }
            }
        }

        isFlying = false;
    }

    void HandleNonPickableCollision(NonPickable nonPickable)
    {
        // 제출대
        DishSubmissionCounter counter = nonPickable.GetComponent<DishSubmissionCounter>();
        if (counter != null && this is Dish dish)
        {
            int resultId = CookingSystem.GetDishId(dish.currentIngredientIds, CookwareType.Plate);
            counter.ClearSubmitDish(resultId);

            Destroy(dish.gameObject);
            isFlying = false;
            return;
        }

        // 일반 배치
        if (nonPickable.canPlace)
        {
            if (nonPickable.TryPlaceItem(this))
            {
                isFlying = false;
                return;
            }
        }

        isFlying = false;
    }

    // 잠시 충돌 무시하는 코루틴
    IEnumerator ReenableCollision(Collider a, Collider b, float delay)
    {
        yield return new WaitForSeconds(delay);
        Physics.IgnoreCollision(a, b, false);
    }

    //=================================데이터 전달======================================

    public Transform GetTransform()
    {
        return transform;
    }
}
