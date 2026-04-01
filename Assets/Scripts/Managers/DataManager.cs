using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    public IngredientDatabaseSO ingredientDatabase;
    public CookedIngredientDatabaseSO cookedIngredientDatabase;
    public DishDatabaseSO dishDatabase;
    public AchievementDatabaseSO achievementDatabase;
    public SoundDatabaseSO soundDatabase;

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
        ingredientDatabase.Initialize();
        cookedIngredientDatabase.Initialize();
        dishDatabase.Initialize();
        achievementDatabase.Initialize();
        soundDatabase.Initialize();
    }
}
