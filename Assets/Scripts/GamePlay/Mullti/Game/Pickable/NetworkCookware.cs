using Fusion;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkCookware : NewPickable
{
    public override int ID => ResultId;

    [Header("Cookware")]

    [Networked]
    public int ResultId { get; set; }

    public CookwareType cookwareType;

    [Networked, Capacity(4), OnChangedRender(nameof(OnIngredientChanged))]
    NetworkArray<int> IngredientIds => default;

    [Networked]
    public int IngredientCount { get; set; }

    [Networked]
    public NetworkBool IsComplete { get; set; }

    [Networked]
    public NetworkBool IsBurnt { get; set; }

    [Networked]
    public float NetCurrentTime { get; set; }

    [Networked]
    public float NetMaxTime { get; set; }

    [Networked]
    public NetworkBool NetIsRunning { get; set; }

    public CookingIconUI cookingIconUI;

    public GameObject visualObject;

    public GameObject checkImage;
    [SerializeField] private Slider timerSlider;

    NetworkTimer timer = new();

    //----------------------------------------------------



    public override void Spawned()
    {
        base.Spawned();

        if (Object.HasStateAuthority)
        {
            for (int i = 0; i < 4; i++)
                IngredientIds.Set(i, -1);
        }

        OnIngredientChanged();
        
        checkImage.SetActive(IsComplete);
    }

    public override void Render()
    {
        base.Render();

        checkImage.SetActive(IsComplete);

        UpdateTimerUI();
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        Debug.Log($"{name} FixedUpdate");
        if (!Object.HasStateAuthority)
            return;

        Debug.Log($"Tick : {timer.CurrentTime}");

        if (timer.Tick(Runner.DeltaTime))
        {
            CookingComplete();
        }

        SyncTimer();
    }

    //----------------------------------------------------

    void SyncTimer()
    {
        NetCurrentTime = timer.CurrentTime;
        NetMaxTime = timer.MaxTime;
        NetIsRunning = timer.IsRunning;
    }

    void UpdateTimerUI()
    {
        if (timerSlider == null)
            return;

        if (NetMaxTime <= 0)
        {
            timerSlider.gameObject.SetActive(false);
            return;
        }

        timerSlider.gameObject.SetActive(NetIsRunning);

        timerSlider.value =
            1f - NetCurrentTime / NetMaxTime;
    }

    //----------------------------------------------------

    void OnIngredientChanged()
    {
        visualObject.SetActive(IngredientCount > 0);

        Debug.Log($"OnIngredientChanged 호출");
        Debug.Log($"IngredientCount = {IngredientCount}");

        List<int> list = GetIngredientList();

        Debug.Log($"List Count = {list.Count}");

        foreach (var id in list)
            Debug.Log($"ID : {id}");

        cookingIconUI?.UpdateUI(list);
    }

    //----------------------------------------------------

    public List<int> GetIngredientList()
    {
        List<int> list = new();

        for (int i = 0; i < IngredientCount; i++)
            list.Add(IngredientIds[i]);

        return list;
    }

    //----------------------------------------------------

    public bool TryAddIngredient(NetworkIngredient ingredient)
    {
        if (!Object.HasStateAuthority)
            return false;

        if (IngredientCount >= 4)
            return false;

        bool canAdd = !ingredient.ingredientData.isCutable || ingredient.IsCut;

        if (!canAdd)
            return false;

        IngredientIds.Set(IngredientCount, ingredient.ID);

        IngredientCount++;

        Runner.Despawn(ingredient.Object);

        OnIngredientChanged();

        return true;
    }

    //----------------------------------------------------

    public void StartCooking(float cookTime)
    {
        Debug.Log($"StartCooking 호출 : {cookTime}");

        timer.Start(cookTime);
    }

    public void ResumeCooking()
    {
        Debug.Log("ResumeCooking 호출");

        timer.Resume();
    }

    public void StopCooking()
    {
        timer.Stop();
    }

    public void AddCookTime(float t)
    {
        timer.AddTime(t);
    }

    //----------------------------------------------------

    void CookingComplete()
    {
        IsComplete = true;

        ResultId =
            CookingSystem.GetCookedIngredientId(
                GetIngredientList(),
                cookwareType,
                IsBurnt);
    }

    //----------------------------------------------------

    public void Clear()
    {
        ResultId = -1;

        for (int i = 0; i < 4; i++)
        {
            IngredientIds.Set(i, -1);
        }

        IngredientCount = 0;

        IsComplete = false;
        IsBurnt = false;

        timer.Reset();

        SyncTimer();

        OnIngredientChanged();

        checkImage.SetActive(false);
    }

    //----------------------------------------------------

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