using Unity.Entities;
using UnityEngine;

public class LinkGunToPlayerSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<PlayerData>().ForEach((Entity gunEntity, Transform player) =>
        {
            Entities.WithAll<PlayerGunData>().ForEach((Entity playerEntity, Transform gun) =>
            {
                gun.SetParent(player.GetComponent<PlayerObject>().GunPivot, false);
                Enabled = false;
            });
        });
    }
}
