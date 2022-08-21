using System;
using UnityEngine;

public class Loot : MonoBehaviour
{
    [SerializeField] private LootType LootType;
    [SerializeField] private Transform Transform;
    public DropPickup DropPickup { get; set; }
    public Transform GetTransform => Transform;
    public LootType GetLootType => LootType;

    public void OnTrigger()
    {
        DropPickup.OnTrigger(this);
    }
}

[Serializable]
public enum LootType
{
    none,
    gem
}
