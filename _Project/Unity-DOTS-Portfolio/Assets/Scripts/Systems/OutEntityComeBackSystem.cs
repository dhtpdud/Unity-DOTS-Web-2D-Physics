using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[BurstCompile]
partial struct OutEntityComeBackSystem : ISystem, ISystemStartStop
{
    float2 topRightScreenPoint;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameManagerSingletonComponent>();
    }
    [BurstCompile]
    public void OnStartRunning(ref SystemState state)
    {
        topRightScreenPoint = new float2(16, 9);
    }

    [BurstCompile]
    public void OnStopRunning(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Dependency = new OutEnityComeBackJob { topRightScreenPoint = this.topRightScreenPoint }.ScheduleParallel(state.Dependency);
    }

    [BurstCompile]
    [WithAll(typeof(DragableTag))]
    partial struct OutEnityComeBackJob : IJobEntity
    {
        [ReadOnly] public float2 topRightScreenPoint;
        public void Execute(ref LocalTransform localTransform, ref PhysicsVelocity velocity)
        {
            if (localTransform.Position.x > topRightScreenPoint.x)
            {
                localTransform.Position.x = topRightScreenPoint.x - 1;
                velocity.Linear.x *= 0.5f;
            }
            else if (localTransform.Position.x < -topRightScreenPoint.x)
            {
                localTransform.Position.x = -topRightScreenPoint.x + 1;
                velocity.Linear.x *= 0.5f;
            }
            if (localTransform.Position.y > topRightScreenPoint.y)
            {
                localTransform.Position.y = topRightScreenPoint.y - 1;
                velocity.Linear.y *= 0.5f;
            }
            else if (localTransform.Position.y < -topRightScreenPoint.y)
            {
                localTransform.Position.y = -topRightScreenPoint.y + 1;
                velocity.Linear.y *= 0.5f;
            }
        }
    }
}
