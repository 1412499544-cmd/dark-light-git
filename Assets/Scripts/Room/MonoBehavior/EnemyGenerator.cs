using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    [Header("地图生成物体配置")]
    public EnemyConfigSO MinorEnemyConfigSO;

    [Header("预制体")] public Enemy enemyPrefab;

    public List<EnemyDataSO> enemyDataList = new();
    public Dictionary<EnemyType, EnemyDataSO> enemyDataDict = new();
    
    //[Header("广播")]
    //public ObjectEventSO 

    private void Awake()
    {
        foreach (var enemyData in enemyDataList)
        {
            enemyDataDict.Add(enemyData.enemyType,enemyData);
        }
    }

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
                    
                    //var enemy = Instantiate(enemyPrefab,, Quaternion.identity, transform);
                    //enemy.SetUpEnemy(enemyBluePrint.column,enemyBluePrint.line,enemyDataDict[enemyBluePrint.enemyType]);
                }
            }
            else if (room.roomData.roomType == RoomType.EliteEnemy)
            {
                
            }
            
            
        }
    }
}
