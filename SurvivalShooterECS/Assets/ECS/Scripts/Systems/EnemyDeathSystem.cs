using Unity.Entities;
using UnityEngine;

public class EnemyDeathSystem : ComponentSystem
{
    private EntityQuery query;

    private int score;
    
    private static readonly int DeadHash = Animator.StringToHash("Dead");

    protected override void OnCreate()
    {
        query = GetEntityQuery(
            ComponentType.ReadOnly<EnemyData>(),
            ComponentType.ReadOnly<DeadData>(),
            ComponentType.ReadOnly<CapsuleCollider>(),
            ComponentType.ReadOnly<Animator>(),
            ComponentType.ReadOnly<AudioSource>());
    }

    protected override void OnUpdate()
    {
        var gameUi = SurvivalShooterBootstrap.Settings.GameUi;
        var scorePerDeath = SurvivalShooterBootstrap.Settings.ScorePerDeath;

        Entities.With(query).ForEach(
            (Entity entity, CapsuleCollider collider, Animator animator, AudioSource audio) =>
            {
                collider.isTrigger = true;

                animator.SetTrigger(DeadHash);

                audio.clip = SurvivalShooterBootstrap.Settings.EnemyDeathClip;
                audio.Play();

                PostUpdateCommands.RemoveComponent<DeadData>(entity);

                score += scorePerDeath;
                gameUi.OnEnemyKilled(score);
            });
    }
}
