# Unity-DOTS-Portfolio
유니티 DOTS 포트폴리오 Repo입니다.

해당 프로젝트에는 제가 자주사용하는 Random System, Entity Spawner System, Entity Drag System 이 포함되어 있습니다.
그외, 2D Physics 환경을 구현하기 위한 Physic2DSystem또한 포함되어 있습니다.

## Random System
Unity.Mathematics.Random 구조체를 Random Component로 감싸 Singleton화 시킨 후,  
UpdateSeed() 함수를 통해 매 프레임 Seed를 업데이트 시켜줍니다.

## Entity Spawner System
minPos, maxPos를 통해 Spawn위치를 랜덤화 시킬 수 있고,
minSize, maxSize를 통해 크기를 랜덤화 시킬 수 있습니다.

batchCount를 통해 한 프레임에 Spawn되는 Entity수를 조절 할 수도 있습니다.

## Entity Drag System(MouseInteractionSystem)
GameManagerInfoSystem을 통해 GameManagerSingletonComponent Singleton의 ScreenPointToRayOfMainCam와 ScreenToWorldPointMainCam변수를 업데이트 시켜줍니다.
