using System;
using UnityEngine;

public class BT_Action : BT_Node
{
    private Func<BT_NodeStatus> action;        // BT_NodeStatus를 반환하는 함수를 저장하는 델리게이트

    // 생성자
    public BT_Action(Func<BT_NodeStatus> action) => this.action = action;

    // 함수의 상태 현황을 판단 후 결과 보고하는 함수
    public override BT_NodeStatus Evaluate()
    {
        // 함수 실행
        return action();
    }
}