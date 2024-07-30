using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaSliderController : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public void UpdateValue(float value)
    {
        slider.value = value;
    }
}
