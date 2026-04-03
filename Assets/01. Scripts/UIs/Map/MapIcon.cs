using UnityEngine;

public class MapIcon : MonoBehaviour
{
    [Header("정보")]
    [SerializeField] private LayerMask iconLayer;

    private Camera camera;                          // 카메라
    private float mapOriginSize;                    // 지도 원래 크기
    private Vector3 iconOriginSize;                 // 아이콘 원래 크기

    private void Awake()
    {
        // 초기화
        camera = Camera.main;
        mapOriginSize = camera.orthographicSize;
        iconOriginSize = transform.localScale;
    }

    private void Update()
    {
        // 아이콘 위치 고정
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        // 아이콘 크기 고정
        transform.localScale = iconOriginSize * (camera.orthographicSize / mapOriginSize);
    }

    public void ChangeCameraCullingMask(bool state)
    {
        if (state)
            camera.cullingMask |= iconLayer.value;
        else
            camera.cullingMask &= ~iconLayer.value;
    }
}