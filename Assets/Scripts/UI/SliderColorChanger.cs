using UnityEngine;
using UnityEngine.UI;

public class SliderColorChanger : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image fillImage;

    private void Start()
    {
        UpdateColor();
    }

    private void Update()
    {
        UpdateColor();
    }

    private void UpdateColor()
    {
        float percent = slider.value / slider.maxValue;

        if (percent <= 0.15f)
        {
            fillImage.color = Color.red;
        }
        else if (percent <= 0.4f)
        {
            fillImage.color = new Color(1f, 0.5f, 0f); // ÁÖÈ²»ö
        }
        else if (percent <= 0.6f)
        {
            fillImage.color = Color.yellow;
        }
        else
        {
            fillImage.color = Color.green;
        }
    }
}