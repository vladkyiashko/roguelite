using UnityEngine;
using System;

public class EnvSpawnZone : MonoBehaviour
{
    [SerializeField] private BoxCollider2D Collider;
    [SerializeField] private Transform Transform;
    public Transform GetTransform => Transform;
    public Vector2Int GetSize => new Vector2Int((int)Collider.size.x, (int)Collider.size.y);
    public Action<EnvSpawnZone> OnTriggerEnter;
    public Action<EnvSpawnZone> OnTriggerExit;

    private void OnTriggerEnter2D(Collider2D other)
    {        
        OnTriggerEnter.Invoke(this);
    }
}
