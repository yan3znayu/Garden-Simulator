using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Garden;

public class PlantableBed : MonoBehaviour, IInteractable, IPlantable
{
    [System.Serializable]
    public class PlantStages
    {
        public SeedBag.PlantType plantType;
        public List<GameObject> stages;
        public GameObject harvestItemPrefab;
        public GameObject[] fruitPrefabs;
        public int minFruits = 2;
        public int maxFruits = 5;
    }

    public List<PlantStages> allPlantStages;
    public List<Transform> plantSlots;
    public List<float> growthTimes = new List<float>() { 60f, 60f };

    private GardenBedWatering bedWatering;
    private List<PlantGrowth> plantsInSlots;


    void Awake()
    {
        bedWatering = GetComponent<GardenBedWatering>() ?? gameObject.AddComponent<GardenBedWatering>();
        plantsInSlots = new List<PlantGrowth>(new PlantGrowth[plantSlots.Count]);
    }

    public string GetInteractionHint()
    {
        var interactor = FindObjectOfType<Interactor>();
        if (interactor.HeldItem == null) return "";

        if (interactor.HeldItem.TryGetComponent<SeedBag>(out var seedBag))
            return $"Нажмите [E] чтобы посадить {seedBag.seedName}";

        if (interactor.HeldItem.TryGetComponent<WaterContainer>(out _))
            return "Нажмите [E] чтобы полить грядку";

        return "";
    }

    public void Interact()
    {
        var interactor = FindObjectOfType<Interactor>();
        var heldItem = interactor.HeldItem;

        if (heldItem == null) return;

        if (heldItem.TryGetComponent<SeedBag>(out var seedBag))
            PlantSeed(seedBag);
        else if (heldItem.TryGetComponent<WaterContainer>(out _))
            bedWatering.Water(bedWatering.GetMaxWaterLevel());
    }

    public void PlantSeed(SeedBag seedBag)
    {
        Transform availableSlot = GetAvailablePlantSlot();
        if (availableSlot != null)
        {
            List<GameObject> plantStages = GetPlantStages(seedBag.plantType);
            PlantStages plantData = GetPlantData(seedBag.plantType);

            if (plantStages != null && plantStages.Count > 0)
            {
                GameObject plantInstance = new GameObject("Plant");
                plantInstance.transform.SetParent(availableSlot);
                plantInstance.transform.localPosition = Vector3.zero;

                PlantGrowth growth = plantInstance.AddComponent<PlantGrowth>();
                growth.plantStages = plantStages;
                growth.growthTimes = growthTimes;
                growth.harvestItemPrefab = plantData.harvestItemPrefab;
                growth.fruitPrefabs = plantData.fruitPrefabs;
                growth.minFruits = plantData.minFruits;
                growth.maxFruits = plantData.maxFruits;

                seedBag.Use();
            }
        }
    }

    PlantStages GetPlantData(SeedBag.PlantType plantType)
    {
        foreach (var plant in allPlantStages)
            if (plant.plantType == plantType)
                return plant;
        return null;
    }

    Transform GetAvailablePlantSlot()
    {
        foreach (Transform slot in plantSlots)
        {
            if (slot.childCount == 0)
            {
                return slot;
            }
        }
        return null;
    }

    List<GameObject> GetPlantStages(SeedBag.PlantType plantType)
    {
        foreach (var plant in allPlantStages)
            if (plant.plantType == plantType)
                return plant.stages;
        return null;
    }

    public void ClearSlot(Transform slot)
    {
        int index = plantSlots.IndexOf(slot);
        if (index != -1) plantsInSlots[index] = null;
    }
}