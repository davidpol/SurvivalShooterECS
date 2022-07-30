using UnityEngine;
using UnityEngine.Assertions;

public sealed class SurvivalShooterBootstrap
{
    public static SurvivalShooterSettings Settings;

    public static void NewGame()
    {
        var player = Object.Instantiate(Settings.PlayerPrefab);
        var gun = Object.Instantiate(Settings.GunPrefab);
        gun.transform.parent = player.GetComponent<PlayerObject>().GunPivot.transform;
        Settings.GameUi = GameObject.Find("GameUi").GetComponent<GameUi>();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void InitializeWithScene()
    {
        var settingsGo = GameObject.Find("Settings");
        if (settingsGo != null)
            Settings = settingsGo.GetComponent<SurvivalShooterSettings>();
        Assert.IsNotNull(Settings);
    }
}
