using UnityEngine;

public class NewInteractionFinder : MonoBehaviour
{
    [Header("Reference")]
    public Transform targetTransform;
    public Transform playerTransform;

    [Header("Setting")]
    public float radius = 3f;

    // 탐색 레이어
    private int pickableLayer = 6;
    private int nonPickableLayer = 7;

    private int interactLayer;

    private void Awake()
    {
        interactLayer =
            (1 << pickableLayer) |
            (1 << nonPickableLayer);
    }

    private void Update()
    {
        if (targetTransform == null || playerTransform == null)
            return;

        targetTransform.position =
            playerTransform.position +
            playerTransform.forward * radius +
            Vector3.down;
    }

    public INewInteractable FindClosestInteractable()
    {
        Collider[] hits =
            Physics.OverlapSphere(
                playerTransform.position,
                radius,
                interactLayer);

        NewPlayer player =
            playerTransform.GetComponent<NewPlayer>();

        Transform closest = null;

        float minDist = float.MaxValue;

        foreach (Collider hit in hits)
        {
            if (!hit.TryGetComponent<INewInteractable>(out var interactable))
                continue;

            // 내가 들고있는 아이템 제외
            if (interactable is NewPickable pickable)
            {
                if (pickable == player.HeldItem)
                    continue;
            }

            float dist =
                (hit.transform.position -
                 targetTransform.position).sqrMagnitude;

            if (dist < minDist)
            {
                minDist = dist;
                closest = hit.transform;
            }
        }

        if (closest != null)
        {
            closest.TryGetComponent<INewInteractable>(
                out var interactable);

            return interactable;
        }

        return null;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (playerTransform == null)
            return;

        bool hasTarget =
            FindClosestInteractable() != null;

        Gizmos.color =
            hasTarget ? Color.green : Color.red;

        Gizmos.DrawWireSphere(
            playerTransform.position,
            radius);

        if (targetTransform != null)
        {
            Gizmos.color = Color.yellow;

            Gizmos.DrawSphere(
                targetTransform.position,
                0.2f);

            Gizmos.DrawLine(
                playerTransform.position,
                targetTransform.position);
        }
    }
#endif
}