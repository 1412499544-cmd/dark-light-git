using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyConfigSO", menuName = "Room/EnemyConfigSO")]
public class EnemyConfigSO : ScriptableObject
{
    public List<EnemyBluePrint> enemyBluePrints;
}

[System.Serializable]
public class EnemyBluePrint
{
    public EnemyType enemyType;
    //public EnemyDataSO enemyData;
    public float posX, posY;
    public int column, line;
}
