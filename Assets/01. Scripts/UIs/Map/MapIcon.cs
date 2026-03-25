using UnityEngine;

public class MapIcon : MonoBehaviour
{
    [Header("카메라")]
    [SerializeField] private Camera mapCamera;      // 지도 카메라

    private float mapOriginSize;                    // 지도 원래 크기
    private Vector3 iconOriginSize;                 // 아이콘 원래 크기

    private void Awake()
    {
        // 초기화
        mapOriginSize = mapCamera.orthographicSize;
        iconOriginSize = transform.localScale;
    }

    private void Update()
    {
        // 아이콘 위치 고정
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        // 아이콘 크기 고정
        transform.localScale = iconOriginSize * (mapCamera.orthographicSize / mapOriginSize);
    }
}