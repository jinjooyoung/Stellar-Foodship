using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Setup")]
    public Transform target; // 쫓아갈 대상 (우주선)

    [Header("Camera Settings")]
    // 우주선과 카메라 사이의 기본 거리 (우주선을 화면 중앙에 두기 위한 값)
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    // 따라가는 부드러움 정도 (높을수록 찰싹 달라붙습니다)
    public float smoothSpeed = 10f;

    void LateUpdate()
    {
        // 타겟이 없으면 멈춤
        if (target == null) return;

        // 1. 목표 위치 = 우주선의 현재 위치(몸체 피봇) + 떨어질 거리(오프셋)
        Vector3 desiredPosition = target.position + offset;

        // 2. 현재 카메라 위치에서 목표 위치로 부드럽게 이동
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}