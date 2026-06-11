using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class NetworkOrderManager : NetworkBehaviour
{
    [Header("Reference")]
    public OrderUIManager uiManager;
    public NetworkScoreManager scoreManager;

    [Networked]
    public float SpawnTimer { get; set; }

    [Networked]
    public int OrderIndexCounter { get; set; }

    [Networked]
    public int OrderCount { get; set; }

    [Networked, Capacity(5)]
    public NetworkArray<int> OrderIds => default;

    //------------------------------------------------

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
            return;

        HandleOrderSpawn();
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

        OrderIds.Set(OrderCount, dishId);

        OrderCount++;

        OrderIndexCounter++;
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
        }

        OrderCount--;
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