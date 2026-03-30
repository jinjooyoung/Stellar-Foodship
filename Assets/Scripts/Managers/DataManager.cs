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

            InitializeDatabases();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeDatabases()
    {
        IngredientDatabase.Initialize();
        CookedIngredientDatabase.Initialize();
        DishDatabase.Initialize();
        AchievementDatabase.Initialize();
        SoundDatabase.Initialize();
    }
}
