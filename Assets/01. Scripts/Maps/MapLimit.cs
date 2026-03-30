using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class MapLimit : MonoBehaviour
{
    private void Start()
    {
        CombineChildrenMeshes();
    }

    [ContextMenu("Create Map Limit")]
    public void CombineChildrenMeshes()
    {
        MeshFilter[] filters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combines = new CombineInstance[filters.Length - 1];
        int index = 0;

        for (int i = 0; i < filters.Length; i++)
        {
            // 자신이라면
            if (filters[i].gameObject == gameObject)
                // 건너뛰기
                continue;

            combines[index].mesh = filters[i].sharedMesh;
            combines[index].transform = transform.worldToLocalMatrix * filters[i].transform.localToWorldMatrix;
            filters[i].gameObject.SetActive(false);
            index++;
        }

        Mesh finalMesh = new Mesh();
        finalMesh.name = "MapLimit";
        finalMesh.CombineMeshes(combines);

        GetComponent<MeshFilter>().sharedMesh = finalMesh;

        MeshCollider meshCollider = GetComponent<MeshCollider>();
        meshCollider.sharedMesh = finalMesh;
        meshCollider.convex = true;

        Debug.Log("완료");
    }
}