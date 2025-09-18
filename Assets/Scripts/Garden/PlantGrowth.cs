using UnityEngine;
using System.Collections.Generic;

public class PlantGrowth : MonoBehaviour
{
    public List<GameObject> plantStages;
    public List<float> growthTimes;
    public float waterPenaltyMultiplier = 2f;

    private int currentStage = 0;
    private float growthTimer = 0f;
    private GameObject currentPlantInstance;
    private GardenBedWatering bedWatering;
    private bool isDead = false;

    [Header("Harvest Settings")]
    public GameObject harvestItemPrefab;
    public GameObject[] fruitPrefabs;
    public int minFruits = 2;
    public int maxFruits = 5;
    public bool isReadyToHarvest = false;

    void Start()
    {
        bedWatering = GetComponentInParent<GardenBedWatering>();

        if (plantStages.Count > 0 && plantStages[0] != null)
        {
            SpawnCurrentStage();
        }
        else
        {
            Debug.LogError("Первая стадия растения не назначена!");
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (isDead || bedWatering == null) return;

        if (!bedWatering.IsWatered())
        {
            Die();
            return;
        }

        if (currentStage < plantStages.Count - 1)
        {
            growthTimer += Time.deltaTime;

            if (growthTimer >= growthTimes[currentStage])
            {
                growthTimer = 0f;
                currentStage++;
                UpdatePlantStage();
            }
        }
    }

    public PickableItem Harvest()
    {
        if (!isReadyToHarvest) return null;

        GameObject harvestItem = Instantiate(harvestItemPrefab);
        PickableItem pickable = harvestItem.GetComponent<PickableItem>();
        if (pickable == null)
        {
            pickable = harvestItem.AddComponent<PickableItem>();
        }

        int fruitCount = Random.Range(minFruits, maxFruits + 1);
        for (int i = 0; i < fruitCount; i++)
        {
            Vector3 spawnPos = transform.position + new Vector3(
                Random.Range(-0.5f, 0.5f),
                0,
                Random.Range(-0.5f, 0.5f)
            );

            GameObject fruit = Instantiate(
                fruitPrefabs[Random.Range(0, fruitPrefabs.Length)],
                spawnPos,
                Quaternion.identity
            );

            Rigidbody rb = fruit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.drag = 0.5f;
                rb.angularDrag = 0.2f;

                rb.AddForce(new Vector3(
                    Random.Range(-1f, 1f),
                    Random.Range(2f, 4f),
                    Random.Range(-1f, 1f)
                ), ForceMode.Impulse);

                rb.AddTorque(new Vector3(
                    Random.Range(-5f, 5f),
                    Random.Range(-5f, 5f),
                    Random.Range(-5f, 5f)
                ));
            }
        }

        var plantableBed = GetComponentInParent<PlantableBed>();
        if (plantableBed != null)
        {
            plantableBed.ClearSlot(transform.parent);
        }

        Destroy(gameObject);

        return pickable;
    }
    void SpawnCurrentStage()
    {
        if (currentPlantInstance != null)
        {
            Destroy(currentPlantInstance);
        }

        currentPlantInstance = Instantiate(plantStages[currentStage], transform);
        currentPlantInstance.transform.localPosition = Vector3.zero;
    }

    void UpdatePlantStage()
    {
        SpawnCurrentStage();

            if (currentStage == plantStages.Count - 1)
            {
                isReadyToHarvest = true;
            }

        }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (currentPlantInstance != null)
        {
            Destroy(currentPlantInstance);
        }

        var plantableBed = GetComponentInParent<PlantableBed>();
        if (plantableBed != null)
        {
            plantableBed.ClearSlot(transform.parent);
        }

        Destroy(gameObject);
    }
}