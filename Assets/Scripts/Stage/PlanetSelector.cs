using UnityEngine;
using TMPro;

public class PlanetSelector : MonoBehaviour
{
    [Header("[참조 설정]")]
    public Planet currentMainPlanet;
    public Transform mainSpot;

    [Header("[UI 설정]")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI difficultyText;
    public TextMeshProUGUI infoText;

    [Header("[멀티]")]
    private FusionBootstrap bootstrap;

    private void Awake()
    {
        Time.timeScale = 1f;
    }

    private void Start()
    {
        bootstrap =
            FindFirstObjectByType<FusionBootstrap>();

        if (currentMainPlanet != null)
        {
            UpdatePlanetUI(currentMainPlanet);
        }
    }

    private void Update()
    {
        if (bootstrap == null)
            return;

        if (bootstrap.Runner == null)
            return;

        // 호스트만 조작 가능
        if (!bootstrap.Runner.IsServer)
            return;

        if (!Input.GetMouseButtonDown(0))
            return;

        Ray ray =
            Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit))
            return;

        Planet clickedPlanet =
            hit.collider.GetComponent<Planet>();

        if (clickedPlanet == null)
            return;

        if (!clickedPlanet.isMain)
        {
            SwapPlanets(clickedPlanet);
        }

        UpdatePlanetUI(clickedPlanet);

        // 선택된 스테이지 번호 저장
        bootstrap.SelectedStageNumber =
            clickedPlanet.info.stageNumber;
    }

    private void SwapPlanets(Planet newMain)
    {
        Vector3 oldMainSubPos =
            currentMainPlanet.subPosition;

        currentMainPlanet.MoveToSub(oldMainSubPos);

        newMain.MoveToMain(mainSpot.position);

        currentMainPlanet = newMain;
    }

    private void UpdatePlanetUI(Planet planet)
    {
        if (nameText != null)
            nameText.text = planet.info.planetName;

        if (infoText != null)
            infoText.text = planet.info.description;

        if (difficultyText != null)
        {
            string stars = "난이도: ";

            for (int i = 0; i < 5; i++)
            {
                stars +=
                    (i < planet.info.difficulty)
                    ? "★"
                    : "☆";
            }

            difficultyText.text = stars;
        }
    }

    public void StartStage()
    {
        if (bootstrap == null)
            return;

        if (bootstrap.Runner == null)
            return;

        if (!bootstrap.Runner.IsServer)
            return;

        bootstrap.StartGameScene();
    }
}