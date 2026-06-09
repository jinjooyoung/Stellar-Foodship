using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class NetworkCookware : NewPickable
{
    public override int ID => ResultId;

    [Header("Cookware")]
    [Networked] public int ResultId { get; set; }
    public CookwareType cookwareType;

    public List<int> currentIngredientIds = new();

    public CookingIconUI cookingIconUI;
    public GameObject visualObject;
    public GameObject checkImage;

    [Networked] public NetworkBool IsComplete { get; set; }
    [Networked] public NetworkBool IsBurnt { get; set; }

    [Networked] public float NetCurrentTime { get; set; }
    [Networked] public float NetMaxTime { get; set; }
    [Networked] public NetworkBool NetIsRunning { get; set; }

    private NetworkTimer timer = new();

    public override void Spawned()
    {
        base.Spawned();

        if (visualObject != null)
            visualObject.SetActive(currentIngredientIds.Count > 0);

        if (checkImage != null)
            checkImage.SetActive(IsComplete);

        cookingIconUI?.UpdateUI(currentIngredientIds);
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (!Object.HasStateAuthority)
            return;

        if (timer.Tick(Runner.DeltaTime))
        {
            CookingComplete();
        }

        NetCurrentTime = timer.CurrentTime;
        NetMaxTime = timer.MaxTime;
        NetIsRunning = timer.IsRunning;
    }

    public override void Render()
    {
        base.Render();

        if (checkImage != null)
            checkImage.SetActive(IsComplete);
    }

    //================================================

    public void AddIngredient(NetworkIngredient ingredient)
    {
        if (currentIngredientIds.Count >= 4)
            return;

        currentIngredientIds.Add(ingredient.ID);

        visualObject?.SetActive(true);

        cookingIconUI?.UpdateUI(currentIngredientIds);

        Runner.Despawn(ingredient.Object);
    }

    //================================================

    public void StartCooking(float cookTime)
    {
        if (timer.IsRunning)
            return;

        timer.Start(cookTime);
    }

    public void ResumeCooking()
    {
        timer.Resume();
    }

    public void StopCooking()
    {
        timer.Stop();
    }

    public void AddCookTime(float time)
    {
        timer.AddTime(time);
    }

    //================================================

    void CookingComplete()
    {
        IsComplete = true;

        if (checkImage != null)
            checkImage.SetActive(true);

        resultId = CookingSystem.GetCookedIngredientId(
            currentIngredientIds,
            cookwareType,
            false
        );
    }

    //================================================

    public void ClearIds()
    {
        resultId = -1;

        currentIngredientIds.Clear();

        IsComplete = false;
        IsBurnt = false;

        timer.Reset();

        NetCurrentTime = 0;
        NetMaxTime = 0;
        NetIsRunning = false;

        visualObject?.SetActive(false);
        checkImage?.SetActive(false);

        cookingIconUI?.UpdateUI(currentIngredientIds);
    }

    //================================================

    public StationType GetRequiredStation()
    {
        return cookwareType switch
        {
            CookwareType.Pan => StationType.FirePit,
            CookwareType.Pot => StationType.FirePit,
            CookwareType.Steamer => StationType.FirePit,
            CookwareType.MixerCup => StationType.Blender,
            CookwareType.FryerBasket => StationType.Fryer,
            _ => StationType.FirePit
        };
    }
}