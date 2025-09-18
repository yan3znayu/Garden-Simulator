using UnityEngine;

public class Shovel : PickableItem
{
    new void Awake()
    {
        if (string.IsNullOrEmpty(itemName))
        {
            itemName = "Лопата";
        }
        base.Awake();
    }
}