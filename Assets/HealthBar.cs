using System;

using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    public Slider slider;
    public Gradient sliderGradient;
    public Image fill;

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;

        fill.color = sliderGradient.Evaluate(1f);
    }
    public void SetHealth(int health)
    {
        slider.value = health;

        fill.color = sliderGradient.Evaluate(slider.normalizedValue);
    }

    internal int GetHealth()
    {
        return (int)slider.value;
    }
}
