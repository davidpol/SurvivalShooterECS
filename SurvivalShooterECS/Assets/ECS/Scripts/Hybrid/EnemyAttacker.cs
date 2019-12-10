using Unity.Entities;
using UnityEngine;

public class EnemyAttacker : MonoBehaviour
{
    private GameObject player;
    private EntityManager entityManager;
    private EntityArchetype enemyAttackArchetype;
    private Entity enemyAttackEntity;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        enemyAttackArchetype = entityManager.CreateArchetype(typeof(EnemyAttackData));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            enemyAttackEntity = entityManager.CreateEntity(enemyAttackArchetype);
            entityManager.SetComponentData(enemyAttackEntity, new EnemyAttackData
            {
                Timer = 0f,
                Frequency = SurvivalShooterBootstrap.Settings.TimeBetweenEnemyAttacks,
                Damage = SurvivalShooterBootstrap.Settings.EnemyAttackDamage,
                Source = GetComponent<EnemyObject>().Entity,
                Target = player.GetComponent<PlayerObject>().Entity
            });
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player && entityManager.Exists(enemyAttackEntity))
            entityManager.DestroyEntity(enemyAttackEntity);
    }
}
