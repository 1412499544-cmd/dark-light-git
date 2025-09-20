using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    [Header("地图生成物体配置")]
    public EnemyConfigSO MinorEnemyConfigSO;

    [Header("预制体")] public Enemy enemy;
    
    //[Header("广播")]
    //public ObjectEventSO 
    
    /// <summary>
    /// 房间加载后事件
    /// </summary>
    /// <param name="value">房间</param>
    public void AfterEnemyRoomLoaded(object value)
    {
        if (value is Room room)
        {
            if (room.roomData.roomType == RoomType.MinorEnemy)
            {
                foreach (var enemyBluePrint in MinorEnemyConfigSO.enemyBluePrints)
                {
                    //TODO:更新地图，生成敌人
                    
                }
            }
            else if (room.roomData.roomType == RoomType.EliteEnemy)
            {
                
            }
            
            
        }
    }
}
