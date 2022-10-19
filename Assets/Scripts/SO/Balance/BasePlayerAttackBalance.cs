using System;

[Serializable]
public struct BasePlayerAttackBalance
{
    public float StunDuration;

    public int MaxLevel;
    public float BaseDamage;
    public float Speed;
    public float Duration;
    public float Cooldown;
    public float HitboxDelay;
    public int PoolLimit;
    public float Rarity;
    public float Area;
    public int Amount;
    public float ProjectileInterval;
    public int Pierce;
    public float Knockback;
    public float Chance;
    public float CriticalMultiplier;
    public bool BlockedByWalls;

    public bool CooldownAfterDuration;
}
