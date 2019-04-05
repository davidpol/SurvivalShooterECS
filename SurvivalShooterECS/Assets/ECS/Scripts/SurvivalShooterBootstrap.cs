using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;

public sealed class SurvivalShooterBootstrap
{
    public static SurvivalShooterSettings Settings;

    public static void NewGame()
    {
        var player = Object.Instantiate(Settings.PlayerPrefab);
        var entity = player.GetComponent<GameObjectEntity>().Entity;
        var entityManager = World.Active.EntityManager;
        entityManager.AddComponentData(entity, new PlayerData());
        entityManager.AddComponentData(entity, new HealthData { Value = Settings.StartingPlayerHealth });
        entityManager.AddComponentData(entity, new PlayerInputData { Move = new float2(0, 0) });

        Settings.GameUi = GameObject.Find("GameUi").GetComponent<GameUi>();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void InitializeWithScene()
    {
        var settingsGo = GameObject.Find("Settings");
        Settings = settingsGo?.GetComponent<SurvivalShooterSettings>();
        Assert.IsNotNull(Settings);
    }
}
