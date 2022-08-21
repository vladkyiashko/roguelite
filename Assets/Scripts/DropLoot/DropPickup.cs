using DG.Tweening;
using UnityEngine;

public class DropPickup : MonoBehaviour
{
    private const float FlyDuration = 1f;
    [SerializeField] private Transform TargetTransform;

    public void OnTrigger(Loot loot)
    {
        Debug.LogError(gameObject + " DropPickup.OnTrigger " + loot.GetLootType);
        FlyAnimation(loot);
    }

    private void FlyAnimation(Loot loot)
    {
        Vector3[] path = new Vector3[] {loot.GetTransform.position, loot.GetTransform.position - Vector3.left, TargetTransform.position};
        _ = loot.GetTransform.DOPath(path, FlyDuration, PathType.CatmullRom).OnComplete(() => OnFlyComplete(loot));
    }

    private void OnFlyComplete(Loot loot)
    {
        Debug.LogError(gameObject + " DropPickup.OnFlyComplete " + loot.GetLootType);
    }
}
