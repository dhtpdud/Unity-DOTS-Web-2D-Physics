using OSY;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
//using Material = Unity.Physics.Material;
using RaycastHit = Unity.Physics.RaycastHit;

[BurstCompile]
[RequireMatchingQueriesForUpdate]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct MouseInteractionSystem : ISystem, ISystemStartStop
{
    private CollisionFilter clickableFilter;
    private PhysicsWorldSingleton physicsWorld;
    private EntityManager entityManager;
    Entity mouseRockEntity;
    float2 entityPositionOnDown;

    public bool isDraging;

    public float2 mouseLastPosition;
    public float2 mouseVelocity;
    public float2 onMouseDownPosition;
    public float lastEntityRotation;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        clickableFilter = new CollisionFilter { BelongsTo = 1u << 0, CollidesWith = ~(1u << 3), GroupIndex = 0 };
        state.RequireForUpdate<GameManagerSingletonComponent>();
        state.RequireForUpdate<EntityBakeryComponent>();
        entityManager = state.EntityManager;
    }


    [BurstCompile]
    public void OnStartRunning(ref SystemState state)
    {
        mouseRockEntity = entityManager.Instantiate(SystemAPI.GetSingleton<EntityBakeryComponent>().mouseCollider);
        entityManager.SetEnabled(mouseRockEntity, false);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        ref var gameManager = ref SystemAPI.GetSingletonRW<GameManagerSingletonComponent>().ValueRW;
        var deltaTime = SystemAPI.Time.DeltaTime;
        if (Input.GetMouseButtonDown(0))
        {
            //RefRW는 매 프레임 마다. 사용할 때 호출해서 사용해야함.
            OnMouseDown(ref gameManager);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            OnMouseUp(ref gameManager);
        }
        if (Input.GetMouseButton(0))
        {
            OnMouse(ref gameManager, in deltaTime);
        }

        var localTransform = entityManager.GetComponentData<LocalTransform>(mouseRockEntity);
        var velocity = entityManager.GetComponentData<PhysicsVelocity>(mouseRockEntity);
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {

            velocity.Linear *= 0;
            localTransform.Position = gameManager.ScreenToWorldPointMainCam.ToFloat3();

            entityManager.SetComponentData(mouseRockEntity, localTransform);
            entityManager.SetComponentData(mouseRockEntity, velocity);
            entityManager.SetEnabled(mouseRockEntity, true);
        }
        if (!Input.GetKeyDown(KeyCode.LeftAlt) && !Input.GetKey(KeyCode.LeftAlt))
        {
            entityManager.SetEnabled(mouseRockEntity, false);
        }
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            float3 rockToMouse = gameManager.ScreenToWorldPointMainCam.ToFloat3() - localTransform.Position;
            velocity.Linear += rockToMouse * 500 * deltaTime;
            velocity.Linear = math.lerp(velocity.Linear, float3.zero, 20 * deltaTime);
            entityManager.SetComponentData(mouseRockEntity, velocity);
            //왜인지 job을 쓰면 튕겨버림(버그로 추정됨)
        }
    }
    private void OnMouseDown(ref GameManagerSingletonComponent gameManagerRW)
    {
        onMouseDownPosition = gameManagerRW.ScreenToWorldPointMainCam;

        float3 rayStart = gameManagerRW.ScreenPointToRayOfMainCam.origin;
        float3 rayEnd = gameManagerRW.ScreenPointToRayOfMainCam.GetPoint(1000f);
        if (Raycast(rayStart, rayEnd, out RaycastHit raycastHit))
        {
            RigidBody hitRigidBody = physicsWorld.PhysicsWorld.Bodies[raycastHit.RigidBodyIndex];
            Entity hitEntity = hitRigidBody.Entity;
            if (entityManager.HasComponent<DragableTag>(hitEntity))
            {
                LocalTransform localTransform = entityManager.GetComponentData<LocalTransform>(hitEntity);
                lastEntityRotation = localTransform.Rotation.value.z;
                entityPositionOnDown = localTransform.Position.ToFloat2();
                isDraging = true;

                //Material material = Utils.GetMaterial(hitRigidBody, raycastHit.ColliderKey);
                gameManagerRW.dragingEntityInfo = new GameManagerSingletonComponent.DragingEntityInfo(hitEntity, hitRigidBody, raycastHit.ColliderKey/*, material*/);

                /*material.RestitutionCombinePolicy = Material.CombinePolicy.Minimum;
                Utils.SetMaterial(gameManagerRW.dragingEntityInfo.rigidbody, material, raycastHit.ColliderKey);*/
            }
        }
    }
    private void OnMouse(ref GameManagerSingletonComponent gameManagerRW, in float deltaTime)
    {
        if (!isDraging) return;
        PhysicsVelocity velocity = entityManager.GetComponentData<PhysicsVelocity>(gameManagerRW.dragingEntityInfo.entity);
        LocalTransform localTransform = entityManager.GetComponentData<LocalTransform>(gameManagerRW.dragingEntityInfo.entity);

        float2 entityPosition = localTransform.Position.ToFloat2();
        float2 entityPositionFromGrabingPoint = entityPosition - entityPositionOnDown;
        float2 mousePositionFromGrabingPoint = gameManagerRW.ScreenToWorldPointMainCam - onMouseDownPosition;
        float2 entitiyToMouse = mousePositionFromGrabingPoint - entityPositionFromGrabingPoint;
        /*float2 mouseToEntity = entityPositionFromGrabingPoint - mousePositionFromGrabingPoint;

        float angularForce = lastEntityRotation - Vector2.Angle(Vector2.up, mouseToEntity);
        velocity.Angular += angularForce * time.DeltaTime;*/

        velocity.Linear = math.lerp(velocity.Linear, float3.zero, gameManagerRW.dragDamping * deltaTime);
        velocity.Linear += (entitiyToMouse * gameManagerRW.dragPower * deltaTime).ToFloat3();

        entityManager.SetComponentData(gameManagerRW.dragingEntityInfo.entity, velocity);

        lastEntityRotation = localTransform.Rotation.value.z;
    }
    private void OnMouseUp(ref GameManagerSingletonComponent gameManagerRW)
    {
        if (!isDraging) return;
        isDraging = false;
        gameManagerRW.dragingEntityInfo = default;
    }
    private bool Raycast(float3 rayStart, float3 rayEnd, out RaycastHit raycastHit)
    {
        var raycastInput = new RaycastInput
        {
            Start = rayStart,
            End = rayEnd,
            Filter = clickableFilter
        };
        return physicsWorld.CastRay(raycastInput, out raycastHit);
    }

    [BurstCompile]
    public void OnStopRunning(ref SystemState state)
    {
    }
}
