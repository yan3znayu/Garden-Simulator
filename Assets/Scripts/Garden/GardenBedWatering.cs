using UnityEngine;

public class GardenBedWatering : MonoBehaviour
{
    public float waterLossRate = 0.5f;
    public float maxWaterLevel = 50f;
    private float currentWaterLevel = 0f;
    private float dryTimeCounter = 0f;

    void Update()
    {
        if (currentWaterLevel > 0f)
        {
            currentWaterLevel -= waterLossRate * Time.deltaTime;
            currentWaterLevel = Mathf.Max(0f, currentWaterLevel);
            dryTimeCounter = 0f;
        }
        else
        {
            dryTimeCounter += Time.deltaTime;
        }
    }

    public void Water(float amount)
    {
        currentWaterLevel = Mathf.Min(maxWaterLevel, currentWaterLevel + amount);
    }

    public bool IsWatered() => currentWaterLevel > 0f;
    public float GetCurrentWaterLevel() => currentWaterLevel;
    public float GetMaxWaterLevel() => maxWaterLevel;
    public float GetDryTimeCounter() => dryTimeCounter;
}