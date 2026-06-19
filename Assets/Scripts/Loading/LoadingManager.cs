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

    public void TriggerLoadScene(string sceneName)
    {
        Debug.Log($"[SceneManager] 로딩 요청 들어옴. 현재 isLoading 상태: {isLoading}");

        if (isLoading) return;

        // 혹시 모를 메인 스레드 병목을 방지하기 위해 청소 한 번 실행
        System.GC.Collect();

        // 반드시 매니저 자신의 인스턴스로 코루틴 실행 보장
        instance.StartCoroutine(instance.LoadSceneWithLoading(sceneName));
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

        if (useLoadingScreen && !string.IsNullOrEmpty(loadingSceneName))
        {
            Debug.Log("[Scene] 1. 로딩 화면 씬 로드 시작");
            yield return StartCoroutine(LoadSceneAsync(loadingSceneName));

            yield return new WaitForSecondsRealtime(0.2f);

          
            float startRealTime = Time.realtimeSinceStartup;

            Debug.Log($"[Scene] 2. 실제 목적지 씬 비동기 로드 시작 -> {sceneName}");
            AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);

            if (asyncLoad == null) { isLoading = false; yield break; }

            asyncLoad.allowSceneActivation = false;

            while (asyncLoad.progress < 0.9f)
            {
                LoadingProgress = asyncLoad.progress;
                yield return null;
            }

            Debug.Log("[Scene] 3. 실제 씬 데이터 로드 완료 (90%), 최소 로딩 시간 대기 시작");
            LoadingProgress = 1f;

            while (Time.realtimeSinceStartup - startRealTime < minimumLoadingTime)
            {
                yield return null;
            }

            Debug.Log("[Scene] 4. 문 열기 (allowSceneActivation = true)");
            asyncLoad.allowSceneActivation = true;

            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
        else
        {
            yield return StartCoroutine(LoadSceneAsync(sceneName));
        }

        Debug.Log("[Scene] 5. 모든 로딩 프로세스 종료. isLoading = false 플래그 리셋");
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