using Unity.Entities;
using UnityEngine;

public class EnemyObject : MonoBehaviour, IConvertGameObjectToEntity
{
    public Entity Entity;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var settings = SurvivalShooterBootstrap.Settings;
        dstManager.AddComponentData(entity, new EnemyData());
        dstManager.AddComponentData(entity, new HealthData { Value = settings.StartingEnemyHealth });

        Entity = entity;
    }
}
