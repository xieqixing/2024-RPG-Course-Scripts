using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class StatSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private UI ui;

    [SerializeField] private string statName;
    [SerializeField] private StatType statType;
    [SerializeField] private TextMeshProUGUI statValueText;
    [SerializeField] private TextMeshProUGUI statNameText;

    [TextArea]
    [SerializeField] private string statDescription;

    private void OnValidate()
    {
        gameObject.name = "Stat - " + statName;

        if (statNameText != null)
            statNameText.text = statName;
    }

    private void Start()
    {
        UpdateStatValueUI();

        ui = GetComponentInParent<UI>();
    }

    public void UpdateStatValueUI()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        if(playerStats != null)
        {
            statValueText.text = playerStats.StatToModify(statType).GetValue().ToString();

            switch(statType)
            {
                case StatType.health:
                    statValueText.text = playerStats.GetMaxHealthValue().ToString();
                    break;
                case StatType.damage:
                    statValueText.text = (playerStats.damage.GetValue() + playerStats.strength.GetValue()).ToString();
                    break;
                case StatType.critPower:
                    statValueText.text = (playerStats.critPower.GetValue() + playerStats.strength.GetValue()).ToString();
                    break;
                case StatType.critChance:
                    statValueText.text = (playerStats.critChance.GetValue() + playerStats.agility.GetValue().ToString()).ToString();
                    break;
                case StatType.evasion:
                    statValueText.text = (playerStats.evasion.GetValue() + playerStats.agility.GetValue()).ToString();
                    break;
                case StatType.magicRes:
                    statValueText.text = (playerStats.magicResistance.GetValue() + 3 * playerStats.intelligence.GetValue()).ToString();
                    break;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Vector2 mousePosition = Input.mousePosition;

        ui.statTooltip.ShowStatToolTip(statDescription);
        //ui.statTooltip.transform.position = new Vector2(mousePosition.x + 300f, mousePosition.y);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.statTooltip.HideStatToolTip();
    }
}
