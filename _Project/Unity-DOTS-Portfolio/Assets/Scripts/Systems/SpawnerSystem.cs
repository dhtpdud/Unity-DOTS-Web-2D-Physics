using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct SpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        new TimerJob { deltaTime = SystemAPI.Time.DeltaTime }.ScheduleParallel(state.Dependency).Complete();

        EntityCommandBuffer ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        var parallelWriter = ecb.AsParallelWriter();
        ref var randomSingleton = ref SystemAPI.GetSingletonRW<RandomComponent>().ValueRW;

        state.Dependency = new SpawnJob { random = randomSingleton.Random, parallelWriter = parallelWriter }.ScheduleParallel(state.Dependency);
        randomSingleton.UpdateSeed();
    }

    [BurstCompile]
    partial struct TimerJob : IJobEntity
    {
        [ReadOnly] public float deltaTime;
        public void Execute(ref SpawnerComponent spawnerComponent)
        {
            if (spawnerComponent.currentSec < spawnerComponent.spawnIntervalSec)
                spawnerComponent.currentSec += deltaTime;
        }
    }
    [BurstCompile]
    partial struct SpawnJob : IJobEntity
    {
        [ReadOnly] public Random random;
        public EntityCommandBuffer.ParallelWriter parallelWriter;
        public void Execute([ChunkIndexInQuery] int chunkIndex, ref SpawnerComponent spawnerComponent, in LocalTransform spawnerTransformComponent)
        {
            if (spawnerComponent.currentSec < spawnerComponent.spawnIntervalSec || spawnerComponent.spawnedCount >= spawnerComponent.maxCount) return;
            Entity spawnedEntity;
            LocalTransform initTransform;
            int remainCount = spawnerComponent.maxCount - spawnerComponent.spawnedCount;
            if (remainCount <= 0) return;
            int batchCount = remainCount > spawnerComponent.batchCount ? spawnerComponent.batchCount : remainCount;
            for (int i = 0; i < batchCount; i++)
            {
                spawnedEntity = parallelWriter.Instantiate(chunkIndex, spawnerComponent.targetEntity);
                initTransform = new LocalTransform { Position = spawnerTransformComponent.Position + random.NextFloat3(spawnerComponent.minPos, spawnerComponent.maxPos), Rotation = spawnerTransformComponent.Rotation, Scale = spawnerComponent.isRandomSize ? random.NextFloat(spawnerComponent.minSize, spawnerComponent.maxSize) : 1 };
                parallelWriter.SetComponent(chunkIndex, spawnedEntity, initTransform);
                ++spawnerComponent.spawnedCount;
            }
            spawnerComponent.currentSec = 0;
        }
    }
}