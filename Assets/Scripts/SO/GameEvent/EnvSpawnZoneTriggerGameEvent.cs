using UnityEngine;

[CreateAssetMenu(menuName = "SO/GameEvent/EnvSpawnZoneTriggerGameEvent")]
public class EnvSpawnZoneTriggerGameEvent : GenericGameEvent<EnvSpawnZoneTrigger>
{
}

public struct EnvSpawnZoneTrigger
{
    public EnvSpawnZone EnvSpawnZone;
    public GameObject EnvSpawnZoneGO;
    public GameObject OtherGameObject;

    public EnvSpawnZoneTrigger(EnvSpawnZone envSpawnZone, GameObject envSpawnZoneGO, GameObject otherGameObject)
    {
        EnvSpawnZone = envSpawnZone;
        EnvSpawnZoneGO = envSpawnZoneGO;
        OtherGameObject = otherGameObject;
    }
}
