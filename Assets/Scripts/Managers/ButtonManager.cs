using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public void StageButton()

    {
        SceneManager.LoadScene("StageScene");
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}
