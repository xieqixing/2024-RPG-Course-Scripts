using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlotUI : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected Image itemImage;
    [SerializeField] protected TextMeshProUGUI itemText;

    public InventoryItem item;
    protected UI ui;

    protected virtual void Start()
    {
        ui = GetComponentInParent<UI>();
    }

    public void UpdateSlot(InventoryItem _newItem)
    {
        item = _newItem;
        itemImage.color = Color.white;

        if (item != null)
        {
            itemImage.sprite = item.data.icon;

            if (item.stackSize > 1)
            {
                itemText.text = item.stackSize.ToString();
            }
            else
            {
                itemText.text = "";
            }
        }
    }

    public void CleanUpSlot()
    {
        item = null;

        itemImage.sprite = null;
        itemImage.color = Color.clear;
        itemText.text = "";
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (item == null) 
            return;

        if (Input.GetKey(KeyCode.LeftControl))
        {
            Inventory.Instance.RemoveItem(item.data);
            return;
        }

        if(item.data.itemType == ItemType.Equipment)
        {
            Inventory.Instance.EquipItem(item.data);
            //Debug.Log("Equiped new item: " + item.data.itemName);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(item == null) return;

        //Vector2 mousePosition = Input.mousePosition;

        ui.itemTooltip.ShowToolTip(item.data as ItemData_Equipment);
        //ui.itemTooltip.transform.position = new Vector2(mousePosition.x - 300f, mousePosition.y + 500f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (item == null) return;

        ui.itemTooltip.HideToolTip();
    }
}
