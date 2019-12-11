using Unity.Entities;
using UnityEngine;

public class PlayerHitFxSystem : ComponentSystem
{
    private static readonly int DieHash = Animator.StringToHash("Die");

    protected override void OnUpdate()
    {
        var gameUi = SurvivalShooterBootstrap.Settings.GameUi;

        Entities.ForEach((Entity entity, ref HealthUpdatedEvent hp) =>
        {
            PostUpdateCommands.DestroyEntity(entity);

            gameUi.OnPlayerTookDamage(hp.Health);

            var health = hp.Health;
            Entities.WithAll<PlayerData>().ForEach((AudioSource audio, Animator animator) =>
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
