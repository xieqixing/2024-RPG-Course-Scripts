using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftSlotUI : ItemSlotUI
{

    protected override void Start()
    {
        base.Start();
    }

    public void SetupCraftSlot(ItemData_Equipment _data)
    {
        if (_data == null) 
            return;

        item.data = _data;
        itemImage.sprite = _data.icon;
        itemText.text = _data.name;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        ui.craftWindow.SetupCraftWindow(item.data as ItemData_Equipment);
        //ItemData_Equipment craftData = item.data as ItemData_Equipment;

        //Inventory.Instance.CanCraft(craftData, craftData.craftingMaterial);
    }
}
