using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    public IngredientDatabaseSO IngredientDatabase;
    public CookedIngredientDatabaseSO CookedIngredientDatabase;
    public DishDatabaseSO DishDatabase;
    public AchievementDatabaseSO AchievementDatabase;
    public SoundDatabaseSO SoundDatabase;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
