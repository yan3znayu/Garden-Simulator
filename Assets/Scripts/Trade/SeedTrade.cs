using UnityEngine;
using TMPro;

public class SeedTrade : MonoBehaviour, IInteractable
{
    public SeedBag.PlantType plantType;
    public GameObject seedPrefab;     
    public int seedPrice = 10;        
    public string seedName;           
    public bool canBuy = true;        

    void Start()
    {
        switch (plantType)
        {
            case SeedBag.PlantType.PepperRed:
                seedName = "Перец Красный";
                break;
            case SeedBag.PlantType.PepperGreen:
                seedName = "Перец Зеленый";
                break;
            case SeedBag.PlantType.PepperYellow:
                seedName = "Перец Желтый";
                break;
            case SeedBag.PlantType.Tomato:
                seedName = "Помидор";
                break;
            case SeedBag.PlantType.Eggplant:
                seedName = "Баклажан";
                break;
            case SeedBag.PlantType.Potato:
                seedName = "Картошка";
                break;
            case SeedBag.PlantType.Cabage:
                seedName = "Капуста";
                break;
            default:
                seedName = "Неизвестные семена";
                break;
        }
    }

    public void Interact()
    {
        if (!canBuy) return;

        if (PlayerMoney.Instance.TryRemoveMoney(seedPrice))
        {
          
            GameObject newSeedBag = Instantiate(seedPrefab, Vector3.zero, Quaternion.identity); 
            PickableItem pickable = newSeedBag.GetComponent<PickableItem>();
            FindObjectOfType<Interactor>().PickUpItem(pickable); 
        }
    }

    public string GetInteractionHint()
    {
        if (!canBuy)
        {
            return "Нет в наличии";
        }

        if (PlayerMoney.Instance.GetCurrentMoney() >= seedPrice)
        {
            return $"Нажмите [B], чтобы купить {seedName} ({seedPrice} Тёмуксов)";
        }
        else
        {
            return $"Недостаточно денег ({seedPrice} Тёмуксов)";
        }
    }
}