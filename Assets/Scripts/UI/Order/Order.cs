using UnityEngine;

public class Order
{
    public int orderIndex;
    public int dishId;
    public int score;
    public float timeLimit;
    public DishSO dish;

    public void Initialize(int index, DishSO data, float timeLimit)
    {
        orderIndex = index;
        dishId = data.id;
        score = data.score;
        this.timeLimit = timeLimit;
        this.dish = data;
    }
}
