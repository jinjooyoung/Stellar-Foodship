using UnityEngine;

public class FollowWorldUI : MonoBehaviour
{
    
    public Transform uiTargetTransform; // 접시 위치
    public Camera uiWorldCamera; // 메인 카메라
    public Vector3 uiOffset = new Vector3(0, 1.5f, 0);
    public bool isPlayerFollow = false;

    void Update()
    {
        Debug.Log(
    $"{name} target = {uiTargetTransform}");
        if (!OxygenUIManager.Instance.player0set || !OxygenUIManager.Instance.player1set)
            return;

        if (uiTargetTransform == null)
        {
            Destroy(gameObject);
            return;
        }

        // 2. 카메라 확인 및 할당
        if (uiWorldCamera == null) uiWorldCamera = Camera.main;

        // 3. 위치 갱신 (Screen 좌표로 변환)
        if (uiWorldCamera != null)
        {
            transform.position = uiWorldCamera.WorldToScreenPoint(uiTargetTransform.position + uiOffset);
        }
    }
}