using UnityEngine;
using System;

public class EnvSpawnZone : MonoBehaviour
{    
    [SerializeField] private BoxCollider2D Collider;
    [SerializeField] private Transform Transform;
    public Transform GetTransform => Transform;
    public Vector2Int GetSize => new Vector2Int((int)Collider.size.x, (int)Collider.size.y);
    public bool Inited { get; set; }
    public event Action<EnvSpawnZone> OnTriggerPlayerEnter;
    public Action<EnvSpawnZone, GameObject> OnTriggerMobExit;

    private void OnTriggerEnter2D(Collider2D other)
    {        
        if (other.tag == Consts.PlayerTag)
        {
            if (OnTriggerPlayerEnter != null)
            {
                OnTriggerPlayerEnter.Invoke(this);
            }            
        }            
    }

    private void OnTriggerExit2D(Collider2D other)
    {        
        if (other.tag == Consts.MobTag)
        {
            if (OnTriggerMobExit != null)
            {
                OnTriggerMobExit.Invoke(this, other.gameObject);
            }                        
        }            
    }
}
