using UnityEngine;

public class WaterContainer : MonoBehaviour
{
    [SerializeField] private float currentWaterAmount = 0f;
    [SerializeField] private float maxCapacity = 100f;

    public void SetWaterAmount(float amount)
    {
        currentWaterAmount = Mathf.Clamp(amount, 0f, maxCapacity);
    }

    public float GetWaterAmount()
    {
        return currentWaterAmount;
    }

    public float GetMaxCapacity()
    {
        return maxCapacity;
    }

    public string GetWaterLevelText()
    {
        int displayedAmount = Mathf.FloorToInt(currentWaterAmount);
        return $"Вода: {displayedAmount}/{maxCapacity}";
    }

    public void AddWater(float amount)
    {
        currentWaterAmount = Mathf.Clamp(currentWaterAmount + amount, 0f, maxCapacity);
    }

    public bool CanUseWater(float amount)
    {
        return currentWaterAmount >= amount;
    }

    public bool UseWater(float amount)
    {
        if (!CanUseWater(amount)) return false;

        currentWaterAmount -= amount;
        return true;
    }

    public bool IsFull()
    {
        return currentWaterAmount >= maxCapacity;
    }
}