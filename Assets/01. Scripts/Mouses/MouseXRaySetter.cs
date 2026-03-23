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

    private List<Material> lastFadedMaterials = new();          // 페이딩 효과가 적용된 머테리얼들의 리스트

    private int mousePosId;                                     // 셰이더의 마우스 위치 아이디
    private int radiusId;                                       // 셰이더의 원 반지름 아이디
    private int screenRatioId;                                  // 셰이더의 화면 비율 아이디

    private void Awake()
    {
        // 초기화
        mousePosId = Shader.PropertyToID("_SeeThroughPos");
        radiusId = Shader.PropertyToID("_SeeThroughRadius");
        screenRatioId = Shader.PropertyToID("_SeeThroughScreenRatio");
    }

    private void Update()
    {
        // 페이딩 효과 초기화
        ResetLastFadedMaterials();
        // 페이딩 효과 적용
        ApplyFadingMaterials();
    }


    // 머테리얼들의 페이딩 효과 초기화 함수
    private void ResetLastFadedMaterials()
    {
        // 머테리얼의 수만큼
        foreach (var mat in lastFadedMaterials)
            // 머테리얼이 있다면
            if (mat != null)
                // 페이딩 효과 없애기
                mat.SetFloat(radiusId, 0f);

        // 리스트 초기화
        lastFadedMaterials.Clear();
    }

    // 머테리얼들에 페이딩 효과 적용 함수
    private void ApplyFadingMaterials()
    {
        // 카메라의 니어 플레인 위의 현재 마우스 위치를 관통하는 보이지 않는 선 받아오기
        Ray ray = Camera.main.ScreenPointToRay(inputManager.CurMousePos);
        // 마우스 월드 좌표 주변의 페이딩이 필요한 콜라이더 받아오기
        var hits = Physics.SphereCastAll(ray, fadingRadius * 50f, 30f, fadingLayerMask);
        // 화면 비율 저장
        float screenRatio = (float)Screen.width / Screen.height;

        // 콜라이더의 수만큼
        foreach (var hit in hits)
        {
            // 머테리얼 받아오기
            Material mat = hit.collider.GetComponent<Renderer>().material;
            // 마우스의 월드 좌표 전달
            mat.SetVector(mousePosId, CalculateMousePositionRatio(inputManager.CurMousePos, screenRatio));
            // 페이딩 거리 전달
            mat.SetFloat(radiusId, fadingRadius);
            // 화면 비율 전달
            mat.SetFloat(screenRatioId, screenRatio);
            // 리스트에 추가
            lastFadedMaterials.Add(mat);
        }
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