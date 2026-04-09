using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    [Header("참조")]
    public OrderUIManager uiManager;
    public ScoreManager scoreManager;

    // 런타임
    private List<Order> orders = new List<Order>();

    [SerializeField] private int comboCount = 0;
    [SerializeField] private float spawnTimer = 0f;
    [SerializeField] private int orderIndexCounter = 0;

    void Update()
    {
        HandleOrderSpawn();
    }

    //====================================주문 생성====================================

    void HandleOrderSpawn()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer < LevelManager.Instance.orderSpawnInterval) return;

        spawnTimer = 0f;
        TryCreateOrder();
    }

    void TryCreateOrder()
    {
        if (orders.Count >= LevelManager.Instance.maxOrderCount) return;

        CreateOrder();
    }

    void CreateOrder()
    {
        // 랜덤 요리 id 선택
        int id = Random.Range(LevelManager.Instance.minOrderId, LevelManager.Instance.maxOrderId + 1);

        // 데이터매니저에서 요리SO 가져오기
        DishSO dish = DataManager.instance.dishDatabase.GetDishById(id);
        if (dish == null) return;
        if (dish.ingredientIds == null) return;

        // 제한 시간 계산
        float timeLimit = CalculateTimeLimit(dish.ingredientIds);

        // 주문 생성
        Order order = new Order();
        order.Initialize(orderIndexCounter++, dish, timeLimit);

        orders.Add(order);

        // UI 생성
        uiManager.CreateOrderUI(order, dish, this);
    }

    //====================================주문 완료====================================

    public void CompleteOrder(int index)
    {
        Order order = orders[index];

        int earliest = GetEarliestOrderIndex();

        if (order.orderIndex == earliest)
        {
            comboCount++;
            if (comboCount > 5) comboCount = 5;
        }
        else
        {
            comboCount = 0;
        }

        // 점수 계산
        int bonus = scoreManager.GetComboBonus(comboCount);
        int totalScore = order.score + bonus;
        scoreManager.AddScore(totalScore);

        RemoveOrder(index);
    }

    //====================================주문 실패====================================

    public void FailOrder(Order order)
    {
        int index = orders.IndexOf(order);
        if (index < 0) return;

        comboCount = 0;
        scoreManager.AddScore(LevelManager.Instance.penaltyScore);

        RemoveOrder(index);
    }

    //====================================주문 제출====================================

    public bool TrySubmitDish(int id)
    {
        for (int i = 0; i < orders.Count; i++)
        {
            if (orders[i].dishId == id)
            {
                CompleteOrder(i);
                return true;
            }
        }

        // 제출했는데 요리가 없으면 콤보만 끊김
        comboCount = 0;
        return false;
    }

    //====================================유틸====================================

    void RemoveOrder(int index)
    {
        orders.RemoveAt(index);
        uiManager.RemoveOrderUI(index);
    }

    int GetEarliestOrderIndex()
    {
        int min = int.MaxValue;
        foreach (var order in orders)
        {
            if (order.orderIndex < min)
                min = order.orderIndex;
        }
        return min;
    }

    float CalculateTimeLimit(int?[] input)
    {
        float time = 0f;

        foreach (var id in input)
        {
            if (!id.HasValue) continue;

            if (id < 100)
            {
                time += LevelManager.Instance.ingreTime;
            }
            else if (id < 200)
            {
                time += LevelManager.Instance.cookedIngreTime;
            }
        }

        return time;
    }
}