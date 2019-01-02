using Unity.Entities;
using UnityEngine;

public class PlayerTurningSystem : ComponentSystem
{
    private ComponentGroup playerGroup;

    protected override void OnCreateManager()
    {
        playerGroup = GetComponentGroup(
            typeof(Transform),
            typeof(PlayerData),
            ComponentType.Create<Rigidbody>(),
            ComponentType.Subtractive<DeadData>());
    }

    protected override void OnUpdate()
    {
        var mainCamera = Camera.main;
        if (mainCamera == null)
            return;
        
        var mousePos = Input.mousePosition;

        var camRayLen = SurvivalShooterBootstrap.Settings.CamRayLen;
        var floor = LayerMask.GetMask("Floor");

        var go = playerGroup.GetGameObjectArray();
        var rigidbody = playerGroup.GetComponentArray<Rigidbody>();
        for (var i = 0; i < go.Length; i++)
        {
            var camRay = mainCamera.ScreenPointToRay(mousePos);
            RaycastHit floorHit;
            if (Physics.Raycast(camRay, out floorHit, camRayLen, floor))
            {
                var position = go[i].transform.position;
                var playerToMouse = floorHit.point - new Vector3(position.x, position.y, position.z);
                playerToMouse.y = 0f;
                var newRot = Quaternion.LookRotation(playerToMouse);
                rigidbody[i].MoveRotation(newRot);
            }
        }
    }
}
