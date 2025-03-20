using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParrySkill : Skill
{
    [Header("Parry")]
    [SerializeField] private SkillTreeSlotUI parryUnlockButton;
    public bool parryUnlocked {  get; private set; }

    [Header("Parry restore")]
    [SerializeField] private SkillTreeSlotUI restoreUnlockButton;
    [Range(0f, 1f)]
    [SerializeField] private float restorePercentage;
    public bool restoreunlocked {  get; private set; }


    [Header("Parry with mirage")]
    [SerializeField] private SkillTreeSlotUI parryWithMirageUnlockButton;
    public bool parryWithMirageUnlocked {  get; private set; }

    public override bool CanUseSkill()
    {
        if (cooldownTimer < 0 && parryUnlocked)
        {
            cooldownTimer = cooldown;
            return true;
        }

        return false;
    }

    public override void UseSkill()
    {
        base.UseSkill();

        if(restoreunlocked)
        {
            int restoreAmount = Mathf.RoundToInt(player.stats.GetMaxHealthValue() * restorePercentage);

            player.stats.IncreaseHealthBy(restoreAmount);
        }
    }

    protected override void Start()
    {
        base.Start();

        parryUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockParry);
        restoreUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockParryRestore);
        parryWithMirageUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockParryWithMirage);
    }

    protected override void CheckUnlock()
    {
        base.CheckUnlock();

        UnlockParry();
        UnlockParryRestore();
        UnlockParryWithMirage();
    }

    private void UnlockParry()
    {
        if(parryUnlockButton.unlock)
            parryUnlocked = true;
    }

    private void UnlockParryRestore()
    {
        if(restoreUnlockButton.unlock)
            restoreunlocked = true;
    }

    private void UnlockParryWithMirage()
    {
        if(parryWithMirageUnlockButton.unlock)
            parryWithMirageUnlocked = true;
    }

    public void MakeMirageOnParry(Transform _enemy)
    {
        if(parryWithMirageUnlocked)
            SkillManager.instance.clone.CreateCloneOnCounterAttack(_enemy);
    }
}
