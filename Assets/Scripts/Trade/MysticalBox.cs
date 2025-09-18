using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Garden;
using UnityEngine.UI;
using TMPro;

public class MysticalBox : MonoBehaviour, IInteractable
{
    [System.Serializable]
    public class CropValue
    {
        public SeedBag.PlantType plantType;
        public int value;
    }

    [Header("Торговля")]
    public List<CropValue> cropValues;
    public string targetTag = "Collectable";

    [Header("Статистика")]
    public Dictionary<SeedBag.PlantType, int> soldCrops = new Dictionary<SeedBag.PlantType, int>();
    public int totalEarnings = 0;

    [Header("UI")]
    public StatsDisplayUI statsDisplayUI;

    private Material originalMaterial;
    private MeshRenderer meshRenderer;
    private bool isInteracting = false;

    private void Awake()
    {
        foreach (SeedBag.PlantType plantType in System.Enum.GetValues(typeof(SeedBag.PlantType)))
        {
            soldCrops[plantType] = 0;
        }
        
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            originalMaterial = meshRenderer.material;
        }
        
        if (statsDisplayUI != null)
        {
            statsDisplayUI.Hide();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;

        if (other.CompareTag(targetTag))
        {
            Fruit fruit = other.GetComponent<Fruit>();
            if (fruit != null)
            {
                int moneyToAdd = GetValueForPlantType(fruit.plantType);
                if (moneyToAdd > 0)
                {
                    PlayerMoney.Instance.AddMoney(moneyToAdd);
                    totalEarnings += moneyToAdd;
                    
                    soldCrops[fruit.plantType]++;
                }
                Destroy(other.gameObject);
                return;
            }

            PickableItem pickableItem = other.GetComponent<PickableItem>();
            if (pickableItem != null && pickableItem is SeedBag seedBag)
            {
                int moneyToAdd = GetValueForPlantType(seedBag.plantType);
                if (moneyToAdd > 0)
                {
                    PlayerMoney.Instance.AddMoney(moneyToAdd);
                    totalEarnings += moneyToAdd;
                }
                Destroy(other.gameObject);
            }
        }
    }

    private int GetValueForPlantType(SeedBag.PlantType plantType)
    {
        foreach (var cropValue in cropValues)
        {
            if (cropValue.plantType == plantType)
            {
                return cropValue.value;
            }
        }
        return 0;
    }
    
    public void Interact()
    {
        if (isInteracting)
            return;
            
        isInteracting = true;
        
        if (statsDisplayUI != null)
        {
            if (!statsDisplayUI.IsOpen())
            {
                statsDisplayUI.Show(soldCrops, totalEarnings);
            }
            else
            {
                statsDisplayUI.Hide();
            }
        }
        else
        {
            statsDisplayUI = FindObjectOfType<StatsDisplayUI>();
            if (statsDisplayUI != null)
            {
                statsDisplayUI.Show(soldCrops, totalEarnings);
            }
        }
        
        Invoke("ResetInteractionFlag", 0.1f);
    }
    
    private void ResetInteractionFlag()
    {
        isInteracting = false;
    }
    
    public string GetInteractionHint()
    {
        if (statsDisplayUI != null && statsDisplayUI.IsOpen())
        {
            return "Нажмите E, чтобы закрыть статистику";
        }
        return "Нажмите E, чтобы посмотреть статистику";
    }
    
    private void Update()
    {

    }

    private void OnValidate()
    {
        if (GetComponent<Collider>() == null)
        {
        }
    }
    
    private void OnEnable()
    {
        if (statsDisplayUI == null)
        {
        }
    }
    
    public void TestOpenCanvas()
    {
        Interact();
    }
}