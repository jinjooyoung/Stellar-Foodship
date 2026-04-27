using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    [Header("패널 설정")]
    [SerializeField] private GameObject mainCanvas;   // 메인 버튼들이 있는 Canvas
    [SerializeField] private GameObject volumeCanvas; // 슬라이더들이 있는 Canvas (1)
    public bool isOpen;     //VolumeCanvas가 열려있는지 여부

    private void Start()
    {
        // 시작할 때 메인 메뉴는 보이고, 볼륨창만 숨깁니다.
        if (mainCanvas != null) mainCanvas.SetActive(true);
        if (volumeCanvas != null) volumeCanvas.SetActive(false);
        isOpen = false;
    }

    // 설정창 열기 (메인 메뉴를 끄고 볼륨창을 켬)
    public void OpenVolumeSettings()
    {
        if (volumeCanvas != null & !isOpen)
        {
            volumeCanvas.SetActive(true);
            isOpen = true;
        }
        else if (volumeCanvas != null && isOpen)
        {
            volumeCanvas.SetActive(false);
            isOpen = false;
        }
    }

    public void StageButton() => SceneManager.LoadScene("StageScene");
    public void ExitButton() => Application.Quit();
}