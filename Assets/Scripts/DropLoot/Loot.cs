using System;
using UnityEngine;

public class Loot : MonoBehaviour
{
    [SerializeField] private LootType LootType;
    [SerializeField] private Transform Transform;
    [SerializeField] private LootGameEvent OnLootTrigger;
    public Transform GetTransform => Transform;
    public LootType GetLootType => LootType;
    public int Value { get; set; }

    public void OnTrigger()
    {
        OnLootTrigger.Raise(this);
    }
}

[Serializable]
public enum LootType
{
    none,
    gem
}
