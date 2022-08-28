using UnityEngine;

[CreateAssetMenu(menuName = "SO/GameEvent/PlayerAttackTriggerGameEvent")]
public class PlayerAttackTriggerGameEvent : GenericGameEvent<PlayerAttackTrigger>
{
}

public struct PlayerAttackTrigger
{
    public BasePlayerAttack PlayerAttack;
    public GameObject OtherGameObject;

    public PlayerAttackTrigger(BasePlayerAttack playerAttack, GameObject otherGameObject)
    {
        PlayerAttack = playerAttack;
        OtherGameObject = otherGameObject;
    }
}
