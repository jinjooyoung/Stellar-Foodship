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
          
            ui.Init(dish.dishId, order);

            // 4. 리스트에 추가
            orderUIs.Add(ui);

            // 5. 타이머 시작
            ui.timer.StartTimer(order.timeLimit);

           
            {
                manager.FailOrder(order);
            };
        }
    }

   
    public void RemoveOrderUI(int index)
    {
        if (index < 0 || index >= orderUIs.Count) return;

        // 1. 해당 인덱스의 GameObject 참조
        GameObject obj = orderUIs[index].gameObject;

        // 2. 리스트에서 제거
        orderUIs.RemoveAt(index);

        // 3. 오브젝트 파괴
        Destroy(obj);
    }
}