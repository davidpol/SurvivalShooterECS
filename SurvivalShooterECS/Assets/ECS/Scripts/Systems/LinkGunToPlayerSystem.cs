using Unity.Entities;
using UnityEngine;

public class LinkGunToPlayerSystem : ComponentSystem
{
    private EntityQuery playerQuery;
    private EntityQuery gunQuery;

    protected override void OnCreate()
    {
        playerQuery = GetEntityQuery(
            ComponentType.ReadOnly<Transform>(),
            ComponentType.ReadOnly<PlayerData>());
        gunQuery = GetEntityQuery(
            ComponentType.ReadOnly<Transform>(),
            ComponentType.ReadOnly<PlayerGunData>());
    }
    protected override void OnUpdate()
    {
        Entities.With(playerQuery).ForEach((Entity gunEntity, Transform player) =>
        {
            Entities.With(gunQuery).ForEach((Entity playerEntity, Transform gun) =>
            {
                gun.SetParent(player.GetComponent<PlayerObject>().GunPivot, false);
                Enabled = false;
            });
        });
    }
}
