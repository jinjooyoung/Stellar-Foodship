using UnityEngine;
using System.Collections;

public class TeleportPad : MonoBehaviour
{
    [Header("[이동 설정]")]
    public Transform arrivalSpot;

    [Header("[타이머 설정]")]
    public float reuseCooldown = 3f;

    private bool isWorking = false;

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();

        if (player != null && !isWorking && arrivalSpot != null)
        {
            StartCoroutine(Co_TeleportSequence(player));
        }
    }

    private IEnumerator Co_TeleportSequence(Player player)
    {
        isWorking = true;

        // 이동 직전 조작 불가능 상태로 변경 (안전장치)
        player.state = PlayerState.Uncontrollable;

        // [순간이동] 목적지로 위치를 즉시 변경
        player.transform.position = arrivalSpot.position;

        // 도착 후 다시 조작 가능 상태로 복구
        player.state = PlayerState.Controllable;

        // 재사용 대기 시간만큼 기다림
        yield return new WaitForSeconds(reuseCooldown);
        isWorking = false;
    }
}