using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingSceneUI : MonoBehaviour
{
    [Header("UI References")]
    public Slider progressBar;
    public TextMeshProUGUI progressText;
    public TextMeshProUGUI tipText; // 게임 팁이 표시될 텍스트 UI

    [Header("Game Tips")]
    [TextArea(3, 5)] // 인스펙터에서 입력하기 편하게 공간 확보
    public string[] gameTips = {
        "스페이스 바를 눌러 대쉬를 할 수 있습니다.",
        "점프대를 활용해 넘어가보세요.",
        "산소구역을 잘 활용하세요.",
        "ESC를 눌러 소리를 줄이고 키울 수 있습니다.",
        "외계인들은 참 무섭군요. 그렇지 않나요?"
    };

    void Start()
    {
        // 로딩 씬이 시작될 때 팁 하나를 랜덤으로 선택
        ShowRandomTip();
    }

    void Update()
    {
        // 로딩 진행률 업데이트
        float progress = LoadingManager.instance.LoadingProgress;

        if (progressBar != null)
            progressBar.value = progress;

        if (progressText != null)
            progressText.text = $"로딩 중... {(progress * 100f):0}%";
    }

    void ShowRandomTip()
    {
        if (tipText != null && gameTips.Length > 0)
        {
            int randomIndex = Random.Range(0, gameTips.Length);
            tipText.text = "TIP: " + gameTips[randomIndex];
        }
    }
}