using UnityEngine;
using System.Collections;

public class JumpPad : MonoBehaviour
{
    [Header("[이동 설정]")]
    public Transform arrivalSpot;
    public float jumpDuration = 1.5f; // 날아가는 시간 (너무 빠르면 '부웅' 느낌이 안 나요)
    public float jumpHeight = 7.0f;   // 위로 솟구치는 높이 (값 키우면 더 높이 뜸)
    public AnimationCurve jumpCurve = AnimationCurve.EaseInOut(0, 0, 1, 0);

    [Header("[타이머 설정]")]
    public float reuseCooldown = 3f;

    private bool isWorking = false;

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();

        if (player != null && !isWorking && arrivalSpot != null)
        {
            StartCoroutine(Co_JumpSequence(player));
        }
    }

    private IEnumerator Co_JumpSequence(Player player)
    {
        isWorking = true;
        player.state = PlayerState.Uncontrollable;

        // [근본 해결] 시작 순간에 좌표를 '값'으로 미리 저장
        // 이제 arrivalSpot 오브젝트가 중간에 사라져도 에러가 나지 않습니다.
        Vector3 startPos = player.transform.position;
        Vector3 endPos = arrivalSpot.position;

        float timer = 0f;

        while (timer < jumpDuration)
        {
            timer += Time.deltaTime;
            float percent = timer / jumpDuration;

            // 1. 수평 이동: 출발지에서 목적지까지 직선으로 슥 이동
            Vector3 currentPos = Vector3.Lerp(startPos, endPos, percent);

            // 2. 수직 이동: AnimationCurve에 따라 Y축 높이를 추가
            // percent가 0.5일 때 jumpHeight만큼 가장 높이 솟구치게 됩니다.
            float yOffset = jumpCurve.Evaluate(percent) * jumpHeight;
            currentPos.y += yOffset;

            // 3. 플레이어 위치 적용
            if (player != null)
            {
                player.transform.position = currentPos;

            }

            yield return null;
        }

        // [착지] 정확한 목적지 위치로 최종 고정
        if (player != null)
        {
            player.transform.position = endPos;
            player.state = PlayerState.Controllable;
        }

        // 재사용 대기
        yield return new WaitForSeconds(reuseCooldown);
        isWorking = false;
    }
}