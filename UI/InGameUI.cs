using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Slider slider;

    [SerializeField] private Image dashImage;
    [SerializeField] private Image parryImage;
    [SerializeField] private Image crystalImage;
    [SerializeField] private Image swordImage;
    [SerializeField] private Image blackholeImage;
    [SerializeField] private Image flaskImage;

    private SkillManager skill;

    [Header("Souls info")]
    [SerializeField] private TextMeshProUGUI currentSouls;
    [SerializeField] public float soulsAmount;
    [SerializeField] private float increaseRate;

    // Start is called before the first frame update
    void Start()
    {
        if (playerStats != null)
            playerStats.onHealthChanged += UpdateHealthUI;

        skill = SkillManager.instance;
        
        soulsAmount = PlayerManager.instance.CurrentCurrencyAmount();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCurrency();

        //currentSouls.text = PlayerManager.instance.CurrentCurrencyAmount().ToString("#,#");

        if (Input.GetKeyUp(KeyCode.LeftShift) && skill.dash.dashUnlocked)
            SetCooldownOf(dashImage);

        if (Input.GetKeyUp(KeyCode.Q) && skill.parry.parryUnlocked)
            SetCooldownOf(parryImage);

        if (Input.GetKeyUp(KeyCode.W) && skill.crystal.crystalUnlock)
            SetCooldownOf(crystalImage);

        if (Input.GetKeyUp(KeyCode.Mouse1) && skill.sword.swordUnlocked)
            SetCooldownOf(swordImage);

        if (Input.GetKeyUp(KeyCode.R) && skill.blackhole.blackholeUnlocked)
            SetCooldownOf(blackholeImage);

        if (Input.GetKeyUp(KeyCode.Alpha1) && Inventory.Instance.GetEquipment(EquipmentType.Flask) != null)
            SetCooldownOf(flaskImage);

        CheckCooldownOf(dashImage, skill.dash.cooldown);
        CheckCooldownOf(parryImage, skill.parry.cooldown);
        CheckCooldownOf(crystalImage, skill.crystal.cooldown);
        CheckCooldownOf(swordImage, skill.sword.cooldown);
        CheckCooldownOf(blackholeImage, skill.blackhole.cooldown);
        CheckCooldownOf(flaskImage, Inventory.Instance.flaskCooldown);
    }

    private void UpdateCurrency()
    {
        if (soulsAmount < PlayerManager.instance.CurrentCurrencyAmount())
            soulsAmount += Time.deltaTime * increaseRate;
        else
            soulsAmount = PlayerManager.instance.CurrentCurrencyAmount();

        currentSouls.text = ((int)soulsAmount).ToString("#,#");
    }

    private void UpdateHealthUI()
    {
        slider.maxValue = playerStats.GetMaxHealthValue();
        slider.value = playerStats.currrentHealth;
    }

    private void SetCooldownOf(Image _image)
    {
        if (_image.fillAmount <= 0)
            _image.fillAmount = 1;
    }

    private void CheckCooldownOf(Image _image, float _cooldown)
    {
        if(_image.fillAmount > 0)
            _image.fillAmount -= 1 / _cooldown * Time.deltaTime;
    }
}
