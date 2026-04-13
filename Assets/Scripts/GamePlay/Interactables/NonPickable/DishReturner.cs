using UnityEngine;
using TMPro;

public class DishReturner : NonPickable
{
    public bool _canPlace;
    public override bool canPlace => _canPlace;

    [Header("Prefabs")]
    [SerializeField] private GameObject dishPrefab;
    [SerializeField] private GameObject UIGroupPrefab;

    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI canTakeDishUI;
    [SerializeField] private Transform uiParent;

    

    private int dishCount = 0;

    private void Start()
    {
        Invoke("SafeInit", 0.1f);
    }

    private void SafeInit()
    {
        UpdateUI();
    }

    public override void Interact(Player player)
    {
        Debug.Log("상호작용 버튼이 눌렸습니다!");
        if (LevelManager.Instance == null) return;
        if (player.heldItem != null) return;

        TakeOutDish(player);
    }

    private void TakeOutDish(Player player)
    {
        Dish resultComponent = CreateDish();
        if (resultComponent == null) return;

        if (resultComponent.TryPickUp(player))
        {
            player.heldItem = resultComponent;
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

        GameObject dishObj = Instantiate(dishPrefab);

        // <--- 여기서 holdPoint 변수를 사용하여 위치를 잡습니다! ---
        if (holdPoint != null)
        {
            dishObj.transform.position = holdPoint.position;
            dishObj.transform.rotation = holdPoint.rotation;
        }
        else
        {
            dishObj.transform.position = transform.position + Vector3.up;
        }

        GameObject uiGroupObj = Instantiate(UIGroupPrefab, uiParent);
        FollowWorldUI followWorldUI = uiGroupObj.GetComponent<FollowWorldUI>();
        Dish dish = dishObj.GetComponent<Dish>();
        dish.cookingIconUI = uiGroupObj.GetComponent<CookingIconUI>();

        if (followWorldUI != null)
        {
            followWorldUI.uiTargetTransform = dishObj.transform;
            followWorldUI.uiWorldCamera = Camera.main;
        }

        dishCount++;
        return dish;
    }

    public void UpdateUI()
    {
        if (LevelManager.Instance == null) return;
        int count = LevelManager.Instance.maxDishCount - dishCount;
        if (canTakeDishUI != null) canTakeDishUI.text = count.ToString();
    }

    public override void InteractSecondary(Player player) { }
}