using UnityEngine;

public class GameManager : SingletonBehaviour<GameManager>
{
    public Canvas rootCanvas;
    public GameManagerInfoSystem gameManagerSystem;
    public Camera mainCam;

    public float dragPower;
    public float dragDamping;
    public float physicMaxVelocity;

    protected override void Awake()
    {
        base.Awake();
        var tokenInit = destroyCancellationToken;
        mainCam ??= Camera.main;
    }
}
