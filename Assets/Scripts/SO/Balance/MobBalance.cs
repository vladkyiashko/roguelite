using UnityEngine;

[CreateAssetMenu(menuName = "SO/Balance/MobBalance")]
public class MobBalance : BaseUnitBalance
{
    public float Damage;
    public int Exp;
    public GameObjectWeightInfo[] Drops;
}
