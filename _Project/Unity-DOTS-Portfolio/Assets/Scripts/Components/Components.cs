
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

public struct DragableTag : IComponentData
{

}
public struct EntityBakeryComponent : IComponentData
{
    public Entity mouseCollider;
}
public struct SpawnerComponent : IComponentData
{
    public Entity targetEntity;     //Spawn 될 Entity
    public int maxCount;            //총 Spawn 수
    public int spawnedCount;        //Spawn된 Entity 수


    public float spawnIntervalSec;  //Spawn 간격(초)
    public float currentSec;        //현재 타이머
    public int batchCount;          //분할 Spawn 수, 한 프레임에 Spawn될 수 있는 최대 수

    public float3 minPos;           //최소 Spawn 위치
    public float3 maxPos;           //최대 Spawn 위치

    public bool isRandomSize;       //Random 크기 여부
    public float minSize;           //최소 사이즈
    public float maxSize;           //최대 사이즈즈
}
public struct GameManagerSingletonComponent : IComponentData
{
    public struct DragingEntityInfo
    {
        readonly public Entity entity;
        readonly public RigidBody rigidbody;
        readonly public ColliderKey colliderKey;
        //readonly public Unity.Physics.Material material;

        public DragingEntityInfo(Entity entity, RigidBody rigidbody, ColliderKey colliderKey/*, Unity.Physics.Material material*/)
        {
            this.entity = entity;
            this.rigidbody = rigidbody;
            this.colliderKey = colliderKey;
            //this.material = material;
        }
    }
    public DragingEntityInfo dragingEntityInfo;

    public UnityEngine.Ray ScreenPointToRayOfMainCam;
    public float2 ScreenToWorldPointMainCam;

    public float dragPower;
    public float dragDamping;

    public float physicMaxVelocity;
}
[BurstCompile]
public struct RandomComponent : IComponentData
{
    public bool isUpdate;
    public Random Random;
    [BurstCompile]
    public void UpdateSeed()
    {
        if (!isUpdate)
        {
            Random = new Random((uint)Random.NextInt(int.MinValue, int.MaxValue));
            isUpdate = true;
        }
    }
}