using System;
using UnityEngine;

public class Loot : MonoBehaviour
{
    [SerializeField] private LootType LootType;
    [SerializeField] private Transform Transform;
    public event Action<Loot> OnPickup;
    public DropPickup DropPickup { get; set; }
    public Transform GetTransform => Transform;
    public LootType GetLootType => LootType;
    public int Value { get; set; }

    public void OnTrigger()
    {
        DropPickup.OnTrigger(this);
    }

    public void Pickup()
    {
        OnPickup?.Invoke(this);
    }
}

[Serializable]
public enum LootType
{
    none,
    gem
}
