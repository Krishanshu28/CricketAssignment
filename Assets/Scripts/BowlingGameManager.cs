using UnityEngine;
using UnityEngine.UI;

public enum BowlingMode { Fast, Spin}
public class BowlingGameManager : MonoBehaviour
{
    [Header("Refrences")]
    public BowlingSystem bowler;
    public Slider slider;

    public BowlingMode currentMode = BowlingMode.Fast;

    [Header("Delivery Settings")]
    [Range(-100f, 100f)]
    public float delivery = 0f;

    [Header("Delivery Tuning")]
    public float sliderSpeed = 2f;
    public float maxBallSpeed = 25f;
    public float swingForce = 15f;
    public float spinForce = 8f;

    private bool isBowling = false;

    void Update()
    {
        if(!isBowling && slider != null)
        {
            float val = Mathf.PingPong(Time.time * sliderSpeed, 2f) -1f;
            slider.value = val;
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            CalculateAndThrow();
        }
    }

    public void SetFatMode()
    {
        currentMode = BowlingMode.Fast;
    }

    public void SetSpinMode()
    {
        currentMode = BowlingMode.Spin;
    }

    public void ToggleBowlerSide()
    {
        bowler.ToggleSideChange();
    }

    void CalculateAndThrow()
    {
        isBowling = true;

        float stopPosition = slider.value;
        float accuracy = 1f - Mathf.Abs(stopPosition);

        float finalSwing = 0f;
        float finalSpin = 0f;

        float normalizeAdjustment = delivery / 100f;

        if(currentMode == BowlingMode.Fast)
        {
            finalSwing = normalizeAdjustment * swingForce * accuracy;
        }
        else
        {
            finalSpin = normalizeAdjustment * spinForce * accuracy;
        }

        bowler.ThrowBall(maxBallSpeed, finalSwing, finalSpin);
    }

    public void OnResetClicked()
    {
        isBowling = false;
        bowler.ResetSystem();
    }
}
