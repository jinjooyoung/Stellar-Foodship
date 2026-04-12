using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements;

public abstract class Pickable : MonoBehaviour, IInteractable
{
    // 재료든 조리도구(도구오브젝트 자체에 ID가 있는건 아니지만 도구에 들어있는 조리된 1차 조합물에 ID가 있으니)든 요리든 ID 있어서 선언함
    public abstract int ID { get; }
    private bool isFlying = false;

    // 생성될 때 플레이어와 충돌 판정 안 하도록 세팅
    void Awake()
    {
        Collider myCol = GetComponent<Collider>();
        if (myCol == null) return;

        // "Player" 태그가 붙은 오브젝트만 검색
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (var pObj in playerObjects)
        {
            Collider pCol = pObj.GetComponent<Collider>();
            if (pCol != null)
            {
                Physics.IgnoreCollision(myCol, pCol);
            }
        }
    }

    //==================================공통 기능======================================

    // 상호작용1: "집기 / 놓기" 공통 처리 | J / Button South
    public virtual void Interact(Player player)
    {
        // Debug.Log($"{this.name} Pickable 상호작용 호출됨");

        if (player.heldItem != null) return;

        if (TryPickUp(player))
        {
            player.heldItem = this;
        }
    }
        /*들고 있을 때는 Pickable.Interact 호출 안 됨. 
    }     (Player.InteractPrimary에서 처리)*/

        // 상호작용2: 던지기 | K / Button West
    public virtual void InteractSecondary(Player player)
    {
        // 픽커블은 상호작용2키 필요 없을 듯 근데 혹시 모르니 일단 냅두고 나중에 확실해지면 인터페이스부터 코드 수정
    }

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

    public void OnThrown(Vector3 direction, float force)
    {
        isFlying = true;

        Transform t = transform;
        t.SetParent(null);

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
            // 재료를 던졌을때만 픽커블이랑 상호작용함
            if (this is Ingredient ingredient)
            {
                Cookware cookware = collision.gameObject.GetComponent<Cookware>();
                Dish dish = collision.gameObject.GetComponent<Dish>();

                // 닿은 픽커블이 조리도구인지, 접시인지
                if (cookware != null)
                {
                    bool canAdd = !ingredient.ingredientData.isCutable || ingredient.isCut;
                    if (canAdd)
                    {
                        cookware.AddIngredient(ingredient);
                        isFlying = false;
                        return;
                    }
                    else
                    {
                        Debug.Log("던지기 : 재료가 썰리지 않아 넣을 수 없습니다.");
                        isFlying = false;
                        return;
                    }
                }
                else if (dish != null)
                {
                    // 생으로 담을 수 없는 재료면 return
                    if (!ingredient.ingredientData.isRawPlatable)
                    {
                        Debug.Log("던지기 : 생으로 담을 수 없는 재료!");
                        isFlying = false;
                        return;
                    }

                    // 썰 수 있는 재료인지 확인
                    if (ingredient.ingredientData.isCutable)
                    {
                        // 썰려있지 않으면 담을 수 없음
                        if (!ingredient.isCut)
                        {
                            Debug.Log("던지기 : 재료가 썰려있지 않아, 담을 수 없습니다.");
                            isFlying = false;
                            return;
                        }
                    }

                    // 접시가 이미 가득 찼으면
                    if (dish.currentIngredientIds.Count >= 4)
                    {
                        Debug.Log("던지기 : 접시가 이미 가득 차서 넣을 수 없습니다!");
                        isFlying = false;
                        return;
                    }

                    dish.currentIngredientIds.Add(ingredient.ID);
                    dish.cookingIconUI?.UpdateUI(dish.currentIngredientIds);
                    Destroy(ingredient.gameObject);

                    isFlying = false;
                    return;
                }
            }
        }

        // 3. NonPickable 맞음
        if (layer == 7)
        {
            NonPickable nonPickable = collision.gameObject.GetComponent<NonPickable>();
            if (nonPickable != null)
            {
                DishSubmissionCounter counter = collision.gameObject.GetComponent<DishSubmissionCounter>();
                // 맞은게 요리 제출 창구
                if (counter != null)
                {
                    // 던진게 접시
                    if (this is Dish dish)
                    {
                        int resultId = CookingSystem.GetDishId(dish.currentIngredientIds, CookwareType.Plate);
                        counter.ClearSubmitDish(resultId);

                        Destroy(dish.cookingIconUI.gameObject);
                        Destroy(dish.gameObject);

                        isFlying = false;
                        return;
                    }
                }

                if (nonPickable.canPlace)
                {
                    if (nonPickable.TryPlaceItem(this))
                    {
                        isFlying = false;
                        return;
                    }
                }
            }
        }

        // 3. 바닥 or 기타 → 그냥 떨어짐
        isFlying = false;
    }

    //=================================데이터 전달======================================

    public Transform GetTransform()
    {
        return transform;
    }
}
