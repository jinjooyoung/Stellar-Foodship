using JinJooYoung;
using UnityEngine;

public class ESCPanelOnOff : MonoBehaviour
{
    public UIPopup escPanel;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (escPanel.canvasGroup.alpha == 1f)
            {
                escPanel.Close();
            }
            else
            {
                escPanel.Open();
            }
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
