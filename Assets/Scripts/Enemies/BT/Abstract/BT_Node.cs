using UnityEngine;

// BT의 노드 상태 현황
public enum BT_NodeStatus
{
    Success,        // 성공
    Failure,        // 실패
    Running         // 실행 중
}

public abstract class BT_Node
{
    protected bool logEnabled = false;      // 로그 출력 여부

    // 노드의 상태 현황을 판단 후 결과 보고하는 함수
    public abstract BT_NodeStatus Evaluate();

    // 로그 출력 함수
    protected void Log(string name, string msg)
    {
        // 로그 출력 상태라면
        if (logEnabled)
            // 로그 출력
            Debug.Log($"[{name}] {msg}");
    }
}