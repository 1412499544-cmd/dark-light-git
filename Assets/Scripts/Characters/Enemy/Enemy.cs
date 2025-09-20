using System;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(SpriteRenderer))]
public class Enemy : CharacterBase
{
    [Header("地图位置")]
    public int column, line;    //纵,横

    [Header("敌人配置")]
    public EnemyDataSO enemyDataSO;
    public EnemyType enemyType;
    public EnemyAction currentAction;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetUpEnemy(int column, int line,EnemyDataSO enemyDataSO)
    {
        this.column = column;
        this.line = line;
        this.enemyDataSO = enemyDataSO;
        enemyType = enemyDataSO.enemyType;
        var randomIndex = Random.Range(0, enemyDataSO.actions.Count);
        currentAction = enemyDataSO.actions[randomIndex];
    }
}
