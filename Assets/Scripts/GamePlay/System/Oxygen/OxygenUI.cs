using UnityEngine;
using UnityEngine.UI;

public class OxygenUI : MonoBehaviour
{
    public Player player;
    public Slider slider;

    void Start()
    {
        slider.maxValue = player.maxOxygen;
    }

    void Update()
    {
        slider.value = player.oxygen;
    }
}