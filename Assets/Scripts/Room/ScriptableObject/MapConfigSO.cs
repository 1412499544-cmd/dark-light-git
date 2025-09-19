using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName = "MapConfigSO",menuName = "Map/MapConfigSO")]
public class MapConfigSO : ScriptableObject
{
    public List<RoomBluePrint> roomBluePrints;
}

/// <summary>
/// 房间蓝图
/// 记录随机房间个数,房间类型
/// </summary>
[System.Serializable]
public class RoomBluePrint
{
    public int max,min;
    public RoomType roomType;
}
