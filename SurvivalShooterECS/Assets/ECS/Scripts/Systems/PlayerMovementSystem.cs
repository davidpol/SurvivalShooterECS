using Unity.Entities;
using UnityEngine;

public class PlayerMovementSystem : ComponentSystem
{
    private ComponentGroup inputGroup;

    protected override void OnCreateManager()
    {
        inputGroup = GetComponentGroup(
            ComponentType.Create<Transform>(),
            ComponentType.ReadOnly<PlayerInputData>(),
            ComponentType.Create<Rigidbody>(),
            ComponentType.Subtractive<DeadData>());
    }

    protected override void OnUpdate()
    {
        var speed = SurvivalShooterBootstrap.Settings.PlayerMoveSpeed;
        var dt = Time.deltaTime;

        var go = inputGroup.GetGameObjectArray();
        var input = inputGroup.GetComponentDataArray<PlayerInputData>();
        var rigidbody = inputGroup.GetComponentArray<Rigidbody>();
        for (var i = 0; i < go.Length; i++)
        {
            var move = input[i].Move;

            var movement = new Vector3(move.x, 0, move.y);
            movement = movement.normalized * speed * dt;

            var position = go[i].transform.position;
            var newPos = new Vector3(position.x, position.y, position.z) + movement;
            rigidbody[i].MovePosition(newPos);
        }
    }
}
