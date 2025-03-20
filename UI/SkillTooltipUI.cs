using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillTooltipUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI skillText;
    [SerializeField] private TextMeshProUGUI skillName;

    private void Start()
    {
        HideTooltip();
    }

    public void ShowTooltip(string _text, string _name)
    {
        skillName.text = _name;
        skillText.text = _text;

        gameObject.SetActive(true);
    }

    public void HideTooltip() => gameObject.SetActive(false);
}
