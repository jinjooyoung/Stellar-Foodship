using UnityEngine;
using UnityEngine.UI;

public class OxygenSliderColorChanger : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image fillImage;

    private readonly Color color100 = new(0.156f, 0.737f, 1f);      // 28BCFF
    private readonly Color color70 = new(0.486f, 0.945f, 0.545f);   // 7CF18B
    private readonly Color color50 = new(1f, 0.969f, 0.290f);       // FFF74A
    private readonly Color color30 = new(1f, 0.447f, 0.204f);       // FF7234
    private readonly Color color10 = new(0.953f, 0.243f, 0.157f);   // F33E28

    private Color currentColor;

    void Update()
    {
        float percent = slider.value / slider.maxValue;

        Color targetColor;

        if (percent >= 0.7f)
            targetColor = color100;
        else if (percent >= 0.5f)
            targetColor = color70;
        else if (percent >= 0.3f)
            targetColor = color50;
        else if (percent >= 0.1f)
            targetColor = color30;
        else
            targetColor = color10;

        if (targetColor == currentColor)
            return;

        currentColor = targetColor;
        fillImage.color = targetColor;
    }
}
