using UnityEngine;
using UnityEngine.AI;

public class EnemySinker : MonoBehaviour
{
    private bool isSinking;

    private void Update()
    {
        if (isSinking)
            transform.Translate(-Vector3.up * SurvivalShooterBootstrap.Settings.EnemySinkSpeed * Time.deltaTime);
    }

    public void StartSinking()
    {
        GetComponent<NavMeshAgent>().enabled = false;

        GetComponent<Rigidbody>().isKinematic = true;

        isSinking = true;

        Destroy(gameObject, 2f);
    }
}
