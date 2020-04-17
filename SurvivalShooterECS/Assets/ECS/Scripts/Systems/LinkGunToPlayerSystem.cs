using Unity.Entities;
using UnityEngine;

public class LinkGunToPlayerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.WithoutBurst().WithAll<PlayerData>().ForEach((Entity gunEntity, Transform player) =>
        {
            Entities.WithoutBurst().WithAll<PlayerGunData>().ForEach((Entity playerEntity, Transform gun) =>
            {
                gun.SetParent(player.GetComponent<PlayerObject>().GunPivot, false);
                Enabled = false;
            }).Run();
        }).Run();
    }
}
