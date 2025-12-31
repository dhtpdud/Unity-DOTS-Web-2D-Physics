using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(BeforePhysicsSystemGroup))]
[RequireMatchingQueriesForUpdate]
public partial struct Physic2DSystem : ISystem, ISystemStartStop
{
    public float3 initGravity;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameManagerSingletonComponent>();
        state.RequireForUpdate<PhysicsStep>();
    }

    [BurstCompile]
    public void OnStartRunning(ref SystemState state)
    {
        initGravity = SystemAPI.GetSingleton<PhysicsStep>().Gravity;
    }

    [BurstCompile]
    public void OnStopRunning(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        GameManagerSingletonComponent gmComponent = SystemAPI.GetSingleton<GameManagerSingletonComponent>();
        state.Dependency = new Physics2DJob { maxVelocity = gmComponent.physicMaxVelocity }.ScheduleParallel(state.Dependency);
    }
    [BurstCompile]
    partial struct Physics2DJob : IJobEntity
    {
        [ReadOnly] public float maxVelocity;
        public void Execute(ref LocalTransform localTransform, ref PhysicsMass physicsMass, ref PhysicsVelocity physicsVelocity)
        {
            if (math.length(physicsVelocity.Linear) > maxVelocity)
                physicsVelocity.Linear = math.normalize(physicsVelocity.Linear) * maxVelocity;

            #region Invalid worldAABB. Object is too large or too far away from the origin. 버그 방지
            if (physicsVelocity.Linear.z != 0)
                physicsVelocity.Linear.z = 0;
            if (localTransform.Position.z != 0)
                localTransform.Position = new float3(localTransform.Position.x, localTransform.Position.y, 0);
            #endregion

            if (physicsMass.InverseInertia.x == 0 && physicsMass.InverseInertia.y == 0) return;

            physicsMass.InverseInertia = new float3(0, 0, physicsMass.InverseInertia.z);
            physicsVelocity.Angular = new float3(0, 0, physicsVelocity.Angular.z);
            localTransform.Rotation = new quaternion(0, 0, localTransform.Rotation.value.z, localTransform.Rotation.value.w);
        }
    }
}