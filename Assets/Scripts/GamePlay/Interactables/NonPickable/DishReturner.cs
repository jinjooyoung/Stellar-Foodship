using UnityEngine;
using TMPro;

public class DishReturner : NonPickable
{
    
    public override bool canPlace => false;

    [Header("Prefabs")]
    [SerializeField] private GameObject dishPrefab;
    [SerializeField] private GameObject UIGroupPrefab;

    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI canTakeDishUI;
    [SerializeField] private Transform uiParent;

    private int dishCount = 0;

    private void Awake()
    {
        // 초기 설정 (레벨 매니저에서 가져올 수도 있음)
        dishCount = 1;
        UpdateUI();
    }

    // 2. 상호작용 1 구현 (오버라이드)
    public override void Interact(Player player)
    {
        // 플레이어의 손(heldItem)이 비어 있는지 확인
        if (player.heldItem != null) return;

        TakeOutDish(player);
    }

    // 3. 상호작용 2 구현 (오버라이드 - 기능 없음)
    public override void InteractSecondary(Player player)
    {
        // 빈 기능으로 둠
    }

    private void TakeOutDish(Player player)
    {
        Dish resultComponent = CreateDish();
        if (resultComponent == null) return;

        // 접시 스크립트의 TryPickUp 호출 (Player를 인자로 전달)
        if (resultComponent.TryPickUp(player))
        {
            // NonPickable의 heldItem이 아니라 Player의 heldItem을 설정하는 로직
            player.heldItem = resultComponent as Pickable;
            UpdateUI();
        }
        else
        {
            Destroy(resultComponent.gameObject);
        }
    }

    private Dish CreateDish()
    {
        if (dishPrefab == null || UIGroupPrefab == null) return null;
        if (LevelManager.Instance.maxDishCount <= dishCount) return null;

        // UI 생성
        GameObject uiGroupObj = Instantiate(UIGroupPrefab, uiParent);
        FollowWorldUI followWorldUI = uiGroupObj.GetComponent<FollowWorldUI>();
        CookingIconUI cookingIconUI = uiGroupObj.GetComponent<CookingIconUI>();

        // 접시 생성
        GameObject dishObj = Instantiate(dishPrefab);
        Dish dish = dishObj.GetComponent<Dish>();

        // 데이터 연결
        followWorldUI.target = dishObj.transform;
        followWorldUI.cam = Camera.main;
        dish.cookingIconUI = cookingIconUI;

        dishCount++;
        return dish;
    }

    public void UpdateUI()
    {
        int count = LevelManager.Instance.maxDishCount - dishCount;
        if (canTakeDishUI != null)
        {
            canTakeDishUI.text = count.ToString();
        }
    }
}