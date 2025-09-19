using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    private Room currentRoom;
    
    /// <summary>
    /// 房间加载后事件
    /// </summary>
    /// <param name="value">房间</param>
    public void AfterEnemyRoomLoaded(object value)
    {
        if (value is Room room)
        {
            currentRoom = room;
            if (currentRoom.roomData.roomType == RoomType.MinorEnemy)
            {
                //TODO:普通敌人配置表
            }
            else if (currentRoom.roomData.roomType == RoomType.EliteEnemy)
            {
                
            }
            
            
        }
    }
}
