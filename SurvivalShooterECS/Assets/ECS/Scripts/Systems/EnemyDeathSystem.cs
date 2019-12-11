using Unity.Entities;
using UnityEngine;

public class EnemyDeathSystem : ComponentSystem
{
    private int score;

    private static readonly int DeadHash = Animator.StringToHash("Dead");

    protected override void OnUpdate()
    {
        var gameUi = SurvivalShooterBootstrap.Settings.GameUi;
        var scorePerDeath = SurvivalShooterBootstrap.Settings.ScorePerDeath;

        Entities.WithAll<EnemyData, DeadData>().ForEach(
            (Entity entity, CapsuleCollider collider, Animator animator, AudioSource audio) =>
            {
                collider.isTrigger = true;

                animator.SetTrigger(DeadHash);

                audio.clip = SurvivalShooterBootstrap.Settings.EnemyDeathClip;
                audio.Play();

                EntityManager.DestroyEntity(collider.gameObject.GetComponent<EnemyObject>().Entity);

                score += scorePerDeath;
                gameUi.OnEnemyKilled(score);
            });
    }
}
