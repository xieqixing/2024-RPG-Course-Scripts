using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;


public enum StatType
{
    strength,
    agility,
    intelligence,
    vitality,
    damage,
    critChance,
    critPower,
    health,
    armor,
    evasion,
    magicRes,
    fireDamage,
    iceDamage,
    LightingDamage
}

public class CharacterStats : MonoBehaviour
{
    private EntityFX fx;

    [Header("Major stats")]
    public Stat strength;   // increase damage and crit,power
    public Stat agility;    // increase evasion and crit.chance
    public Stat intelligence;   // increase magic damage and magic resistance
    public Stat vitality;   // increase health

    [Header("Defensive stats")]
    public Stat maxHealth;
    public Stat armor;
    public Stat evasion;
    public Stat magicResistance;

    [Header("Offensive stats")]
    public Stat damage;
    public Stat critChance;
    public Stat critPower;

    [Header("Magic stats")]
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightingDamage;


    public bool isIgnited;
    public bool isChilled;
    public bool isShocked;

    private float igniteTimer;
    private float chillTimer;
    private float shockedTimer;
    [SerializeField] private float alimentsDuration = 4;


    private float igniteDamageCooldown = .3f;
    private float igniteDamageTimer;
    private int igniteDamage;

    [SerializeField] private GameObject shockStrikePrefab;
    private int shockDamage;

    public int currrentHealth;

    public System.Action onHealthChanged;
    public bool isDead = false;
    public bool isVulnerable;

    protected virtual void Awake()
    {
        fx = GetComponent<EntityFX>();
    }

    protected virtual void Start()
    {
        critPower.SetDefaultValue(150);
        currrentHealth = GetMaxHealthValue();
    }

    protected virtual void Update()
    {
        igniteTimer -= Time.deltaTime;
        chillTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;
        igniteDamageTimer -= Time.deltaTime;

        if(igniteTimer < 0)
            isIgnited = false;

        if(chillTimer < 0)
            isChilled = false;

        if(shockedTimer < 0)
            isShocked = false;

        if (igniteDamageTimer < 0 && isIgnited && currrentHealth > 0)
        {
            //Debug.Log("Take burn damamge" + igniteDamage);
            igniteDamageTimer = igniteDamageCooldown;

            DecreaseHealth(igniteDamage);

            if (currrentHealth < 0)
                Die();
        }
    }

    public void MakeVulnerable(float _duration)
    {
        StartCoroutine(VulnerableForCoroutine(_duration));
    }

    private IEnumerator VulnerableForCoroutine(float _duration)
    {
        isVulnerable = true;

        yield return new WaitForSeconds(_duration);

        isVulnerable = false;
    }

    public virtual void IncreaseStatBy(int _modifier, float _duration, Stat _statToModify)
    {
        StartCoroutine(StatModCoroutine(_modifier, _duration, _statToModify));
    }

    private IEnumerator StatModCoroutine(int _modifier, float _duration, Stat _statToModify)
    {
        _statToModify.AddModifier(_modifier);

        yield return new WaitForSeconds(_duration);

        _statToModify.RemoveModifier(_modifier);
    }

    public virtual void DoDamage(CharacterStats _targetStats)
    {
        if (CanAvoidAttack(_targetStats))
            return;

        _targetStats.GetComponent<Entity>().SetupKnockbackDir(transform);
        int totalDamage = damage.GetValue() + strength.GetValue();

        if (CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
            //Debug.Log("boom!");
        }

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
        _targetStats.TakeDamage(totalDamage);

        //DoMagicalDamage(_targetStats);
    }

    #region Magic Damage and Ailments
    public virtual void DoMagicalDamage(CharacterStats _targetStats)
    {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightingDamage = lightingDamage.GetValue();

        _targetStats.GetComponent<Entity>().SetupKnockbackDir(transform);

        int totalMagicalDamage = _fireDamage + _iceDamage + _lightingDamage + intelligence.GetValue();
        totalMagicalDamage = checkTargetResistance(_targetStats, totalMagicalDamage);

        //Debug.Log(totalMagicalDamage + _fireDamage);
        _targetStats.TakeDamage(totalMagicalDamage);

        AttemptToApplyAilments(_targetStats, _fireDamage, _iceDamage, _lightingDamage);
    }

    private void AttemptToApplyAilments(CharacterStats _targetStats, int _fireDamage, int _iceDamage, int _lightingDamage)
    {
        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightingDamage;
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lightingDamage;
        bool canApplyShock = _lightingDamage > _fireDamage && _lightingDamage > _iceDamage;

        if (Mathf.Max(_fireDamage, _iceDamage, _lightingDamage) <= 0)
            return;

        if (!canApplyChill && !canApplyShock && !canApplyIgnite)
        {
            int randomNum = Random.Range(0, 100);

            if (randomNum < 33)
                canApplyShock = true;
            else if (randomNum < 67)
                canApplyChill = true;
            else
                canApplyIgnite = true;
        }

        if (canApplyIgnite)
            _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * .2f));

        if (canApplyShock)
            _targetStats.SetupShockDamage(Mathf.RoundToInt(_lightingDamage * .3f));

        _targetStats.Applyailments(canApplyIgnite, canApplyChill, canApplyShock);
    }

    private int checkTargetResistance(CharacterStats _targetStats, int totalMagicalDamage)
    {
        totalMagicalDamage -= _targetStats.magicResistance.GetValue() + (3 * _targetStats.intelligence.GetValue());

        if (totalMagicalDamage < 0)
            totalMagicalDamage = 0;
        return totalMagicalDamage;
    }

    public void Applyailments(bool _ignite, bool _chill, bool _shock)
    {
        bool canApplyIgnite = !(isIgnited || isChilled || isShocked);
        bool canApplyChill = !(isIgnited || isChilled || isShocked);
        bool canApplyShock = !(isIgnited || isChilled || isShocked);
        //bool canApplyShock = !(isIgnited || isChilled);

        if (_ignite && canApplyIgnite)
        {
            isIgnited = _ignite;
            igniteTimer = alimentsDuration;

            fx.IgniteFxFor(alimentsDuration);
        }

        if (_chill && canApplyChill)
        {
            isChilled = _chill;
            chillTimer = alimentsDuration;

            float slowPercentage = .2f;
            GetComponent<Entity>().SlowEntityBy(slowPercentage, alimentsDuration);
            fx.ChillFxFor(alimentsDuration);
        }

        if (_shock && canApplyShock)
        {
            isShocked = _shock;
            shockedTimer = alimentsDuration;

            fx.ShockFxFor(alimentsDuration);

            //if(!isShocked)
            //{
            //isShocked = _shock;
            //shockedTimer = alimentsDuration;

            //fx.ShockFxFor(alimentsDuration);
            //}
            //else
            //{
            //    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25);
            //    Transform closestEnemy = null;

            //    float closestDistance = Mathf.Infinity;

            //    foreach (var hit in colliders)
            //    {
            //        if (hit.GetComponent<Enemy>() != null && Vector2.Distance(transform.position, hit.transform.position) > .5f)
            //        {
            //            float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

            //            if (distanceToEnemy < closestDistance)
            //            {
            //                closestDistance = distanceToEnemy;
            //                closestEnemy = hit.transform;
            //            }
            //        }
            //    }

            //    if (closestEnemy != null)
            //        closestEnemy = transform;

            //    GameObject newShockStrike = Instantiate(shockStrikePrefab, transform.position, Quaternion.identity);
            //    newShockStrike.GetComponent<ThunderStrikeController>().Setup(shockDamage, closestEnemy.GetComponent<CharacterStats>());
            //}
        }
    }

    public void SetupIgniteDamage(int _damage) => igniteDamage = _damage;

    public void SetupShockDamage(int _damage) => shockDamage = _damage;

    #endregion


    #region Stats Calculations
    protected int CheckTargetArmor(CharacterStats _targetStats, int totalDamage)
    {
        if(_targetStats.isChilled)
            totalDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue() * .8f);
        else
            totalDamage -= _targetStats.armor.GetValue();

        if (totalDamage < 0)
            totalDamage = 0;
        return totalDamage;
    }

    public virtual void OnEvasion()
    {

    }

    protected bool CanAvoidAttack(CharacterStats _targetStats)
    {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();

        if (isShocked)
            totalEvasion += 20;

        if (Random.Range(0, 100) < totalEvasion)
        {
            _targetStats.OnEvasion();

            //Debug.Log("avoid!");
            return true;
        }

        return false;
    }

    public virtual void TakeDamage(int _damage)
    {
        DecreaseHealth(_damage);

        //Debug.Log(_damage);
        GetComponent<Entity>().DamageEffect();

        if (currrentHealth < 0)
            Die();
    }

    public virtual void IncreaseHealthBy(int _amount)
    {
        currrentHealth += _amount;

        if(currrentHealth >= GetMaxHealthValue())
            currrentHealth = GetMaxHealthValue();

        if (onHealthChanged != null)
            onHealthChanged();
    }

    protected virtual void DecreaseHealth(int _damage)
    {
        if(isVulnerable)
            _damage = Mathf.RoundToInt(1.1f * _damage);

        currrentHealth -= _damage;

       if(onHealthChanged != null)
            onHealthChanged();
    }

    protected bool CanCrit()
    {
        int totalCriticalChance = critChance.GetValue() + agility.GetValue();

        if(Random.Range(0, 100) <= totalCriticalChance)
        {
            return true;
        }

        return false;
    }

    protected int CalculateCriticalDamage(int _damage)
    {
        float totalCritPower = (critPower.GetValue() + strength.GetValue()) * .01f;
        //Debug.Log("total crit power:" + totalCritPower);

        float critDamage = _damage * totalCritPower;
        //Debug.Log("crit damage:" + critDamage);

        return Mathf.RoundToInt(critDamage);
    }

    public int GetMaxHealthValue()
    {
        return maxHealth.GetValue() + vitality.GetValue() * 5;
    }
    #endregion

    protected virtual void Die()
    {

    }

    public void Kill()
    {
        Die();
    }

    public Stat StatToModify(StatType _statType)
    {
        switch (_statType)
        {
            case StatType.strength: return strength;
            case StatType.agility: return agility;
            case StatType.intelligence: return intelligence;
            case StatType.vitality: return vitality;
            case StatType.damage: return damage;
            case StatType.critChance: return critChance;
            case StatType.critPower: return critPower;
            case StatType.health: return maxHealth;
            case StatType.armor: return armor;
            case StatType.evasion: return evasion;
            case StatType.magicRes: return magicResistance;
            case StatType.fireDamage: return fireDamage;
            case StatType.iceDamage: return iceDamage;
            case StatType.LightingDamage: return lightingDamage;
        }

        return null;
    }
}
