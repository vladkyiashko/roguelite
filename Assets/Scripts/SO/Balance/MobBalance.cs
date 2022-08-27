using DataStructures.RandomSelector;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Balance/MobBalance")]
public class MobBalance : BaseUnitBalance
{
    public float Damage;
    public int Exp;
    public GameObjectWeightInfo[] Drops;
    private DynamicRandomSelector<GameObjectWeightInfo> Selector;
    public DynamicRandomSelector<GameObjectWeightInfo> GetSelector
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

    private void InitSelector()
    {
        Selector = new DynamicRandomSelector<GameObjectWeightInfo>();
        for (int i = 0; i < Drops.Length; i++)
        {
            Selector.Add(Drops[i], Drops[i].Weight);
        }
        _ = Selector.Build();
    }
}
