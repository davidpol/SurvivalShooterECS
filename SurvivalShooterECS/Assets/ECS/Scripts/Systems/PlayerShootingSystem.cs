using Unity.Entities;
using UnityEngine;

public partial class PlayerShootingSystem : SystemBase
{
    private float timer;

    private PlayerGunObject gun;
    private AudioSource audio;
    private LineRenderer line;
    private Light light;
    private ParticleSystem particles;

    protected override void OnUpdate()
    {
        var hasToExit = true;
        var playerShot = false;
        Entities.ForEach(
            (Entity entity, ref HealthData health, ref PlayerInputData input) =>
            {
                if (health.Value > 0)
                    hasToExit = false;

                if (input.Shoot > 0.0f)
                    playerShot = true;
            }).Run();

        if (hasToExit)
            return;

        timer += Time.DeltaTime;

        var timeBetweenBullets = SurvivalShooterBootstrap.Settings.TimeBetweenBullets;
        var effectsDisplayTime = SurvivalShooterBootstrap.Settings.GunEffectsDisplayTime;

        InitializeGun();
        if (gun != null)
        {
            if (playerShot && timer > timeBetweenBullets)
                Shoot();

            if (timer >= timeBetweenBullets * effectsDisplayTime)
                DisableEffects();
        }
    }

    private void Shoot()
    {
        timer = 0f;

        audio.Play();

        light.enabled = true;

        particles.Stop();
        particles.Play();

        var go = GameObject.FindObjectOfType<PlayerObject>().GunPivot;
        var pos = go.transform.position;
        particles.transform.position = pos;
        line.enabled = true;
        line.SetPosition(0, pos);

        var shootRay = new Ray
        {
            origin = pos,
            direction = go.transform.forward
        };

        RaycastHit shootHit;
        if (Physics.Raycast(shootRay, out shootHit, SurvivalShooterBootstrap.Settings.GunRange,
            LayerMask.GetMask("Shootable")))
        {
            var enemyObj = shootHit.collider.gameObject.GetComponent<EnemyObject>();
            if (enemyObj != null)
            {
                var hitEntity = enemyObj.GetComponent<EnemyObject>().Entity;
                if (!EntityManager.HasComponent<DamagedData>(hitEntity))
                    EntityManager.AddComponentData(hitEntity, new DamagedData
                    {
                        Damage = SurvivalShooterBootstrap.Settings.DamagePerShot,
                        HitPoint = shootHit.point
                    });
            }

            line.SetPosition(1, shootHit.point);
        }
        else
        {
            line.SetPosition(1, shootRay.origin + shootRay.direction * SurvivalShooterBootstrap.Settings.GunRange);
        }
    }

    private void DisableEffects()
    {
        light.enabled = false;
        line.enabled = false;
    }

    private void InitializeGun()
    {
        if (gun == null)
        {
            gun = GameObject.FindObjectOfType<PlayerGunObject>();
            if (gun != null)
            {
                audio = gun.GetComponent<AudioSource>();
                line = gun.GetComponent<LineRenderer>();
                light = gun.GetComponent<Light>();
                particles = gun.GetComponent<ParticleSystem>();
            }
        }
    }
}
