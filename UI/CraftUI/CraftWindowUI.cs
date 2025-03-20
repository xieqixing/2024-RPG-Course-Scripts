using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftWindowUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemDescription;
    [SerializeField] private Image itemIcon;
    [SerializeField] private Button craftButton;

    [SerializeField] private Image[] materialImage;

    private void Start()
    {
        for (int i = 0; i < materialImage.Length; i++)
        {
            materialImage[i].color = Color.clear;
            materialImage[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.clear;
        }

        itemIcon.color = Color.clear;

        itemName.text = "";
        itemDescription.text = "";
    }

    public void SetupCraftWindow(ItemData_Equipment _data)
    {
        craftButton.onClick.RemoveAllListeners();

        for (int i = 0; i < materialImage.Length; i++)
        {
            materialImage[i].color = Color.clear;
            materialImage[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.clear;
        }

        for (int i = 0;i < _data.craftingMaterial.Count; i++)
        {
            materialImage[i].sprite = _data.craftingMaterial[i].data.icon;
            materialImage[i].color = Color.white;

            TextMeshProUGUI materialSlotText = materialImage[i].GetComponentInChildren<TextMeshProUGUI>();

            materialSlotText.color = Color.white;
            materialSlotText.text = _data.craftingMaterial[i].stackSize.ToString();
        }

        itemIcon.sprite = _data.icon;
        itemIcon.color = Color.white;
        itemName.text = _data.name;
        itemDescription.text = _data.GetDescription();

        craftButton.onClick.AddListener(() => Inventory.Instance.CanCraft(_data, _data.craftingMaterial));
    }

}
