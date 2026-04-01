#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Mesh;

public enum ConversionType
{
    Ingredients,
    CookedIngredients,
    Dishes,
    Achievements
}

[Serializable]
public class DialogRowData
{
    public int? id;     //int?는 Nullable<int>의 축약형. null값도 가질 수 있는 정수형
    public string characterName;
    public string text;
    public int? nextId;
    public string portraitPath;
    public string choiceText;
    public int? choiceNextId;
}

public class JsonToScriptableConverter : EditorWindow
{
    private string jsonFilePath = "";                                           //JSON 파일 경로 문자열 값
    private string outputFolder = "Assets/Data/Generated";                      //출력 SO 파일 경로 값
    private bool createDatabase = true;                                         //데이터 베이스 활용 여부 체크 값
    private ConversionType conversionType = ConversionType.Ingredients;
    private ConversionType prevConversionType;

    [MenuItem("Tools/JSON to Scriptable Objects")]
    public static void ShowWindow()
    {
        GetWindow<JsonToScriptableConverter>("JSON to Scriptable Objects");
    }

    void OnGUI()
    {
        GUILayout.Label("JSON to Scriptable object Converter", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if (GUILayout.Button("Select JSON File"))
        {
            jsonFilePath = EditorUtility.OpenFilePanel("Select JSON File", "", "json");
        }

        EditorGUILayout.LabelField("Selected File : ", jsonFilePath);
        EditorGUILayout.Space();

        // 변환 타입 선택
        conversionType = (ConversionType)EditorGUILayout.EnumPopup("Conversion Type : ", conversionType);

        // 타입이 바뀌었을 때만 폴더 변경
        if (conversionType != prevConversionType)
        {
            switch (conversionType)
            {
                case ConversionType.Ingredients:
                    outputFolder = "Assets/Data/Generated/Ingredients";
                    break;
                case ConversionType.CookedIngredients:
                    outputFolder = "Assets/Data/Generated/CookedIngredients";
                    break;
                case ConversionType.Dishes:
                    outputFolder = "Assets/Data/Generated/Dishes";
                    break;
                case ConversionType.Achievements:
                    outputFolder = "Assets/Data/Generated/Achievements";
                    break;
            }

            prevConversionType = conversionType;
        }

        outputFolder = EditorGUILayout.TextField("Output Folder : ", outputFolder);
        createDatabase = EditorGUILayout.Toggle("Create Databse Asset", createDatabase);
        EditorGUILayout.Space();

        if (GUILayout.Button("Convert to Scriptable Objects"))
        {
            if (string.IsNullOrEmpty(jsonFilePath))
            {
                EditorUtility.DisplayDialog("Error", "Pease Select a JSON file first", "OK");
                return;
            }

            switch (conversionType)
            {
                case ConversionType.Ingredients:
                    ConvertJsonToIngredientScriptableObjects();
                    break;
                case ConversionType.CookedIngredients:
                    ConvertJsonToCookedIngredientScriptableObjects();
                    break;
                case ConversionType.Dishes:
                    ConvertJsonToDishScriptableObjects();
                    break;
                case ConversionType.Achievements:
                    ConvertJsonToAchievementScriptableObjects();
                    break;
            }
        }
    }

    // 재료 데이터 JSON -> 재료SO 변환 함수
    private void ConvertJsonToIngredientScriptableObjects()
    {
        //폴더 생성
        if (!Directory.Exists(outputFolder))                                    //폴더 위치를 확인하고 없으면 생성한다. 
        {
            Directory.CreateDirectory(outputFolder);
        }

        //JSON 파일 읽기
        string jsonText = File.ReadAllText(jsonFilePath);                           //JSON 파일을 읽는다. 

        try
        {
            //JSON 파싱
            List<IngredientData> ingredientDataList = JsonConvert.DeserializeObject<List<IngredientData>>(jsonText);

            List<IngredientSO> createdIngredients = new List<IngredientSO>();                 //IngredientSO 리스트 생성

            //각 아이템을 데이터 스크립터블 오브젝트로 변환
            foreach (IngredientData ingredientData in ingredientDataList)
            {
                IngredientSO ingredientSO = ScriptableObject.CreateInstance<IngredientSO>();                              //ItemSO 파일을 생성

                //데이터 복사
                ingredientSO.id = ingredientData.id;
                ingredientSO.ingredientName = ingredientData.ingredientName;
                ingredientSO.nameEng = ingredientData.nameEng;
                ingredientSO.isCutable = ingredientData.isCutable;
                ingredientSO.isRawPlatable = ingredientData.isRawPlatable;

                //아이콘 로드 (경로가 있는 경우)
                if (!string.IsNullOrEmpty(ingredientData.iconPath))                       //아이콘 경로가 있는지 확인한다. 
                {
                    ingredientSO.icon = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Resources/{ingredientData.iconPath}.png");

                    if (ingredientSO.icon == null)
                    {
                        Debug.LogWarning($"재료 {ingredientData.nameEng} 의 아이콘을 찾을 수 없습니다. : {ingredientData.iconPath}");
                    }
                }

                ingredientSO.basicModel = LoadPrefab(ingredientData.modelPath);
                ingredientSO.cutModel = LoadPrefab(ingredientData.cutModelPath);
                ingredientSO.cookingModel = LoadPrefab(ingredientData.cookingModelPath);

                //스크립터블 오브젝트 저장 - ID를 4자리 숫자로 포맷팅
                string assetPath = $"{outputFolder}/Ingredient_{ingredientData.id.ToString("D4")}_{ingredientData.nameEng}.asset";
                AssetDatabase.CreateAsset(ingredientSO, assetPath);

                //이셋 이름 지정
                ingredientSO.name = $"Ingredient_{ingredientData.id.ToString("D4")} + {ingredientData.nameEng}";
                createdIngredients.Add(ingredientSO);

                EditorUtility.SetDirty(ingredientSO);
            }

            //데이터베이스
            if (createDatabase && createdIngredients.Count > 0)
            {
                IngredientDatabaseSO dataBase = ScriptableObject.CreateInstance<IngredientDatabaseSO>();            //생성
                dataBase.ingredients = createdIngredients;

                AssetDatabase.CreateAsset(dataBase, $"{outputFolder}/IngredientDatabase.asset");
                EditorUtility.SetDirty(dataBase);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Sucess", $"Created {createdIngredients.Count} scriptable objects!", "OK");
        }
        catch (System.Exception e)
        {
            EditorUtility.DisplayDialog("Error", $"Failed to Convert JSON : {e.Message}", "OK");
            Debug.LogError($"JSON 변환 오류 : {e}");
        }
    }

    // 1차 조리품 데이터 JSON -> 1차 조리품SO 변환 함수
    private void ConvertJsonToCookedIngredientScriptableObjects()
    {
        //폴더 생성
        if (!Directory.Exists(outputFolder))                                    //폴더 위치를 확인하고 없으면 생성한다. 
        {
            Directory.CreateDirectory(outputFolder);
        }

        //JSON 파일 읽기
        string jsonText = File.ReadAllText(jsonFilePath);                           //JSON 파일을 읽는다. 

        try
        {
            //JSON 파싱
            List<CookedIngredientData> cookedIngredientDataList = JsonConvert.DeserializeObject<List<CookedIngredientData>>(jsonText);

            List<CookedIngredientSO> createdCookedIngredients = new List<CookedIngredientSO>();                 //IngredientSO 리스트 생성

            //각 아이템을 데이터 스크립터블 오브젝트로 변환
            foreach (CookedIngredientData cookedIngredientData in cookedIngredientDataList)
            {
                CookedIngredientSO cookedIngredientSO = ScriptableObject.CreateInstance<CookedIngredientSO>();                              //ItemSO 파일을 생성

                //데이터 복사
                cookedIngredientSO.id = cookedIngredientData.id;
                cookedIngredientSO.cookedIngredientName = cookedIngredientData.cookedIngredientName;
                cookedIngredientSO.nameEng = cookedIngredientData.nameEng;

                int?[] ingredientIds = new int?[4];

                ingredientIds[0] = cookedIngredientData.ingredientOne;
                ingredientIds[1] = cookedIngredientData.ingredientTwo;
                ingredientIds[2] = cookedIngredientData.ingredientThree;
                ingredientIds[3] = cookedIngredientData.ingredientFour;

                cookedIngredientSO.ingredientIds = ingredientIds;

                //열거형 변환
                if (System.Enum.TryParse(cookedIngredientData.cookwareTypeString, out CookwareType parsedType))
                {
                    cookedIngredientSO.cookwareType = parsedType;
                }
                else
                {
                    Debug.LogWarning($"아이템 {cookedIngredientData.cookedIngredientName}의 유효하지 않은 타입 : {cookedIngredientData.cookwareTypeString}");
                }

                cookedIngredientSO.model = LoadPrefab(cookedIngredientData.modelPath);

                //스크립터블 오브젝트 저장 - ID를 4자리 숫자로 포맷팅
                string assetPath = $"{outputFolder}/CookedIngredient_{cookedIngredientData.id.ToString("D4")}_{cookedIngredientData.nameEng}.asset";
                AssetDatabase.CreateAsset(cookedIngredientSO, assetPath);

                //이셋 이름 지정
                cookedIngredientSO.name = $"CookedIngredient_{cookedIngredientData.id.ToString("D4")} + {cookedIngredientData.nameEng}";
                createdCookedIngredients.Add(cookedIngredientSO);

                EditorUtility.SetDirty(cookedIngredientSO);
            }

            //데이터베이스
            if (createDatabase && createdCookedIngredients.Count > 0)
            {
                CookedIngredientDatabaseSO dataBase = ScriptableObject.CreateInstance<CookedIngredientDatabaseSO>();            //생성
                dataBase.cookedIngredients = createdCookedIngredients;

                AssetDatabase.CreateAsset(dataBase, $"{outputFolder}/CookedIngredientDatabase.asset");
                EditorUtility.SetDirty(dataBase);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Sucess", $"Created {createdCookedIngredients.Count} scriptable objects!", "OK");
        }
        catch (System.Exception e)
        {
            EditorUtility.DisplayDialog("Error", $"Failed to Convert JSON : {e.Message}", "OK");
            Debug.LogError($"JSON 변환 오류 : {e}");
        }
    }

    // 요리 데이터 JSON -> 요리SO 변환 함수
    private void ConvertJsonToDishScriptableObjects()
    {
        //폴더 생성
        if (!Directory.Exists(outputFolder))                                    //폴더 위치를 확인하고 없으면 생성한다. 
        {
            Directory.CreateDirectory(outputFolder);
        }

        //JSON 파일 읽기
        string jsonText = File.ReadAllText(jsonFilePath);                           //JSON 파일을 읽는다. 

        try
        {
            //JSON 파싱
            List<DishData> dishDataList = JsonConvert.DeserializeObject<List<DishData>>(jsonText);

            List<DishSO> dishes = new List<DishSO>();                 //IngredientSO 리스트 생성

            //각 아이템을 데이터 스크립터블 오브젝트로 변환
            foreach (DishData dishData in dishDataList)
            {
                DishSO dishSO = ScriptableObject.CreateInstance<DishSO>();                              //ItemSO 파일을 생성

                //데이터 복사
                dishSO.id = dishData.id;
                dishSO.dishName = dishData.dishName;
                dishSO.nameEng = dishData.nameEng;

                int?[] ingredientIds = new int?[4];

                ingredientIds[0] = dishData.ingredientOne;
                ingredientIds[1] = dishData.ingredientTwo;
                ingredientIds[2] = dishData.ingredientThree;
                ingredientIds[3] = dishData.ingredientFour;

                dishSO.ingredientIds = ingredientIds;

                dishSO.score = dishData.score;

                //열거형 변환
                if (System.Enum.TryParse(dishData.cookwareTypeString, out CookwareType parsedType))
                {
                    dishSO.cookwareType = parsedType;
                }
                else
                {
                    Debug.LogWarning($"아이템 {dishData.dishName}의 유효하지 않은 타입 : {dishData.cookwareTypeString}");
                }

                //아이콘 로드 (경로가 있는 경우)
                if (!string.IsNullOrEmpty(dishData.iconPath))                       //아이콘 경로가 있는지 확인한다. 
                {
                    dishSO.icon = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Resources/{dishData.iconPath}.png");

                    if (dishSO.icon == null)
                    {
                        Debug.LogWarning($"재료 {dishData.nameEng} 의 아이콘을 찾을 수 없습니다. : {dishData.iconPath}");
                    }
                }

                dishSO.model = LoadPrefab(dishData.modelPath);

                //스크립터블 오브젝트 저장 - ID를 4자리 숫자로 포맷팅
                string assetPath = $"{outputFolder}/Dish_{dishData.id.ToString("D4")}_{dishData.nameEng}.asset";
                AssetDatabase.CreateAsset(dishSO, assetPath);

                //이셋 이름 지정
                dishSO.name = $"Dish_{dishData.id.ToString("D4")} + {dishData.nameEng}";
                dishes.Add(dishSO);

                EditorUtility.SetDirty(dishSO);
            }

            //데이터베이스
            if (createDatabase && dishes.Count > 0)
            {
                DishDatabaseSO dataBase = ScriptableObject.CreateInstance<DishDatabaseSO>();            //생성
                dataBase.dishes = dishes;

                AssetDatabase.CreateAsset(dataBase, $"{outputFolder}/DishDatabase.asset");
                EditorUtility.SetDirty(dataBase);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Sucess", $"Created {dishes.Count} scriptable objects!", "OK");
        }
        catch (System.Exception e)
        {
            EditorUtility.DisplayDialog("Error", $"Failed to Convert JSON : {e.Message}", "OK");
            Debug.LogError($"JSON 변환 오류 : {e}");
        }
    }

    // 업적 데이터 JSON -> 업적SO 변환 함수
    private void ConvertJsonToAchievementScriptableObjects()
    {
        //폴더 생성
        if (!Directory.Exists(outputFolder))                                    //폴더 위치를 확인하고 없으면 생성한다. 
        {
            Directory.CreateDirectory(outputFolder);
        }

        //JSON 파일 읽기
        string jsonText = File.ReadAllText(jsonFilePath);                           //JSON 파일을 읽는다. 

        try
        {
            //JSON 파싱
            List<AchievementData> achievementDataList = JsonConvert.DeserializeObject<List<AchievementData>>(jsonText);

            List<AchievementSO> createdAchievements = new List<AchievementSO>();                 //IngredientSO 리스트 생성

            //각 아이템을 데이터 스크립터블 오브젝트로 변환
            foreach (AchievementData achievementData in achievementDataList)
            {
                AchievementSO achievementSO = ScriptableObject.CreateInstance<AchievementSO>();                              //ItemSO 파일을 생성

                //데이터 복사
                achievementSO.id = achievementData.id;
                achievementSO.achievementName = achievementData.achievementName;
                achievementSO.nameEng = achievementData.nameEng;
                achievementSO.description = achievementData.description;
                achievementSO.goal = achievementData.goal;

                //열거형 변환
                if (System.Enum.TryParse(achievementData.achievementTypeString, out AchievementType parsedType))
                {
                    achievementSO.achievementType = parsedType;
                }
                else
                {
                    Debug.LogWarning($"아이템 {achievementData.achievementName}의 유효하지 않은 타입 : {achievementData.achievementTypeString}");
                }

                //아이콘 로드 (경로가 있는 경우)
                if (!string.IsNullOrEmpty(achievementData.iconPath))                       //아이콘 경로가 있는지 확인한다. 
                {
                    achievementSO.icon = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Resources/{achievementData.iconPath}.png");

                    if (achievementSO.icon == null)
                    {
                        Debug.LogWarning($"재료 {achievementData.nameEng} 의 아이콘을 찾을 수 없습니다. : {achievementData.iconPath}");
                    }
                }

                //스크립터블 오브젝트 저장 - ID를 4자리 숫자로 포맷팅
                string assetPath = $"{outputFolder}/Achievement_{achievementData.id.ToString("D4")}_{achievementData.nameEng}.asset";
                AssetDatabase.CreateAsset(achievementSO, assetPath);

                //이셋 이름 지정
                achievementSO.name = $"Item_{achievementData.id.ToString("D4")} + {achievementData.nameEng}";
                createdAchievements.Add(achievementSO);

                EditorUtility.SetDirty(achievementSO);
            }

            //데이터베이스
            if (createDatabase && createdAchievements.Count > 0)
            {
                AchievementDatabaseSO dataBase = ScriptableObject.CreateInstance<AchievementDatabaseSO>();            //생성
                dataBase.achievements = createdAchievements;

                AssetDatabase.CreateAsset(dataBase, $"{outputFolder}/AchievementDatabase.asset");
                EditorUtility.SetDirty(dataBase);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Sucess", $"Created {createdAchievements.Count} scriptable objects!", "OK");
        }
        catch (System.Exception e)
        {
            EditorUtility.DisplayDialog("Error", $"Failed to Convert JSON : {e.Message}", "OK");
            Debug.LogError($"JSON 변환 오류 : {e}");
        }
    }

    // 경로를 통해 프리팹 오브젝트 받아오는 함수
    GameObject LoadPrefab(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;

        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/Resources/{path}.prefab");

        if (prefab == null)
        {
            Debug.LogWarning($"모델을 찾을 수 없습니다: {path}");
            return null;
        }

        return prefab;
    }
}

#endif