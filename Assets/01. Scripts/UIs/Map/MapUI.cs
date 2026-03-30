using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapUI : MonoBehaviour, IDragHandler, IScrollHandler
{
    [Header("카메라")]
    [SerializeField] private CinemachineCamera mapCamera;               // 지도 카메라

    [Header("카메라 타겟")]
    [SerializeField] private Transform mapTarget;                       // 카메라 타겟

    [Header("이동 속도")]
    [SerializeField] private float minMoveSensitivity = 0.00005f;       // 최소 이동 민감도
    [SerializeField] private float maxMoveSensitivity = 0.2f;           // 최대 이동 민감도

    [Header("크기")]
    [SerializeField] private float zoomSensitivity = 10f;               // 줌 민감도
    [SerializeField] private float minMapSize = 50f;                    // 최소 지도 크기
    [SerializeField] private float maxMapSize = 1000f;                  // 최대 지도 크기

    // 드래그 함수(지도 이동)
    public void OnDrag(PointerEventData eventData)
    {
        float height = Mathf.InverseLerp(minMapSize, maxMapSize, mapTarget.position.y);
        // 좌우 이동값
        Vector3 moveRight = mapCamera.transform.right * eventData.delta.x *
                    Mathf.Lerp(minMoveSensitivity, maxMoveSensitivity, height);
        // 상하 이동값
        Vector3 moveUp = mapCamera.transform.up * eventData.delta.y *
                    Mathf.Lerp(minMoveSensitivity, maxMoveSensitivity, height);
        // 지도 카메라 움직이기
        mapTarget.position -= (moveRight + moveUp);
    }

    // 스크롤 함수(지도 확대/축소)
    public void OnScroll(PointerEventData eventData)
    {
        // 현재 지도 크기에서 스크롤 값을 반영한 크기 만들기
        float newHeight = mapTarget.localPosition.y - (eventData.scrollDelta.y * zoomSensitivity);
        // 지도 크기 바꾸기
        mapTarget.localPosition = new Vector3(mapTarget.localPosition.x,
                                                    Mathf.Clamp(newHeight, minMapSize, maxMapSize),
                                                                    mapTarget.localPosition.z);
    }
}