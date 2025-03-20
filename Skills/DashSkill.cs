using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashSkill : Skill
{
    [Header("Dash")]
    [SerializeField] private SkillTreeSlotUI dashUnlockButton;
    public bool dashUnlocked {  get; private set; }

    [Header("clone on dash")]
    [SerializeField] private SkillTreeSlotUI cloneOnDashUnlockButton;
    public bool cloneOnDashUnlocked {  get; private set; }

    [Header("Clone on arrival")]
    [SerializeField] private SkillTreeSlotUI cloneOnArrivalUnlockButton;
    public bool cloneonArrivalUnlocked {  get; private set; }

    public override void UseSkill()
    {
        base.UseSkill();


    }

    protected override void Start()
    {
        base.Start();

        dashUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockDash);
        cloneOnDashUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneOnDash);
        cloneOnArrivalUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneOnArrival);
    }

    protected override void CheckUnlock()
    {
        base.CheckUnlock();

        UnlockDash();
        UnlockCloneOnArrival();
        UnlockCloneOnDash();
    }

    public void UnlockDash()
    {
        if (dashUnlockButton.unlock)
        {
            dashUnlocked = true;
        }
    }

    public void UnlockCloneOnDash()
    {
        if(cloneOnDashUnlockButton.unlock)
            cloneOnDashUnlocked = true;
    }

    public void UnlockCloneOnArrival()
    {
        if(cloneOnArrivalUnlockButton.unlock)
            cloneonArrivalUnlocked = true;
    }

    public void CreateCloneOnDashStart()
    {
        if (cloneOnDashUnlocked)
            SkillManager.instance.clone.CreateClone(player.transform, Vector3.zero);
    }

    public void CreateCloneOnDashOver()
    {
        if (cloneonArrivalUnlocked)
            SkillManager.instance.clone.CreateClone(player.transform, Vector3.zero);
    }
}
