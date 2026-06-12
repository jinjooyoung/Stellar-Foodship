using UnityEngine;
using UnityEngine.UI;

public class OxygenUIManager : MonoBehaviour
{
    public static OxygenUIManager Instance;

    public Slider player0Slider;
    public Slider player1Slider;

    public bool player0set = false;
    public bool player1set = false;

    public NewPlayer player0;
    public NewPlayer player1;

    [Header("Start Delay")]
    [SerializeField]
    private float startDelay = 3f;

    //------------------------------------------------

    void Awake()
    {
        Instance = this;
    }

    //------------------------------------------------

    public void RegisterPlayer(NewPlayer player, int index)
    {
        Debug.Log(
    $"RegisterPlayer : {player.name} / {index}");
        if (index == 0)
        {
            player0 = player;

            FollowWorldUI follow =
                player0Slider.GetComponent<FollowWorldUI>();

            follow.uiTargetTransform =
                player0.transform;

            player0set = true;
        }
        else
        {
            player1 = player;

            FollowWorldUI follow =
                player1Slider.GetComponent<FollowWorldUI>();

            follow.uiTargetTransform =
                player1.transform;

            player1set = true;
        }
    }

    //------------------------------------------------

    public void UnregisterPlayer(int index)
    {
        if (index == 0)
        {
            player0 = null;
            player0set = false;
        }
        else
        {
            player1 = null;
            player1set = false;
        }
    }

    //------------------------------------------------

    void Update()
    {
        if (startDelay > 0f)
        {
            startDelay -= Time.deltaTime;

            if (startDelay <= 0f)
                startDelay = 0f;

            return;
        }

        if (player0 != null && player0set)
        {
            player0Slider.value =
                player0.Oxygen;

            player0Slider.gameObject.SetActive(
                !player0.isDead);
        }

        if (player1 != null && player1set)
        {
            player1Slider.value =
                player1.Oxygen;

            player1Slider.gameObject.SetActive(
                !player1.isDead);
        }
    }
}