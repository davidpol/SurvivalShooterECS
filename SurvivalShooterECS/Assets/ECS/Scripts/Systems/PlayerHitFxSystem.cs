using Unity.Entities;
using UnityEngine;

public class PlayerHitFxSystem : ComponentSystem
{
    private EntityQuery hpUpdatedQuery;
    private EntityQuery playerQuery;
    
    private static readonly int DieHash = Animator.StringToHash("Die");

    protected override void OnCreate()
    {
        hpUpdatedQuery = GetEntityQuery(
            ComponentType.ReadOnly<HealthUpdatedData>());
        playerQuery = GetEntityQuery(
            ComponentType.ReadOnly<PlayerData>(),
            ComponentType.ReadOnly<Animator>(),
            ComponentType.ReadOnly<AudioSource>()
        );
    }

    protected override void OnUpdate()
    {
        var gameUi = SurvivalShooterBootstrap.Settings.GameUi;
        
        Entities.With(hpUpdatedQuery).ForEach((Entity entity, ref HealthUpdatedData hp) =>
        {
            PostUpdateCommands.DestroyEntity(entity);
            gameUi.OnPlayerTookDamage(hp.Health);

            var health = hp.Health;
            Entities.With(playerQuery).ForEach((AudioSource audio, Animator animator) =>
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
