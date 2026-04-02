using UnityEngine;

public class FollowWorldUI : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public Camera cam;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 screenPos = cam.WorldToScreenPoint(target.position + offset);
        transform.position = screenPos;
    }
}
