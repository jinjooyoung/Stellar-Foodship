using UnityEngine;

public class PlanetSelector : MonoBehaviour
{
    public Planet currentMainPlanet; // 현재 빨간 박스에 있는 행성
    public Transform mainSpot;      // 빨간 박스의 위치(Transform)

    void Update()
    {
        // 마우스 클릭 감지
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Planet clickedPlanet = hit.collider.GetComponent<Planet>();

                // 클릭한 게 행성이고, 현재 메인이 아닌 경우에만 스왑
                if (clickedPlanet != null && !clickedPlanet.isMain)
                {
                    SwapPlanets(clickedPlanet);
                }
            }
        }
    }

    void SwapPlanets(Planet newMain)
    {
        // 1. 현재 메인 행성을 자신의 노란 박스(Sub Position)로 보냄
        Vector3 oldMainSubPos = currentMainPlanet.subPosition;
        currentMainPlanet.MoveToSub(oldMainSubPos);

        // 2. 클릭된 행성을 메인 자리(Main Spot)로 보냄
        newMain.MoveToMain(mainSpot.position);

        // 3. 매니저의 메인 행성 참조 변경
        currentMainPlanet = newMain;
    }
}