using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour, ICameraHandler
{
    [SerializeField] private Camera mapCamera;
    [SerializeField] private GameObject mapCinemachine;
    [SerializeField] private GameObject mapCameraLimit;

    private void OnEnable()
    {
        Subject<ICameraHandler>.Attach(this);
        OnCamera(false);
    }

    private void OnDisable()
    {
        Subject<ICameraHandler>.Detach(this);
    }

    public void OnCamera(bool state)
    {
        mapCamera.gameObject.SetActive(state);
        mapCinemachine.SetActive(state);

        if (mapCameraLimit != null)
            mapCameraLimit.SetActive(state);
    }
}