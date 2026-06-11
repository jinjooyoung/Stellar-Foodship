using Fusion;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class NetworkOrderManager : NetworkBehaviour
{
    [Header("Reference")]
    public NewOrderUIManager uiManager;
    public NetworkScoreManager scoreManager;

    [Networked]
    public float SpawnTimer { get; set; }

    [Networked]
    public int OrderIndexCounter { get; set; }

    [Networked]
    public int OrderCount { get; set; }

    [Networked, Capacity(5)]
    public NetworkArray<int> OrderIds => default;

    //------------------오더 타이머-------------------

    [Networked, Capacity(5)]
    public NetworkArray<float> NetCurrentTimes => default;

    [Networked, Capacity(5)]
    public NetworkArray<float> NetMaxTimes => default;

    [Networked, Capacity(5)]
    public NetworkArray<NetworkBool> NetIsRunning => default;

    private NetworkTimer[] orderTimers =
    {
        new(),
        new(),
        new(),
        new(),
        new()
    };

    void TickOrderTimers()
    {
        for (int i = OrderCount - 1; i >= 0; i--)
        {
            if (orderTimers[i].Tick(Runner.DeltaTime))
            {
                FailOrder(i);
            }
        }

        SyncTimers();
    }

    void SyncTimers()
    {
        for (int i = 0; i < 5; i++)
        {
            NetCurrentTimes.Set(i, orderTimers[i].CurrentTime);

            NetMaxTimes.Set(i, orderTimers[i].MaxTime);

            NetIsRunning.Set(i, orderTimers[i].IsRunning);
        }
    }

    public float GetOrderProgress(int index)
    {
        if (index < 0 || index >= OrderCount)
            return 0f;

        float maxTime = NetMaxTimes[index];

        if (maxTime <= 0)
            return 0f;

        return (NetCurrentTimes[index] / maxTime);
    }

    //------------------------------------------------

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
            return;

        HandleOrderSpawn();

        TickOrderTimers();
    }

    //------------------------------------------------

    void HandleOrderSpawn()
    {
        SpawnTimer += Runner.DeltaTime;

        if (SpawnTimer <
            LevelManager.Instance.orderSpawnInterval)
            return;

        SpawnTimer = 0f;

        TryCreateOrder();
    }

    //------------------------------------------------

    void TryCreateOrder()
    {
        if (OrderCount >=
            LevelManager.Instance.maxOrderCount)
            return;

        CreateOrder();
    }

    //------------------------------------------------

    void CreateOrder()
    {
        int randomIndex =
            Random.Range(
                0,
                LevelManager.Instance.dishIDs.Count);

        int dishId =
            LevelManager.Instance.dishIDs[randomIndex];

        DishSO data = DataManager.instance.dishDatabase.GetDishById(dishId);

        float timeLimit = CalculateTimeLimit(data.ingredientIds);

        // 주문 등록
        OrderIds.Set(OrderCount, dishId);
        // 타이머 시작
        orderTimers[OrderCount].Start(timeLimit);

        OrderCount++;

        OrderIndexCounter++;

        // UI 생성
        uiManager.CreateOrderUI(dishId, timeLimit);
    }

    //------------------------------------------------

    public bool TrySubmitDish(int dishId)
    {
        if (!Object.HasStateAuthority)
            return false;

        for (int i = 0; i < OrderCount; i++)
        {
            if (OrderIds[i] == dishId)
            {
                CompleteOrder(i);

                return true;
            }
        }

        return false;
    }

    //------------------------------------------------

    void CompleteOrder(int index)
    {
        int dishId = OrderIds[index];

        DishSO dish =
            DataManager.instance
            .dishDatabase
            .GetDishById(dishId);

        if (dish == null)
            return;

        scoreManager.AddScore(dish.score);

        RemoveOrder(index);
    }

    //------------------------------------------------

    public void FailOrder(int index)
    {
        if (!Object.HasStateAuthority)
            return;

        if (index < 0 ||
            index >= OrderCount)
            return;

        scoreManager.AddScore(LevelManager.Instance.penaltyScore);

        RemoveOrder(index);
    }

    //------------------------------------------------

    void RemoveOrder(int index)
    {
        for (int i = index; i < OrderCount - 1; i++)
        {
            OrderIds.Set(i, OrderIds[i + 1]);

            orderTimers[i].CopyFrom(orderTimers[i + 1]);
        }

        orderTimers[OrderCount - 1].Reset();

        OrderCount--;

        uiManager.RemoveOrderUI(index);
    }

    //------------------------------------------------

    public List<int> GetOrderIds()
    {
        List<int> list = new();

        for (int i = 0; i < OrderCount; i++)
        {
            list.Add(OrderIds[i]);
        }

        return list;
    }

    //------------------------------------------------

    public float CalculateTimeLimit(List<int> recipe)
    {
        float time = 0f;

        foreach (int id in recipe)
        {
            if (id < 0)
                continue;

            if (id < 100)
            {
                time +=
                    LevelManager.Instance.ingreTime;
            }
            else if (id < 200)
            {
                time +=
                    LevelManager.Instance.cookedIngreTime;
            }
        }

        return time;
    }
}