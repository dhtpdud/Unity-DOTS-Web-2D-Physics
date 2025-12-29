using OSY;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(Unity.Entities.InitializationSystemGroup))]
public sealed partial class GameManagerInfoSystem : SystemBase
{
    public Camera mainCam;

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        mainCam = Camera.main;
        if (!SystemAPI.HasSingleton<GameManagerSingletonComponent>())
            EntityManager.CreateSingleton<GameManagerSingletonComponent>();
    }

    protected override void OnUpdate()
    {
        ref var gameManagerRW = ref SystemAPI.GetSingletonRW<GameManagerSingletonComponent>().ValueRW;
        gameManagerRW.ScreenPointToRayOfMainCam = mainCam.ScreenPointToRay(Input.mousePosition);
        gameManagerRW.ScreenToWorldPointMainCam = mainCam.ScreenToWorldPoint(Input.mousePosition).ToFloat2();
    }
    public void UpdateSetting()
    {
        ref var gameManagerRW = ref SystemAPI.GetSingletonRW<GameManagerSingletonComponent>().ValueRW;

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