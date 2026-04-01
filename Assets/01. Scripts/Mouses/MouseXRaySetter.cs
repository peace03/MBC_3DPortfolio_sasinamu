using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MouseXRaySetter : MonoBehaviour
{
    [Header("정보")]
    [SerializeField] private LayerMask fadingLayerMask;         // 페이딩 레이어

    [Header("플레이어")]
    [SerializeField] private Transform player;                  // 플레이어

    [Header("매니저")]
    [SerializeField] private InputManager inputManager;         // 인풋 매니저

    [Header("스탯")]
    [SerializeField]
    [Range(0f, 1f)] private float fadingRadius;                 // 페이딩 원 반지름(화면 크기 0 ~ 1 기준)

    private int mousePosId;                                     // 셰이더의 마우스 위치 아이디
    private int radiusId;                                       // 셰이더의 원 반지름 아이디
    private int screenRatioId;                                  // 셰이더의 화면 비율 아이디
                                                                
    private float screenRatio                                   // 화면 비율
        = (float)Screen.width / Screen.height;

    private void Awake()
    {
        // 초기화
        mousePosId = Shader.PropertyToID("_SeeThroughPos");
        radiusId = Shader.PropertyToID("_SeeThroughRadius");
        screenRatioId = Shader.PropertyToID("_SeeThroughScreenRatio");
    }

    private void Update()
    {
        // 마우스의 월드 좌표 전달
        Shader.SetGlobalVector(mousePosId, CalculateMousePositionRatio(inputManager.CurMousePos, screenRatio));
        // 페이딩 거리 전달
        Shader.SetGlobalFloat(radiusId, fadingRadius);
        // 화면 비율 전달
        Shader.SetGlobalFloat(screenRatioId, screenRatio);
    }

    // 카메라의 마우스 위치를 비율로 바꾼 후, 셰이더 벡터(Vector4)로 반환하는 함수
    private Vector4 CalculateMousePositionRatio(Vector2 pos, float xRatio)
    {
        // 마우스의 X, Y좌표 제한 적용
        float clampX = Mathf.Clamp(pos.x, 0f, Screen.width);
        float clampY = Mathf.Clamp(pos.y, 0f, Screen.height);
        // 제한된 마우스의 X좌표 정규화 후 화면 비율 적용
        float mouseX = (clampX / Screen.width) * xRatio;
        // 제한된 마우스의 Y좌표 정규화
        float mouseY = clampY / Screen.height;
        // 셰이더를 위한 Vector4 반환
        return new Vector4(mouseX, mouseY, 0, 0);
    }
}