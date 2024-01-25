using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat : MonoBehaviour
{
    [SerializeField]
    protected int _level;
    [SerializeField]
    protected int _hp;
    [SerializeField]
    protected int _maxHp;
    [SerializeField]
    protected int _attack;
    [SerializeField]
    protected int _defense;
    [SerializeField]
    protected float _moveSpeed;
    [SerializeField]
    protected float _dashingPower;
    [SerializeField]
    protected float _dashingTime;
    [SerializeField]
    protected float _dashingCooldown;

    public int Level { get { return _level; } set { _level = value; } }
    public int Hp { get { return _hp; } set { _hp = value; } }
    public int MaxHp { get { return _maxHp; } set { _maxHp = value; } }
    public int Attack { get { return _attack; } set { _attack = value; } }
    public int Defense { get { return _defense; } set { _defense = value; } }
    public float MoveSpeed { get { return _moveSpeed; } set { _moveSpeed = value; } }
    public float DashingSpeed { get { return _dashingPower; } set { _dashingPower = value; } }
    public float DashingTime { get { return _dashingTime; } set { _dashingTime = value; } }
    public float DashingCooldown { get { return _dashingCooldown; } set { _dashingCooldown = value; } }

    private void Start()
    {
        _level = 1;
        _hp = 100;
        _maxHp = 100;
        _attack = 10;
        _defense = 5;
        _moveSpeed = 5.0f;
        _dashingPower = 10.0f;
        _dashingTime = 0.2f;
        _dashingCooldown = 0.1f;
    }

    public virtual void OnAttacked(Stat attacker)
    {
		int damage = Mathf.Max(0, attacker.Attack - Defense);
		Hp -= damage;
        if (Hp <= 0)
        {
            Hp = 0;
            OnDead(attacker);
        }   
    }

    protected virtual void OnDead(Stat attacker)
    {
		PlayerStat playerStat = attacker as PlayerStat;
		if (playerStat != null)
		{
            playerStat.Exp += 15;
		}

        Managers.Game.Despawn(gameObject);
    }
}
