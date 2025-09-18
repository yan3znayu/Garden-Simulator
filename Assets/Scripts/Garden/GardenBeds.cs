using UnityEngine;

public class GardenBed : MonoBehaviour, IInteractable
{
    [SerializeField] private float waterNeeded = 1f;
    [SerializeField] private Color dryColor = Color.yellow;
    [SerializeField] private Color wateredColor = Color.green;

    private Renderer bedRenderer;
    private GardenBedWatering bedWatering;

    void Awake()
    {
        bedRenderer = GetComponent<Renderer>();
        bedWatering = GetComponent<GardenBedWatering>() ?? gameObject.AddComponent<GardenBedWatering>();
        UpdateVisuals();
    }

    void Update() => UpdateVisuals();

    public void Interact()
    {
        var interactor = FindObjectOfType<Interactor>();
        if (interactor.HeldItem?.TryGetComponent<WaterContainer>(out var container) == true && container.UseWater(waterNeeded))
        {
            bedWatering.Water(waterNeeded);
        }
    }

    public string GetInteractionHint()
    {
        var interactor = FindObjectOfType<Interactor>();
        return interactor.HeldItem?.TryGetComponent<WaterContainer>(out _) == true
            ? "Нажмите [E] чтобы полить грядку"
            : "";
    }

    public string GetBedStatus() => bedWatering.IsWatered()
        ? $"Воды в почве: {Mathf.RoundToInt(bedWatering.GetCurrentWaterLevel() / bedWatering.GetMaxWaterLevel() * 100)}%"
        : $"Требуется полив!";

    private void UpdateVisuals()
    {
        if (bedRenderer == null) return;

        float healthPercent = Mathf.Clamp01(bedWatering.GetCurrentWaterLevel() / bedWatering.GetMaxWaterLevel());
        bedRenderer.material.color = bedWatering.IsWatered()
            ? Color.Lerp(dryColor, wateredColor, healthPercent)
            : dryColor;
    }
}