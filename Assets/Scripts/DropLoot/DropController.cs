using UnityEngine;

public class DropController : MonoBehaviour
{
    private LocalObjectPoolGeneric<Loot> Pool;

    private void Awake()
    {
        Pool = new LocalObjectPoolGeneric<Loot>();
    }

    public void OnMobZeroHealth(MobHolder mobHolder)
    {
        GameObjectWeightInfo dropInfo = mobHolder.GetBalance.GetSelector.SelectRandomItem();
        if (dropInfo.Prefab == null)
        {
            return;
        }

        Loot lootInstance = Pool.Instantiate(dropInfo.Prefab);
        lootInstance.Value = mobHolder.GetBalance.Exp;
        lootInstance.GetTransform.position = mobHolder.GetTransform.position;
    }

    public void OnPickup(Loot lootInstance)
    {
        Pool.Destroy(lootInstance.GetTransform);
    }
}
