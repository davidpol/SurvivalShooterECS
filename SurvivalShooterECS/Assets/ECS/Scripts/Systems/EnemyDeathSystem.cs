using Unity.Entities;
using UnityEngine;

public class EnemyDeathSystem : ComponentSystem
{
    private ComponentGroup enemyGroup;

    private int score;
    
    private static readonly int DeadHash = Animator.StringToHash("Dead");

    protected override void OnCreateManager()
    {
        enemyGroup = GetComponentGroup(
            typeof(EnemyData),
            typeof(DeadData),
            ComponentType.Create<CapsuleCollider>(),
            ComponentType.Create<Animator>(),
            ComponentType.Create<AudioSource>());
    }

    protected override void OnUpdate()
    {
        var puc = PostUpdateCommands;
        var gameUi = SurvivalShooterBootstrap.Settings.GameUi;
        var scorePerDeath = SurvivalShooterBootstrap.Settings.ScorePerDeath;

        var entity = enemyGroup.GetEntityArray();
        var collider = enemyGroup.GetComponentArray<CapsuleCollider>();
        var animator = enemyGroup.GetComponentArray<Animator>();
        var audioSource = enemyGroup.GetComponentArray<AudioSource>();
        for (var i = 0; i < entity.Length; i++)
        {
            collider[i].isTrigger = true;

            animator[i].SetTrigger(DeadHash);

            audioSource[i].clip = SurvivalShooterBootstrap.Settings.EnemyDeathClip;
            audioSource[i].Play();

            puc.RemoveComponent<DeadData>(entity[i]);

            score += scorePerDeath;
            gameUi.OnEnemyKilled(score);
        }
    }
}
