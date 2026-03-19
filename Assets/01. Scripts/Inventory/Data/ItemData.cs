using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Data/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("기본 정보")]
    public Sprite sprite; //인벤토리에 표시될 이미지
    public GameObject objectPrefab; //3D 모델
    public int _id;
    public string _name;
    public float _weight;
    public string _explain;
}
