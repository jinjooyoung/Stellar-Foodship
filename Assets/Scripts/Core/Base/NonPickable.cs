using UnityEngine;

public abstract class NonPickable : MonoBehaviour, IInteractable
{
    [Header("����")]
    public IInteractable heldItem;
    public Transform holdPoint;

    // ������ ���� �����ϸ� ���� �÷��� �ִ� �� ��� null�ε� ������ ���� ���̴ϱ� �ּ�ó���ص�
    /*void Awake()
    {
        heldItem = null;
    }*/

    //==================================���� ���======================================

    // �⺻�����δ� ����Ŀ�� ���� �� �ø�
    /*�� �ּ��� �����Ͻø� �����ּ���!!
    ����, ������, ����, ������ ������ �ø� �� �ְ� ����â��, ���ø��ʱ� ������ �ø� �� ����.
    �׷��� �⺻�����δ� false ���� �� ������� ��ܿ� if Ÿ��.CanPlace�� üũ �Ŀ� �ø���
    ����Ŀ�� ��� Ŭ�������� override�� ������ => true, ������ return item is Ingredient �̷�������
    �ø� ��Ŀ���� ���������� ���� �� �ø� �� �ִ��� �������� override*/
    public virtual bool CanPlace(Pickable item) => false;

    // ����Ŀ�� ���� ������ �ø���
    public virtual bool TryPlaceItem(Pickable item)
    {
        if (heldItem != null || item == null) return false;

        heldItem = item;
        Debug.Log($"{this.name} helditem {heldItem.ToString()}");

        Transform t = item.GetTransform();
        t.SetParent(holdPoint);
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;

        // �ݶ��̴� ���� (Pickup���� ���� ��)
        Collider col = t.GetComponent<Collider>();
        if (col != null) col.enabled = true;
        Debug.Log("TryPlaceItem ȣ��");
        return true;
    }

    // ����Ŀ�� ���� �ִ� ������ -> Player�� ���
    public virtual IInteractable TakeItem(Player player)
    {
        Debug.Log("����Ŀ�� ����ũ������ ȣ���");
        if (heldItem == null)
        {
            Debug.Log("����Ŀ�� ����ũ������ > �������� null ���ϵ�");
            return null;
        }

        IInteractable item = heldItem;
        heldItem = null;

        //item.GetTransform().SetParent(null);
        item.GetTransform().SetParent(player.holdPoint);
        item.GetTransform().localPosition = Vector3.zero;
        item.GetTransform().localRotation = Quaternion.identity;

        // �ݶ��̴� ���� (TryPlaceItem���� ���� ��)
        Collider col = item.GetTransform().GetComponent<Collider>();
        if (col != null) col.enabled = false;

        return item;
    }

    //==================================���� ���======================================

    // ��ȣ�ۿ�1 : J / Button South
    public abstract void Interact(Player player);
    // ��ȣ�ۿ�2 : K / Button West
    public abstract void InteractSecondary(Player player);

    //=================================������ ����======================================

    public Transform GetTransform()
    {
        return transform;
    }
}
