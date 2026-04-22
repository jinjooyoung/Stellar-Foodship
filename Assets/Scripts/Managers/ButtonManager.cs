using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    [Header("패널 설정")]
    [SerializeField] private GameObject mainCanvas;   // 메인 버튼들이 있는 Canvas
    [SerializeField] private GameObject volumeCanvas; // 슬라이더들이 있는 Canvas (1)

    private void Start()
    {
        // 시작할 때 메인 메뉴는 보이고, 볼륨창만 숨깁니다.
        if (mainCanvas != null) mainCanvas.SetActive(true);
        if (volumeCanvas != null) volumeCanvas.SetActive(false);
    }

    // 설정창 열기 (메인 메뉴를 끄고 볼륨창을 켬)
    public void OpenVolumeSettings()
    {
        if (mainCanvas != null && volumeCanvas != null)
        {
            mainCanvas.SetActive(false);
            volumeCanvas.SetActive(true);
        }
    }

    // 설정창 닫기 (볼륨창을 끄고 메인 메뉴를 다시 켬)
    public void CloseVolumeSettings()
    {
        if (mainCanvas != null && volumeCanvas != null)
        {
            volumeCanvas.SetActive(false);
            mainCanvas.SetActive(true);
        }
    }

    public void StageButton() => SceneManager.LoadScene("StageScene");
    public void ExitButton() => Application.Quit();
}