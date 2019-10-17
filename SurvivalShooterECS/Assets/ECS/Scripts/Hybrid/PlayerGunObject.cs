using UnityEngine;
using Unity.Entities;

public class PlayerGunObject : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new PlayerGunData());
    }
}
