using UnityEngine;
using static SeedBag;

public class HarvestableItem : PickableItem
{
    public SeedBag.PlantType plantType;
    public int amount = 1;

    void Start()
    {
        switch (plantType)
        {
            case SeedBag.PlantType.PepperRed:
                itemName = "Перец Красный";
                break;
            case SeedBag.PlantType.PepperGreen:
                itemName = "Перец Зеленый";
                break;
            case SeedBag.PlantType.PepperYellow:
                itemName = "Перец Желтый";
                break;
            case SeedBag.PlantType.Tomato:
                itemName = "Помидор";
                break;
            case SeedBag.PlantType.Eggplant:
                itemName = "Баклажан";
                break;
            case SeedBag.PlantType.Potato:
                itemName = "Картошка";
                break;
            case SeedBag.PlantType.Cabage:
                itemName = "Капуста";
                break;
        }
    }
}