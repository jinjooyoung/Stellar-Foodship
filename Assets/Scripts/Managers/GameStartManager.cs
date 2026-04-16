using UnityEngine;
using TMPro;
using System.Collections;

public class GameStartManager : MonoBehaviour
{
    public TextMeshProUGUI countdownText;
    public Player playerScript; // Player_1이 아니라 Player로 수정!

    // 만약 타이머 스크립트가 따로 있다면 여기에 추가 (예: ScoreManager)
    // public ScoreManager stageTimer; 

    void Start()
    {
        // 1. 시작하자마자 플레이어를 못 움직이게 설정
        // 인스펙터에 있는 State를 'Wait' 등으로 바꿔야 합니다.
        // (Player 스크립트에 정의된 Enum 이름을 확인해야 함)
        // playerScript.State = PlayerState.Wait; 

        StartCoroutine(StartSequence());
    }

    IEnumerator StartSequence()
    {
        float timer = 3f;
        while (timer > 0)
        {
            countdownText.text = Mathf.Ceil(timer).ToString();
            yield return null;
            timer -= Time.deltaTime;
        }

        countdownText.text = "GO!";

        // 2. [핵심] 기획서 내용대로 상태를 Controllable로 변경!
        // 인스펙터에 보이는 'State' 변수를 직접 바꿔줍니다.
        // playerScript.State = Player.PlayerState.Controllable; 

        // 만약 위 코드가 복잡하다면 단순히 스크립트를 껐다 켜도 됩니다.
        playerScript.enabled = true;

        yield return new WaitForSeconds(1f);
        countdownText.gameObject.SetActive(false);
    }
}