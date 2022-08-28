using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Balance/PlayerAttacksBalance")]
public class PlayerAttacksBalance : ScriptableObject
{
    public PlayerAttacksBalanceItem[] Attacks;
    public float AttackDestroyDelay = 0.5f;
    public float DamageTextDestroyDelay = 1f;
    private Dictionary<BasePlayerAttack, WaitForSeconds> _StunWaitForSecondsByAttackPrefab;
    public Dictionary<BasePlayerAttack, WaitForSeconds> StunWaitForSecondsByAttackPrefab
    {
        get
        {
            if (_StunWaitForSecondsByAttackPrefab == null)
            {
                InitStunWaitForSecondsByAttackPrefab();
            }

            return _StunWaitForSecondsByAttackPrefab;
        }
    }
    private Dictionary<BasePlayerAttack, WaitForSeconds> _DelayWaitForSecondsByAttackPrefab;
    public Dictionary<BasePlayerAttack, WaitForSeconds> DelayWaitForSecondsByAttackPrefab
    {
        get
        {
            if (_DelayWaitForSecondsByAttackPrefab == null)
            {
                InitDelayWaitForSecondsByAttackPrefab();
            }

            return _DelayWaitForSecondsByAttackPrefab;
        }
    }
    private WaitForSeconds _AttackDestroyDelayWaitForSeconds;
    public WaitForSeconds AttackDestroyDelayWaitForSeconds
    {
        get
        {
            _AttackDestroyDelayWaitForSeconds ??= new(AttackDestroyDelay);

            return _AttackDestroyDelayWaitForSeconds;
        }
    }
    private WaitForSeconds _DamageTextDestroyDelayWaitForSeconds;
    public WaitForSeconds DamageTextDestroyDelayWaitForSeconds
    {
        get
        {
            _DamageTextDestroyDelayWaitForSeconds ??= new(DamageTextDestroyDelay);

            return _DamageTextDestroyDelayWaitForSeconds;
        }
    }
    private Dictionary<BasePlayerAttack, BasePlayerAttackBalance> _AttackBalanceByAttackPrefab;
    public Dictionary<BasePlayerAttack, BasePlayerAttackBalance> AttackBalanceByAttackPrefab
    {
        get
        {
            if (_AttackBalanceByAttackPrefab == null)
            {
                InitAttackBalanceByAttackPrefab();
            }

            return _AttackBalanceByAttackPrefab;
        }
    }

    private void InitStunWaitForSecondsByAttackPrefab()
    {
        _StunWaitForSecondsByAttackPrefab = new();
        for (int i = 0; i < Attacks.Length; i++)
        {
            _StunWaitForSecondsByAttackPrefab.Add(Attacks[i].Prefab,
                    new WaitForSeconds(Attacks[i].Balance.StunDuration));
        }
    }

    private void InitDelayWaitForSecondsByAttackPrefab()
    {
        _DelayWaitForSecondsByAttackPrefab = new();
        for (int i = 0; i < Attacks.Length; i++)
        {
            _DelayWaitForSecondsByAttackPrefab.Add(Attacks[i].Prefab,
                    new WaitForSeconds(Attacks[i].Balance.Delay));
        }
    }

    private void InitAttackBalanceByAttackPrefab()
    {
        _AttackBalanceByAttackPrefab = new();
        for (int i = 0; i < Attacks.Length; i++)
        {
            _AttackBalanceByAttackPrefab.Add(Attacks[i].Prefab, Attacks[i].Balance);
        }
    }

    [Serializable]
    public struct PlayerAttacksBalanceItem
    {
        public BasePlayerAttack Prefab;
        public BasePlayerAttackBalance Balance;
    }
}
