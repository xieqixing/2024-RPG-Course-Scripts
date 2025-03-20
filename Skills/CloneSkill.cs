using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloneSkill : Skill
{
    [Header("Clone info")]
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;
    [SerializeField] private float attackMutiplier;

    [Header("Clone attack")]
    [SerializeField] private SkillTreeSlotUI cloneAttackUnlockButton;
    [SerializeField] private float cloneAttackMultiplier;
    [SerializeField] private bool canAttack;

    [Header("Aggressive clone")]
    [SerializeField] private SkillTreeSlotUI aggressiveCloneUnlockButton;
    [SerializeField] private float aggressiveCloneMultiplier;
    public bool canApplyOnHitEffect { get; private set; }

    [Header("Mutiple clone")]
    [SerializeField] private SkillTreeSlotUI multipleUnlockButton;
    [SerializeField] private float multipleAttackMultiplier;
    [SerializeField] private bool canDuplicateClone;
    [SerializeField] private float chanceToDuplicate;

    [Header("Crystal instead of clone")]
    [SerializeField] private SkillTreeSlotUI crystalInsteadUnlockButton;
    public bool crystalInsteadOfClone;

    protected override void Start()
    {
        base.Start();

        cloneAttackUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneAttack);
        aggressiveCloneUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockAggressiveClone);
        multipleUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockMultiClone);
        crystalInsteadUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCrystalInstead);
    }

    protected override void CheckUnlock()
    {
        base.CheckUnlock();

        UnlockCloneAttack();
        UnlockAggressiveClone();
        UnlockCrystalInstead();
        UnlockMultiClone();
    }

    #region Unlock region
    private void UnlockCloneAttack()
    {
        if (cloneAttackUnlockButton.unlock)
        {
            canAttack = true;
            attackMutiplier = cloneAttackMultiplier;
        }
    }

    private void UnlockAggressiveClone()
    {
        if (aggressiveCloneUnlockButton.unlock)
        {
            canApplyOnHitEffect  = true;
            attackMutiplier = aggressiveCloneMultiplier;
        }
    }

    private void UnlockMultiClone()
    {
        if (multipleUnlockButton.unlock)
        {
            canDuplicateClone = true;
            attackMutiplier = multipleAttackMultiplier;
        }
    }

    private void UnlockCrystalInstead()
    {
        if(crystalInsteadUnlockButton.unlock)
            crystalInsteadOfClone = true;
    }

    #endregion

    public void CreateClone(Transform _clonePosition, Vector3 _offset)
    {
        if(crystalInsteadOfClone)
        {
            SkillManager.instance.crystal.CreateCrystal();
            return;
        }

        GameObject newClone = Instantiate(clonePrefab);

        newClone.GetComponent<CloneSkillController>().SetupClone(_clonePosition, cloneDuration, canAttack, _offset, FindClosestEnemy(_clonePosition), canDuplicateClone, chanceToDuplicate, attackMutiplier);
    }

    public void CreateCloneOnCounterAttack(Transform _enmeyTransform)
    {
        StartCoroutine(CreateCloneWithDelay(_enmeyTransform, new Vector3(2 * player.facingDir, 0)));
    }


    private IEnumerator CreateCloneWithDelay(Transform _transform, Vector3 _offset)
    {
        yield return new WaitForSeconds(.4f);

        CreateClone(_transform, _offset);
    }
}
