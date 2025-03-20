using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class CloneSkillController : MonoBehaviour
{
    private SpriteRenderer sr;
    private Animator anim;

    [SerializeField] private float cloneLosingSpeed;

    private float cloneTimer;
    [SerializeField] private Transform attackCheck;
    [SerializeField] private float attackCheckRadius = .8f;
    private bool canDuplicateClone;
    private int facingDir = 1;
    private float chanceToDuplicate;

    private Transform closestEnemy;

    private float attackMultiplier;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        cloneTimer -= Time.deltaTime;

        if (cloneTimer < 0)
        {
            sr.color = new Color(1, 1, 1, sr.color.a - (Time.deltaTime * cloneLosingSpeed));
        }

        if(sr.color.a <= 0)
            Destroy(gameObject);
    }

    public void SetupClone(Transform _newTransform, float _cloneDuration, bool _canAttack, Vector3 _offset, Transform _closestEnemy, bool _canDuplicate, float _chanceToDuplicate, float _attackMultiplier)
    {
        if(_canAttack)
        {
            anim.SetInteger("AttackNumber", Random.Range(1, 3));
        }

        transform.position = _newTransform.position + _offset;
        cloneTimer = _cloneDuration;
        closestEnemy = _closestEnemy;
        canDuplicateClone = _canDuplicate;
        chanceToDuplicate = _chanceToDuplicate;
        attackMultiplier = _attackMultiplier;

        FaceClosestTarget();
    }

    private void AnimationTrigger()
    {
        cloneTimer = -.1f;
    }

    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                //PlayerManager.instance.player.stats.DoDamage(hit.GetComponent<CharacterStats>());
                hit.GetComponent<Entity>().SetupKnockbackDir(transform);

                PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
                EnemyStats enemyStats = hit.GetComponent<EnemyStats>();

                playerStats.CloneDoDamage(enemyStats, attackMultiplier);

                if(PlayerManager.instance.player.skill.clone.canApplyOnHitEffect)
                {
                    ItemData_Equipment weaponData = Inventory.Instance.GetEquipment(EquipmentType.weapon);

                    if (weaponData != null)
                        weaponData.itemEffect(hit.transform);
                }

                if (canDuplicateClone)
                {
                    if(Random.Range(0, 100) < chanceToDuplicate)
                    {
                        SkillManager.instance.clone.CreateClone(hit.transform, new Vector3(1.5f * facingDir, 0));
                    }
                }
            }
        }
    }

    private void FaceClosestTarget()
    {

        if(closestEnemy != null)
        {
            if (transform.position.x > closestEnemy.position.x)
            {
                facingDir = -1;  
                transform.Rotate(0, 180, 0);
            }
        }

    }
}
