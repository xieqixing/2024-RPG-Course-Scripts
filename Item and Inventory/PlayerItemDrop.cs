using System.Collections.Generic;
using UnityEngine;

public class PlayerItemDrop : ItemDrop
{
    [Header("Player's drop")]
    [SerializeField] private float chanceToLoseEquipment;
    [SerializeField] private float chanceToLoseMaterial;

    public override void GenerateDrop()
    {
        Inventory inventory = Inventory.Instance;

        List<InventoryItem> currentEquipment = inventory.GetEquipmentList();
        List<InventoryItem> itemsToUnequip = new List<InventoryItem>();

        List<InventoryItem> currentStash = inventory.GetStashList();
        List<InventoryItem> stashToUnequip = new List<InventoryItem>();

        for (int i = 0; i < currentEquipment.Count; i++)
        {
            if (Random.Range(0, 100) < chanceToLoseEquipment)
            {
                DropItem(currentEquipment[i].data);
                itemsToUnequip.Add(currentEquipment[i]);
            }
        }

        for (int i = 0; i < itemsToUnequip.Count; i++)
            inventory.UnequipmentItem(itemsToUnequip[i].data as ItemData_Equipment);

        for (int i = 0; i < currentStash.Count; i++)
        {
            if (Random.Range(0, 100) < chanceToLoseMaterial)
            {
                DropItem(currentStash[i].data);
                stashToUnequip.Add(currentStash[i]);
            }
        }

        for (int i = 0; i < stashToUnequip.Count; i++)
            inventory.RemoveItem(stashToUnequip[i].data);

    }
}
