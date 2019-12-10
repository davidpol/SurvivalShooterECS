using UnityEngine;
using UnityEngine.Assertions;

public sealed class SurvivalShooterBootstrap
{
    public static SurvivalShooterSettings Settings;

    public static void NewGame()
    {
        Object.Instantiate(Settings.PlayerPrefab);
        Object.Instantiate(Settings.GunPrefab);
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
