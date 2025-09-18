using UnityEngine;

public class SeedBag : PickableItem 
{
    public enum PlantType
    {
        PepperRed,
        PepperGreen,
        PepperYellow,
        Tomato,
        Cabage,
        Potato,
        Eggplant
    }

    public PlantType plantType; 
    public string seedName;   
    public int usesRemaining = 5;

    void Start()
    {
        switch (plantType)
        {
            case PlantType.PepperRed:
                seedName = "Перец Красный";
                break;
            case PlantType.PepperGreen:
                seedName = "Перец Зеленый";
                break;
            case PlantType.PepperYellow:
                seedName = "Перец Желтый";
                break;
            case PlantType.Tomato:
                seedName = "Помидор";
                break;
            case PlantType.Eggplant:
                seedName = "Баклажан";
                break;
            case PlantType.Potato:
                seedName = "Картошка";
                break;
            case PlantType.Cabage:
                seedName = "Капуста";
                break;
            default:
                seedName = "Неизвестные семена";
                break;
        }
        itemName = seedName;
    }

    public bool CanUse()
    {
        return usesRemaining > 0;
    }

    public void Use()
    {
        usesRemaining--;
        if (usesRemaining <= 0)
        {
            Destroy(gameObject);
        }
    }
    public PlantType GetPlantType()
    {
        return plantType;
    }
}