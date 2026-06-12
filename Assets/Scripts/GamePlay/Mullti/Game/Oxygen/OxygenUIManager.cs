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

    void Awake()
    {
        Instance = this;
    }


    public void RegisterPlayer(NewPlayer player, int index)
    {
        if (index == 0)
        {
            player0 = player;

            FollowWorldUI follow = player0Slider.gameObject.GetComponent<FollowWorldUI>();
            follow.uiTargetTransform = player0.gameObject.transform;

            player0set = true;
        }
        else
        {
            player1 = player;

            FollowWorldUI follow = player1Slider.gameObject.GetComponent<FollowWorldUI>();
            follow.uiTargetTransform = player1.gameObject.transform;

            player1set = true;
        }
    }

    void Update()
    {
        if (player0 != null && player0set)
        {
            player0Slider.value = player0.Oxygen;

            player0Slider.gameObject.SetActive(!player0.isDead);
        }

        if (player1 != null && player1set)
        {
            player1Slider.value = player1.Oxygen;

            player1Slider.gameObject.SetActive(!player1.isDead);
        }
    }
}