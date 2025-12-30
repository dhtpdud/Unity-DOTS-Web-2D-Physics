using OSY;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
[RequireMatchingQueriesForUpdate]
[UpdateInGroup(typeof(Unity.Entities.InitializationSystemGroup))]
public partial struct GameManagerInfoSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        if (!SystemAPI.HasSingleton<GameManagerSingletonComponent>())
            state.EntityManager.CreateSingleton<GameManagerSingletonComponent>();
    }

    public void OnUpdate(ref SystemState state)
    {
        ref var gameManagerRW = ref SystemAPI.GetSingletonRW<GameManagerSingletonComponent>().ValueRW;
        gameManagerRW.ScreenPointToRayOfMainCam = GameManager.Instance.mainCam.ScreenPointToRay(Input.mousePosition);
        gameManagerRW.ScreenToWorldPointMainCam = GameManager.Instance.mainCam.ScreenToWorldPoint(Input.mousePosition).ToFloat2();

        gameManagerRW.dragDamping = GameManager.Instance.dragDamping;
        gameManagerRW.dragPower = GameManager.Instance.dragPower;
        gameManagerRW.physicMaxVelocity = GameManager.Instance.physicMaxVelocity;
    }
}
[BurstCompile]
[RequireMatchingQueriesForUpdate]
public partial struct RandomSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        if (!SystemAPI.HasSingleton<RandomComponent>())
            state.EntityManager.CreateSingleton(new RandomComponent
            {
                Random = new Unity.Mathematics.Random((uint)Random.Range(int.MinValue, int.MaxValue))
            });
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        ref var randomSingleton = ref SystemAPI.GetSingletonRW<RandomComponent>().ValueRW;
        if (randomSingleton.isUpdate)
            randomSingleton.isUpdate = false;
    }
}