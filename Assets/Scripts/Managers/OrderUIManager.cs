using System.Collections.Generic;
using UnityEngine;

public class OrderUIManager : MonoBehaviour
{
    [Header("[참조]")]
    [SerializeField] private GameObject orderUIPrefab;
    [SerializeField] private Transform uiParent;
    [Header("[런타임]")]
    [SerializeField] private List<OrderUI> orderUIs = new List<OrderUI>();

    public void CreateOrderUI(Order order, DishSO dish, OrderManager manager)
    {
        GameObject obj = Instantiate(orderUIPrefab, uiParent);
        OrderUI ui = obj.GetComponent<OrderUI>();
        if (ui != null)
        {
            ui.Init(dish.id, order);
            orderUIs.Add(ui);
            ui.timer.StartTimer(order.timeLimit);
            ui.timer.OnCompleted += () => { manager.FailOrder(order); };
        }
    }

    public void RemoveOrderUI(int index)
    {
        if (index < 0 || index >= orderUIs.Count) return;
        GameObject obj = orderUIs[index].gameObject;
        orderUIs.RemoveAt(index);
        Destroy(obj);
    }
}