using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaController : MonoBehaviour
{
    [SerializeField] private StaminaSliderController staminaSliderController;

    private const float MaxStaminaConst = 100;

    private float maxStamina = MaxStaminaConst;
    private float currentStamina = MaxStaminaConst;

    private float jumpCosts = 20;
    private float crouchCosts = 20;
    private float hitCosts = 20;
    private float shiftCosts = 50;

    private float recoverySpeed = 10;

    private float recoveryDelay = 0.05f;
    private float recoveryFromZeroDelay = 2f;

    private void Start()
    {
        UpdateSlider();
    }

    private bool HasEnoughStamina(float _)
    {
        return currentStamina >= 0;
    }

    private void SpendStamina(float costs)
    {
        if (costs > currentStamina)
        {
            currentStamina = -recoverySpeed * recoveryFromZeroDelay;
        } else
        {
            currentStamina -= costs;
        }
        UpdateSlider();
    }

    private void UpdateSlider()
    {
        staminaSliderController.UpdateValue(GetStaminaRatio());
    }

    private float GetStaminaRatio()
    {
        return Mathf.Clamp01(currentStamina / maxStamina);
    }

    private bool TrySpendStamina(float costs)
    {
        if (HasEnoughStamina(costs))
        {
            SpendStamina(costs);
            return true;
        }
        return false;
    }

    public bool TrySpendJumpCosts() => TrySpendStamina(jumpCosts);
    public bool TrySpendCrouchCosts() => TrySpendStamina(crouchCosts);
    public bool TrySpendHitCosts() => TrySpendStamina(hitCosts);
    public bool TrySpendShiftCosts() => TrySpendStamina(shiftCosts);

    private void Update()
    {
        var oldStamina = currentStamina;
        currentStamina += recoverySpeed * Time.deltaTime;
        if (currentStamina > maxStamina)
        {
            currentStamina = maxStamina;
        }
        if (currentStamina != oldStamina)
        {
            UpdateSlider();
        }
    }
}
