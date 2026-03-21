using UnityEngine;

public class InteractionFinder : MonoBehaviour
{
    public Transform targetTransform;       // 정면 기준 빈 오브젝트
    public Transform playerTransform;       // 탐색 범위 원점
    public float radius = 3f;
    public LayerMask interactLayer;

    private void Awake()
    {
        targetTransform.position = playerTransform.position + playerTransform.forward * radius;
    }

    // 플레이어 원점 기준 원형 범위 탐색 후 타겟 Transform과 제일 가까운 IInteractable 반환
    public IInteractable FindClosestInteractable()
    {
        Collider[] hits = Physics.OverlapSphere(playerTransform.position, radius, interactLayer);

        Transform closest = null;
        float minDist = float.MaxValue;

        foreach (var hit in hits)
        {
            // 인터페이스 없는 애들은 제외
            if (!hit.TryGetComponent<IInteractable>(out var interactable))
                continue;

            float dist = (hit.transform.position - targetTransform.position).sqrMagnitude;

            if (dist < minDist)
            {
                minDist = dist;
                closest = hit.transform;
            }
        }

        if (closest != null)
        {
            closest.TryGetComponent<IInteractable>(out var interactable);
            return interactable;
        }

        return null;
    }

    // 인식 범위, 기준점 인식 기즈모
    private void OnDrawGizmos()
    {
        if (playerTransform == null) return;

        bool hasInteractable = false;

        if (FindClosestInteractable() != null)      // 인식된 인터랙터블 클래스가 있는지
        {
            hasInteractable = true;
        }
        else
        {
            hasInteractable = false;
        }

        // 색상 설정
        Gizmos.color = hasInteractable ? Color.green : Color.red;

        /*// 반투명 채우기
        Color fillColor = Gizmos.color;
        fillColor.a = 0.2f;
        Gizmos.color = fillColor;
        Gizmos.DrawSphere(playerTransform.position, radius);*/

        // 테두리
        Gizmos.color = hasInteractable ? Color.green : Color.red;
        Gizmos.DrawWireSphere(playerTransform.position, radius);

        // targetTransform 표시 (노란색)
        if (targetTransform != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(targetTransform.position, 0.2f);

            // 방향선 (선택사항, 있으면 더 좋음)
            Gizmos.DrawLine(playerTransform.position, targetTransform.position);
        }
    }
}
