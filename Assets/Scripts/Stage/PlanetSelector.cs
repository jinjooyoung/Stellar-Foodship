using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // <-- 추가: 씬 전환 기능을 위해 필요합니다.

public class PlanetSelector : MonoBehaviour
{
    [Header("[참조 설정]")]
    public Planet currentMainPlanet;
    public Transform mainSpot;

    [Header("[UI 설정]")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI difficultyText;
    public TextMeshProUGUI infoText;

    [Header("[멀티 추가]")]
    private StageSelectData stageSelectData;
    private FusionBootstrap bootstrap;

    void Awake()
    {
        Time.timeScale = 1f;
    }

    void Start()
    {
        stageSelectData =
        FindFirstObjectByType<StageSelectData>();

        bootstrap =
            FindFirstObjectByType<FusionBootstrap>();

        if (currentMainPlanet != null)
        {
            UpdatePlanetUI(currentMainPlanet);
        }
    }

    void Update()
    {
        if (bootstrap == null)
            return;

        if (!bootstrap.Runner.IsServer)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray =
                Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Planet clickedPlanet =
                    hit.collider.GetComponent<Planet>();

                if (clickedPlanet == null)
                    return;

                if (!clickedPlanet.isMain)
                {
                    SwapPlanets(clickedPlanet);
                }

                UpdatePlanetUI(clickedPlanet);

                if (stageSelectData != null)
                {
                    stageSelectData.RPC_SetPlanet(
                        clickedPlanet.info.stageNumber
                    );
                }
            }
        }
    }

    private int lastPlanet = -1;

    private void LateUpdate()
    {
        if (stageSelectData == null)
            return;

        if (lastPlanet == stageSelectData.SelectedPlanet)
            return;

        lastPlanet = stageSelectData.SelectedPlanet;

        Planet[] planets =
            FindObjectsByType<Planet>(FindObjectsSortMode.None);

        foreach (Planet p in planets)
        {
            if (p.info.stageNumber == lastPlanet)
            {
                if (!p.isMain)
                {
                    SwapPlanets(p);
                }

                UpdatePlanetUI(p);
                break;
            }
        }
    }

    void SwapPlanets(Planet newMain)
    {
        Vector3 oldMainSubPos = currentMainPlanet.subPosition;
        currentMainPlanet.MoveToSub(oldMainSubPos);
                            
        newMain.MoveToMain(mainSpot.position);
        currentMainPlanet = newMain;
    }

    void UpdatePlanetUI(Planet planet)
    {
        if (nameText != null) nameText.text = planet.info.planetName;
        if (infoText != null) infoText.text = planet.info.description;

        if (difficultyText != null)
        {
            string stars = "난이도: ";
            for (int i = 0; i < 5; i++)
            {
                stars += (i < planet.info.difficulty) ? "★" : "☆";
            }
            difficultyText.text = stars;
        }
    }

    // --- 추가된 부분: 버튼을 누르면 호출할 함수 ---
    public void StartStage()
    {
        if (bootstrap == null)
            return;

        if (!bootstrap.Runner.IsServer)
            return;

        bootstrap.StartGameScene();
    }
}