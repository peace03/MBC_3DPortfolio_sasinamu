using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Data/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("기본 정보")]
    [SerializeField] private Sprite sprite; //인벤토리에 표시될 이미지
    [SerializeField] private GameObject objectPrefab; //3D 모델
    [SerializeField] private GameObject object2DPrefab;
    [SerializeField] private int _id;
    [SerializeField] private string _name;
    [SerializeField] private float _weight;
    [SerializeField] private string _explain;

    public Sprite Sprite => sprite;
    public GameObject ObjectPrefab => objectPrefab;
    public GameObject Object2DPrefab => object2DPrefab;
    public int ID => _id;
    public string Name => _name;
    public float Weight => _weight;
    public string Explain => _explain;
}
