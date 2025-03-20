using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DodgeSkill : Skill
{
    [Header("Dodge")]
    [SerializeField] private SkillTreeSlotUI unlockDodgeButton;
    public bool dodgeUnlock;

    [Header("Mirage dodge")]
    [SerializeField] private SkillTreeSlotUI unlockMirageDodge;
    public bool dodgeMirageUnlock;

    protected override void Start()
    {
        base.Start();

        unlockDodgeButton.GetComponent<Button>().onClick.AddListener(UnlockDodge);
        unlockMirageDodge.GetComponent<Button>().onClick.AddListener(UnlockMirageDodge);
    }

    protected override void CheckUnlock()
    {
        UnlockDodge();
        UnlockMirageDodge();
    }

    private void UnlockDodge()
    {
        if (unlockDodgeButton.unlock)
        {
            player.stats.evasion.AddModifier(10);
            Inventory.Instance.UpdateStatsUI();
            dodgeUnlock = true;
            unlockDodgeButton.GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }

    public void UnlockMirageDodge()
    {
        if(unlockMirageDodge.unlock)
            dodgeMirageUnlock = true;

    }

    public void CreateMirageOnDodge()
    {
        if (dodgeMirageUnlock)
            SkillManager.instance.clone.CreateClone(player.transform, new Vector3(1 * player.facingDir, 0));
    }
}
