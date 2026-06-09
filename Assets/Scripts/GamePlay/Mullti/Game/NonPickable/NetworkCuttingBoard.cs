using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class NetworkCuttingBoard : NewNonPickable
{
    [Header("Cutting")]
    [Networked]
    public int CutProgress { get; set; }

    [SerializeField]
    private Slider progressBar;

    public override bool CanPlace => true;

    //================================================

    public override void Interact(NewPlayer player)
    {
        if (CutProgress > 0 && CutProgress < 100) return;

        base.Interact(player);
    }

    // 우클릭
    public override void InteractSecondary(NewPlayer player)
    {
        if (!Object.HasStateAuthority)
            return;

        if (HeldItem == null)
            return;

        NetworkIngredient ingredient =
            HeldItem.GetComponent<NetworkIngredient>();

        if (ingredient == null)
            return;

        // 썰 수 없는 재료
        if (!ingredient.ingredientData.isCutable)
            return;

        // 이미 썰림
        if (ingredient.IsCut)
            return;

        CutProgress += 20;

        if (CutProgress >= 100)
        {
            CutProgress = 100;

            ingredient.OnCutComplete();

            Debug.Log("손질 완료");
        }
    }

    //================================================

    protected override bool TakeItem(NewPlayer player)
    {
        // 써는 중에는 못 집음
        if (CutProgress > 0 &&
            CutProgress < 100)
        {
            Debug.Log("손질 중인 재료입니다.");
            return false;
        }

        return base.TakeItem(player);
    }

    //================================================

    protected override void OnItemPlaced(NewPickable item)
    {
        CutProgress = 0;
    }

    protected override void OnItemTaken(NewPickable item)
    {
        CutProgress = 0;
    }

    //================================================

    public float GetNormalizedProgress()
    {
        return CutProgress / 100f;
    }

    public override void Render()
    {
        base.Render();

        if (progressBar != null)
        {
            progressBar.value =
                CutProgress / 100f;

            progressBar.gameObject.SetActive(
                HeldItem != null &&
                CutProgress > 0 &&
                CutProgress < 100);
        }
    }
}