using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("地图布局")]
    public MapLayoutSO mapLayout;

    public void UpdateMapRoom(object value)
    {
        var roomVector = (Vector2Int)value;
        if(mapLayout.mapRoomDataList.Count == 0){return;}
        
        //找到进入的房间设置状态为已进入过
        var currentRoom = mapLayout.mapRoomDataList.Find(room => room.column == roomVector.x && room.line == roomVector.y);
        currentRoom.roomState = RoomState.Visited;

        //找到同一列所有房间并上锁
        var sameColumnRoomVector = mapLayout.mapRoomDataList.FindAll(room => room.column == currentRoom.column);
        foreach (var room in sameColumnRoomVector)
        {
            if(room.line != roomVector.y)
                room.roomState = RoomState.Locked;
        }

        //找到进入过的房间 连线 的房间并设置为可进入的
        foreach (var link in currentRoom.linkTo)
        {
            var linkedRoom = mapLayout.mapRoomDataList.Find(room => room.column == link.x && room.line == link.y);
            linkedRoom.roomState = RoomState.Attainable;
        }
    }
}
