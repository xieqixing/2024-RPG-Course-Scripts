using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    private Player player;

    protected override void Start()
    {
        base.Start();

        player = GetComponent<Player>();
    }

    protected override void DecreaseHealth(int _damage)
    {
        base.DecreaseHealth(_damage);

        if(_damage > GetMaxHealthValue() * 0.3f)
        {
            player.SetupKnockbackPower(new Vector2(7, 10));
            Debug.Log("High damage taken");
        }

        ItemData_Equipment equipedAmulet = Inventory.Instance.GetEquipment(EquipmentType.Armor);

        if (equipedAmulet != null)
            equipedAmulet.itemEffect(player.transform);
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
    }

    protected override void Die()
    {

        if(!isDead)
        {
            base.Die();
            player.Die();
            isDead = true;
            GetComponent<PlayerItemDrop>()?.GenerateDrop();
        }
    }

    public override void OnEvasion()
    {
        base.OnEvasion();

        player.skill.dodge.CreateMirageOnDodge();
    }

    public void CloneDoDamage(CharacterStats _targetStats, float _multiplier)
    {
        if (CanAvoidAttack(_targetStats))
            return;

        int totalDamage = damage.GetValue() + strength.GetValue();

        if(_multiplier > 0)
        {
            totalDamage = Mathf.RoundToInt(totalDamage * _multiplier);
        }

        if (CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
            //Debug.Log("boom!");
        }

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
        _targetStats.TakeDamage(totalDamage);
    }
}
