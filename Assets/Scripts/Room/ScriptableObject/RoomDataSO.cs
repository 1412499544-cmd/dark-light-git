using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "RoomDataSO", menuName = "Map/RoomDataSO")]
public class RoomDataSO : ScriptableObject
{
    public RoomType roomType;
    public Sprite roomSprite;
    //由Addressable序列化保存
    public AssetReference sceneToLoad;
}
