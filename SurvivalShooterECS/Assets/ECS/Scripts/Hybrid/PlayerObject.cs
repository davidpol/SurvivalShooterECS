using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PlayerObject : MonoBehaviour, IConvertGameObjectToEntity
{
    public Transform GunPivot;
    public Entity Entity;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var settings = SurvivalShooterBootstrap.Settings;
        dstManager.AddComponentData(entity, new PlayerData());
        dstManager.AddComponentData(entity, new HealthData { Value = settings.StartingPlayerHealth });
        dstManager.AddComponentData(entity, new PlayerInputData { Move = new float2(0, 0) });

        Entity = entity;
    }
}
