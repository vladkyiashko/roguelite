using UnityEngine;

public class EnvSpawnZone : MonoBehaviour
{
    [SerializeField] private BoxCollider2D Collider;
    [SerializeField] private Transform Transform;
    [SerializeField] private EnvSpawnZoneGameEvent OnTriggerPlayerEnter;
    [SerializeField] private EnvSpawnZoneTriggerGameEvent OnTriggerMobExit;
    public Transform GetTransform => Transform;
    public Vector2Int GetSize => new((int)Collider.size.x, (int)Collider.size.y);

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == Consts.PlayerTag)
        {
            OnTriggerPlayerEnter.Raise(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == Consts.MobTag)
        {
            OnTriggerMobExit.Raise(new EnvSpawnZoneTrigger(this, gameObject, other.gameObject));
        }
    }
}
