using UnityEngine;

public class SurvivalShooterSettings : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public AudioClip PlayerDeathClip;
    public AudioClip EnemyDeathClip;

    [HideInInspector]
    public GameUi GameUi;

    public float PlayerMoveSpeed = 6f;
    public int StartingPlayerHealth = 100;

    public int StartingEnemyHealth = 100;
    public float EnemySinkSpeed = 2.5f;
    public int EnemyAttackDamage = 20;
    public float TimeBetweenEnemyAttacks = 0.5f;

    public float TimeBetweenBullets = 0.15f;
    public float GunRange = 100f;
    public float GunEffectsDisplayTime = 0.2f;

    public int DamagePerShot = 20;

    public int ScorePerDeath = 10;

    public float CamRayLen = 100f;
    public float CamSmoothing = 5f;
}
