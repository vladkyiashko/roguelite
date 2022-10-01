using System;
using System.Collections.Generic;
using DataStructures.RandomSelector;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Balance/PlayerAttacksBalance")]
public class PlayerAttacksBalance : ScriptableObject
{
    public PlayerItemsBalanceItem[] Attacks;
    public float DamageTextDestroyDelay = 1f;
    private DynamicRandomSelector<int> Selector;
    public DynamicRandomSelector<int> GetSelector
    {
        get
        {
            if (Selector == null)
            {
                InitSelector();
            }

            return Selector;
        }
    }

    private Dictionary<int, WaitForSeconds> _StunWaitForSecondsById;
    public Dictionary<int, WaitForSeconds> StunWaitForSecondsById
    {
        get
        {
            if (_StunWaitForSecondsById == null)
            {
                InitStunWaitForSecondsById();
            }

            return _StunWaitForSecondsById;
        }
    }
    private Dictionary<int, WaitForSeconds> _DelayWaitForSecondsById;
    public Dictionary<int, WaitForSeconds> DelayWaitForSecondsById
    {
        get
        {
            if (_DelayWaitForSecondsById == null)
            {
                InitDelayWaitForSecondsById();
            }

            return _DelayWaitForSecondsById;
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

    public void SelectorRemoveId(int[] ids)
    {
        for (int i = 0; i < ids.Length; i++)
        {
            Selector.Remove(ids[i]);
        }
        _ = Selector.Build();
    }

    public void SelectorAddIds(int[] ids)
    {
        for (int i = 0; i < ids.Length; i++)
        {
            Selector.Add(ids[i], Attacks[ids[i]].Weight);
        }
        _ = Selector.Build();
    }

    private void InitSelector()
    {
        Selector = new DynamicRandomSelector<int>();
        for (int i = 0; i < Attacks.Length; i++)
        {
            Selector.Add(i, Attacks[i].Weight);
        }
        _ = Selector.Build();
    }

    private void InitStunWaitForSecondsById()
    {
        _StunWaitForSecondsById = new();
        for (int i = 0; i < Attacks.Length; i++)
        {
            _StunWaitForSecondsById.Add(i, new WaitForSeconds(Attacks[i].Balance.StunDuration));
        }
    }
    
    private void InitDelayWaitForSecondsById()
    {
        _DelayWaitForSecondsById = new();
        for (int i = 0; i < Attacks.Length; i++)
        {
            _DelayWaitForSecondsById.Add(i, new WaitForSeconds(Attacks[i].Balance.Delay));
        }
    }

    [Serializable]
    public struct PlayerItemsBalanceItem
    {
        public BasePlayerAttack Prefab;
        public int Weight;
        public string Name;
        public string Descr;
        public string UpgDescrFormat;
        public float ValueLevelPow;
        public Sprite Sprite;
        public BasePlayerAttackBalance Balance;
    }
}
