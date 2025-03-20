using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillTreeSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISaveManager
{
    private UI ui;
    private Image skillImage;

    [SerializeField] private int skillPrice;

    [SerializeField] private string skillName;
    [SerializeField] private Color lockedSlotColor;
    [TextArea]
    [SerializeField] private string skillDescription;

    public bool unlock;

    [SerializeField] private SkillTreeSlotUI[] shouldBeUnlocked;
    [SerializeField] private SkillTreeSlotUI[] shouldBeLocked;


    private void OnValidate()
    {
        gameObject.name = "SkillTreeSlot_UI - " + skillName;
    }

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => UnlockSkillSlot());  
    }

    private void Start()
    {
        skillImage = GetComponent<Image>();
        ui = GetComponentInParent<UI>();

        if(unlock)
            skillImage.color = Color.white;
        else
            skillImage.color = lockedSlotColor;
    }

    public void UnlockSkillSlot()
    {
        if(unlock) 
            return;

        for (int i = 0; i < shouldBeUnlocked.Length; i++)
        {
            if (!shouldBeUnlocked[i].unlock)
            {
                return;
            }
        }

        for (int i = 0;i < shouldBeLocked.Length; i++)
        {
            if(shouldBeLocked[i].unlock)
            {
                return ;
            }
        }

        if (!PlayerManager.instance.HaveEnoughMoney(skillPrice))
            return;

        unlock = true;
        skillImage.color = Color.white;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillTooltip.ShowTooltip(skillDescription, skillName);

        Vector2 mousePosition = Input.mousePosition;

        float xOffset = 0;
        float yOffset = 0;

        if (mousePosition.x > 600)
            xOffset = -150;
        else
            xOffset = 150;

        if (mousePosition.y > 320)
            yOffset = -150;
        else
            yOffset = 150;

        ui.skillTooltip.transform.position = new Vector2(mousePosition.x + xOffset, mousePosition.y + yOffset);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillTooltip.HideTooltip();
    }

    public void LoadData(GameData _data)
    {
        //Debug.Log("Load skill tree");
        if(_data.skillTree.TryGetValue(skillName, out bool value))
        {
            unlock = value;
        }
    }

    public void SaveData(ref GameData _data)
    {
        if(_data.skillTree.TryGetValue(skillName, out bool value))
        {
            _data.skillTree.Remove(skillName);
            _data.skillTree.Add(skillName, unlock);
        }
        else
        {
            _data.skillTree.Add(skillName, unlock);
        }
    }
}
