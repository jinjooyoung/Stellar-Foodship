using TMPro;
using UnityEngine;
using System.Collections;

public class GameStartManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI countdownText;

    private void Awake()
    {
        StartCoroutine(StartSequence());
    }

    IEnumerator StartSequence()
    {
        float timer = 3f;

        while (timer > 0)
        {
            countdownText.text =
                Mathf.Ceil(timer).ToString();

            yield return null;

            timer -= Time.deltaTime;
        }

        countdownText.text = "GO!";

        EnablePlayers();

        yield return new WaitForSeconds(1f);

        countdownText.gameObject.SetActive(false);
    }

    void EnablePlayers()
    {
        NewPlayer[] players =
            FindObjectsByType<NewPlayer>(
                FindObjectsSortMode.None);

        foreach (var player in players)
        {
            player.State =
                PlayerState.Controllable;
        }
    }
}