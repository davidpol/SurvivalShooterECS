using Unity.Entities;
using UnityEngine;

[DisableAutoCreation]
public class PlayerTurningSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var mainCamera = Camera.main;
        if (mainCamera == null)
            return;

        var camRayLen = SurvivalShooterBootstrap.Settings.CamRayLen;
        var floor = LayerMask.GetMask("Floor");

        Entities.WithoutBurst().WithAll<PlayerData>().WithNone<DeadData>().ForEach(
            (Entity entity, Rigidbody rigidBody, in PlayerInputData input) =>
            {
                var mousePos = new Vector3(input.Look.x, input.Look.y, 0);
                var camRay = mainCamera.ScreenPointToRay(mousePos);
                RaycastHit floorHit;
                if (Physics.Raycast(camRay, out floorHit, camRayLen, floor))
                {
                    var position = rigidBody.gameObject.transform.position;
                    var playerToMouse = floorHit.point - new Vector3(position.x, position.y, position.z);
                    playerToMouse.y = 0f;
                    var newRot = Quaternion.LookRotation(playerToMouse);
                    rigidBody.MoveRotation(newRot);
                }
            }).Run();
    }
}
