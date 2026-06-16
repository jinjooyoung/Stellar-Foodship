using UnityEngine;

public class InteractionFinder : MonoBehaviour
{
    public Transform targetTransform;       // 정면 기준 빈 오브젝트
    public Transform playerTransform;       // 탐색 범위 원점
    public float radius = 3f;

    // 탐색할 레이어
    private int pickableLayer = 6;
    private int nonPickableLayer = 7;
    private int interactLayer = -1;

    private void Awake()
    {
        targetTransform.position = playerTransform.position + playerTransform.forward * radius + new Vector3(0,-1f,0);
        interactLayer = (1 << pickableLayer) | (1 << nonPickableLayer);
    }

    // 플레이어 원점 기준 원형 범위 탐색 후 타겟 Transform과 제일 가까운 IInteractable 반환
    public IInteractable FindClosestInteractable()
    {
        Collider[] hits = Physics.OverlapSphere(playerTransform.position, radius, interactLayer);
        Player player = playerTransform.GetComponent<Player>();
        Transform closest = null;
        float minDist = float.MaxValue;

        foreach (var hit in hits)
        {
            // 인터페이스 없는 애들은 제외
            if (!hit.TryGetComponent<IInteractable>(out var interactable))
                continue;
            
            // 들고 있는 아이템 제외
            if (interactable is Pickable pickable)
            {
                if (pickable == player.heldItem)
                    continue;
            }

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
}
