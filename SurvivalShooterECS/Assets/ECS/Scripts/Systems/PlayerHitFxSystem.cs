using Unity.Entities;
using UnityEngine;

public class PlayerHitFxSystem : ComponentSystem
{
    private ComponentGroup hpUpdatedGroup;
    private ComponentGroup playerGroup;
    
    private static readonly int DieHash = Animator.StringToHash("Die");

    protected override void OnCreateManager()
    {
        hpUpdatedGroup = EntityManager.CreateComponentGroup(typeof(HealthUpdatedData));
        playerGroup = EntityManager.CreateComponentGroup(
            typeof(PlayerData),
            ComponentType.Create<Animator>(),
            ComponentType.Create<AudioSource>()
        );
    }

    protected override void OnUpdate()
    {
        var hpUpdated = hpUpdatedGroup.GetComponentDataArray<HealthUpdatedData>();
        if (hpUpdated.Length == 0)
            return;
        
        var gameUi = SurvivalShooterBootstrap.Settings.GameUi;
        for (var i = 0; i < hpUpdated.Length; ++i)
            gameUi.OnPlayerTookDamage(hpUpdated[i].Health);

        var entity = hpUpdatedGroup.GetEntityArray();
        for (var i = 0; i < entity.Length; ++i)
            PostUpdateCommands.DestroyEntity(entity[i]);

        var audioSource = playerGroup.GetComponentArray<AudioSource>();
            
        if (hpUpdated[0].Health <= 0)
        {
            var playerDeathClip = SurvivalShooterBootstrap.Settings.PlayerDeathClip;
            audioSource[0].clip = playerDeathClip;

            var animators = playerGroup.GetComponentArray<Animator>();
            animators[0].SetTrigger(DieHash);
        }

        audioSource[0].Play();
    }
}
