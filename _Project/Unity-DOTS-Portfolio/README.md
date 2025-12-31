# Unity-DOTS-Portfolio
유니티 DOTS 포트폴리오 Repo입니다.

해당 프로젝트에는 제가 자주사용하는 Random System, Entity Spawner System, Entity Drag System 이 포함되어 있습니다.  
그외, 2D Physics 환경을 구현하기 위한 Physic2DSystem또한 포함되어 있습니다.

## [Random System](https://github.com/dhtpdud/Unity-DOTS-Web-2D-Physics/blob/main/_Project/Unity-DOTS-Portfolio/Assets/Scripts/Systems/GameManagerSystem.cs#L29-L49)
Unity.Mathematics.Random 구조체를 Random Component로 감싸 Singleton화 시킨 후,  
UpdateSeed() 함수를 통해 매 프레임 Seed를 업데이트 시켜줍니다.

## [Entity Spawner System](https://github.com/dhtpdud/Unity-DOTS-Web-2D-Physics/blob/main/_Project/Unity-DOTS-Portfolio/Assets/Scripts/Systems/SpawnerSystem.cs)
minPos, maxPos를 통해 Spawn위치를 랜덤화 시킬 수 있고,  
minSize, maxSize를 통해 크기를 랜덤화 시킬 수 있습니다.

batchCount를 통해 한 프레임에 Spawn되는 Entity수를 조절 할 수도 있습니다.

## [Entity Drag System(MouseInteractionSystem)](https://github.com/dhtpdud/Unity-DOTS-Web-2D-Physics/blob/main/_Project/Unity-DOTS-Portfolio/Assets/Scripts/Systems/MouseInteractionSystem.cs)
GameManagerInfoSystem을 통해 GameManagerSingletonComponent Singleton의 ScreenPointToRayOfMainCam와 ScreenToWorldPointMainCam변수를 매 프레임 업데이트 시켜줍니다.  
그리고, MouseInteractionSystem에서 마우스가 눌려있을 때 PhysicsWorldSingleton.CastRay를 통해 DragableTag가 포함된 Collider를 Drag 할 수 있도록 합니다.

## [Physic2DSystem](https://github.com/dhtpdud/Unity-DOTS-Web-2D-Physics/blob/main/_Project/Unity-DOTS-Portfolio/Assets/Scripts/Systems/Physic2DSystem.cs)
PhysicsMass와 PhysicsVelocity를 가지고있는 모든 Entity들의 InverseInertia.x 와 y값을 0으로 만들어  
z축 회전과 x, y 이동만 가능하도록 만듭니다.
