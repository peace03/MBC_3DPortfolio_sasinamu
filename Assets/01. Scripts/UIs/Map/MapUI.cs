using UnityEngine;
using UnityEngine.EventSystems;

public class MapUI : MonoBehaviour, IDragHandler, IScrollHandler
{
    [Header("카메라")]
    [SerializeField] private Camera mapCamera;                  // 지도 카메라

    [Header("스탯")]
    [SerializeField] private float moveSensitivity = 0.05f;     // 이동 민감도
    [SerializeField] private float zoomSensitivity = 10f;       // 줌 민감도
    [SerializeField] private float minMapSize = 50f;            // 최소 지도 크기
    [SerializeField] private float maxMapSize = 1000f;          // 최대 지도 크기

    // 드래그 함수(지도 이동)
    public void OnDrag(PointerEventData eventData)
    {
        // 좌우 이동값
        Vector3 moveRight = mapCamera.transform.right * eventData.delta.x * moveSensitivity;
        // 상하 이동값
        Vector3 moveUp = mapCamera.transform.up * eventData.delta.y * moveSensitivity;
        // 지도 카메라 움직이기
        mapCamera.transform.position -= (moveRight + moveUp);
    }

    // 스크롤 함수(지도 확대/축소)
    public void OnScroll(PointerEventData eventData)
    {
        // 현재 지도 크기에서 스크롤 값을 반영한 크기 만들기
        float newSize = mapCamera.orthographicSize - (eventData.scrollDelta.y * zoomSensitivity);
        // 지도 크기 바꾸기
        mapCamera.orthographicSize = Mathf.Clamp(newSize, minMapSize, maxMapSize);
    }
}