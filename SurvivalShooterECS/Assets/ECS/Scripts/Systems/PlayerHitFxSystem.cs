using Unity.Entities;
using UnityEngine;

public class PlayerHitFxSystem : ComponentSystem
{
    private ComponentGroup hpUpdatedGroup;
    private ComponentGroup playerGroup;
    
    private static readonly int DieHash = Animator.StringToHash("Die");

    protected override void OnCreateManager()
    {
        hpUpdatedGroup = EntityManager.CreateComponentGroup(
            ComponentType.ReadOnly<HealthUpdatedData>());
        playerGroup = EntityManager.CreateComponentGroup(
            ComponentType.ReadOnly<PlayerData>(),
            ComponentType.ReadOnly<Animator>(),
            ComponentType.ReadOnly<AudioSource>()
        );
    }

    protected override void OnUpdate()
    {
        var gameUi = SurvivalShooterBootstrap.Settings.GameUi;
        
        Entities.With(hpUpdatedGroup).ForEach((Entity entity, ref HealthUpdatedData hp) =>
        {
            PostUpdateCommands.DestroyEntity(entity);
            gameUi.OnPlayerTookDamage(hp.Health);

            var health = hp.Health;
            Entities.With(playerGroup).ForEach((AudioSource audio, Animator animator) =>
            {
                if (health <= 0)
                {
                    var playerDeathClip = SurvivalShooterBootstrap.Settings.PlayerDeathClip;
                    audio.clip = playerDeathClip;

                    animator.SetTrigger(DieHash);
                }

                audio.Play();
            });

        });
        
    }
}
