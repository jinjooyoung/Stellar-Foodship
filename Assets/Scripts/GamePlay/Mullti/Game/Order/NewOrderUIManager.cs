using System.Collections.Generic;
using UnityEngine;

public class NewOrderUIManager : MonoBehaviour
{
    [Header("ÂüÁ¶")]
    [SerializeField]
    private GameObject orderUIPrefab;

    [SerializeField]
    private Transform uiParent;

    public NetworkOrderManager manager;

    //------------------------------------------------

    [SerializeField]
    private List<NewOrderUI> orderUIs = new();

    //------------------------------------------------

    public void CreateOrderUI(int dishId, float timeLimit)
    {
        GameObject obj = Instantiate(orderUIPrefab, uiParent);

        obj.transform.localPosition = Vector3.zero;

        NewOrderUI ui = obj.GetComponent<NewOrderUI>();

        if (ui == null) return;

        ui.Init(dishId, manager);

        orderUIs.Add(ui);

        ui.timer.Start(timeLimit);
    }

    //------------------------------------------------

    public void RemoveOrderUI(int index)
    {
        if (index < 0 || index >= orderUIs.Count) return;

        GameObject obj = orderUIs[index].gameObject;

        orderUIs.RemoveAt(index);

        Destroy(obj);
    }

    //------------------------------------------------

    public void ClearAll()
    {
        foreach (var ui in orderUIs)
        {
            if (ui != null)
            {
                Destroy(ui.gameObject);
            }
        }

        orderUIs.Clear();
    }
}