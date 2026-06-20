using System.Collections.Generic;
using UnityEngine;

public class OrderUIManager : MonoBehaviour
{
    [Header("[참조]")]
    [SerializeField] private GameObject orderUIPrefab;
    [SerializeField] public Transform uiParent;
    public OrderManager manager;
    [Header("[런타임]")]
    [SerializeField] private List<OrderUI> orderUIs = new List<OrderUI>();

    public void CreateOrderUI(Order order, DishSO dish)
    {
        // 생성
        GameObject obj = Instantiate(orderUIPrefab, uiParent);

        // 위치 보정
        obj.transform.localPosition = Vector3.zero;

        OrderUI ui = obj.GetComponent<OrderUI>();
        if (ui != null)
        {
            ui.Init(dish.id, order, manager);
            orderUIs.Add(ui);
            ui.timer.StartTimer(order.timeLimit);
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