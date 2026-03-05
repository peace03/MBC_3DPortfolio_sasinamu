using System.Collections.Generic;
using UnityEngine;

public class BT_Sequence : BT_Node
{
    private List<BT_Node> children;     // 자식 노드들(BT_Node를 상속받는 Sequence, Leaf(Action)을 저장)

    // 생성자
    public BT_Sequence(List<BT_Node> children) => this.children = children;

    // 노드들의 상태 현황을 판단 후 결과 보고하는 함수
    public override BT_NodeStatus Evaluate()
    {
        // 자식 노드들 수만큼
        foreach (var node in children)
        {
            // 노드 한 개의 상태 현황 결과 저장
            var status = node.Evaluate();

            // 결과가 실패라면
            if (status == BT_NodeStatus.Failure)
                // 결과 보고 후 종료
                return BT_NodeStatus.Failure;
            // 결과가 진행 중이라면
            else if (status == BT_NodeStatus.Running)
                // 결과 보고 후 종료
                return BT_NodeStatus.Running;
        }

        // 결과 보고 후 종료
        return BT_NodeStatus.Success;
    }
}