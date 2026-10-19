using Unity.Cinemachine;
using UnityEngine;

public class CinemachineCameraController : MonoBehaviour, IUpdatable
{
    private CinemachineCamera cinemachineCamera;

    private void Awake()
    {
        cinemachineCamera = GetComponent<CinemachineCamera>();
    }

    public void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.Unregister(this);
    }

    public void OnEnable()
    {
        GameManager.Instance.Register(this);
    }

    public void OnFixedUpdate() { }

    public void OnLateUpdate() { }

    public void OnUpdate()
    { 
        if (cinemachineCamera.Follow == null)
        {
            if (PlayerManager.player != null)
                cinemachineCamera.Follow = PlayerManager.player.transform;
        }
    }
}
