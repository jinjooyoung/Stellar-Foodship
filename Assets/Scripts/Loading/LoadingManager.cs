using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager instance { get; private set; }
    [Header("Scene Settings")]
    public string loadingSceneName = "Loading";

    [Header("Loading Settings")]
    public float minimumLoadingTime = 2f;
    public bool useLoadingScreen = true;
    public bool useFadeEffect = true;

    [Header("Fade Settings")]
    public float fadeSpeed = 1f;
    public Color fadeColor = Color.black;

    private string currentSceneName;
    private string targetSceneName;
    private bool isLoading = false;
    private CanvasGroup fadeCanvasGroup;
    private GameObject fadeObject;

    // 씬 로딩 진행률 추적
    public float LoadingProgress { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 로딩 화면을 사용한 씬 로딩
    /// </summary>
    public void LoadSceneWithLoadingScreen(string sceneName)
    {
        if (isLoading)
        {
            Debug.LogWarning("이미 씬 로딩 중입니다.");
            return;
        }

        targetSceneName = sceneName;
        StartCoroutine(LoadSceneWithLoading(sceneName));
    }

    /// <summary>
    /// 로딩 화면을 사용한 씬 로딩
    /// </summary>
    private IEnumerator LoadSceneWithLoading(string sceneName)
    {
        isLoading = true;

        // 로딩 화면 로드
        if (useLoadingScreen && !string.IsNullOrEmpty(loadingSceneName))
        {
            yield return StartCoroutine(LoadSceneAsync(loadingSceneName));

            // 최소 로딩 시간 대기
            float startTime = Time.time;

            // 실제 씬 로딩
            AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.allowSceneActivation = false;

            while (!asyncLoad.isDone)
            {
                LoadingProgress = asyncLoad.progress;

                // 로딩이 90% 완료되면 대기
                if (asyncLoad.progress >= 0.9f)
                {
                    LoadingProgress = 1f;

                    // 최소 로딩 시간 체크
                    if (Time.time - startTime >= minimumLoadingTime)
                    {
                        asyncLoad.allowSceneActivation = true;
                    }
                }

                yield return null;
            }
        }
        else
        {
            // 로딩 화면 없이 직접 로딩
            yield return StartCoroutine(LoadSceneAsync(sceneName));
        }

        isLoading = false;
    }

    /// <summary>
    /// 비동기 씬 로딩
    /// </summary>
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            LoadingProgress = asyncLoad.progress;
            yield return null;
        }

        LoadingProgress = 1f;
    }



    #region Fade Effects

    private IEnumerator FadeIn()
    {
        if (fadeCanvasGroup == null) yield break;

        fadeCanvasGroup.blocksRaycasts = true;

        while (fadeCanvasGroup.alpha < 1f)
        {
            fadeCanvasGroup.alpha += fadeSpeed * Time.unscaledDeltaTime;
            yield return null;
        }

        fadeCanvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOut()
    {
        if (fadeCanvasGroup == null) yield break;

        while (fadeCanvasGroup.alpha > 0f)
        {
            fadeCanvasGroup.alpha -= fadeSpeed * Time.unscaledDeltaTime;
            yield return null;
        }

        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.blocksRaycasts = false;
    }

    #endregion

    #region Scene Events

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentSceneName = scene.name;
        LoadingProgress = 0f;

        Debug.Log($"씬 로딩 완료: {scene.name}");
    }

    private void OnSceneUnloaded(Scene scene)
    {
        Debug.Log($"씬 언로드 완료: {scene.name}");
    }

    #endregion

    #region Utility Methods

    public string GetCurrentSceneName()
    {
        return currentSceneName;
    }

    public bool IsLoading()
    {
        return isLoading;
    }

    public bool IsSceneLoaded(string sceneName)
    {
        return currentSceneName == sceneName;
    }

    public void SetFadeColor(Color color)
    {
        fadeColor = color;

        if (fadeObject != null)
        {
            UnityEngine.UI.Image fadeImage = fadeObject.GetComponentInChildren<UnityEngine.UI.Image>();
            if (fadeImage != null)
            {
                fadeImage.color = color;
            }
        }
    }

    #endregion
}