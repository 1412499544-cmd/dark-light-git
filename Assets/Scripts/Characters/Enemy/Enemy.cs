using System;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(SpriteRenderer))]
public class Enemy : CharacterBase
{
    [Header("地图位置")]
    public int column, line;    //纵,横

    [Header("敌人SO")]
    public EnemyDataSO enemyDataSO;
    
    public EnemyAction currentAction;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetUpEnemy(int column, int line,EnemyAction enemyAction,EnemyDataSO enemyDataSO)
    {
        this.column = column;
        this.line = line;
        var randomIndex = Random.Range(0, enemyDataSO.actions.Count);
        currentAction = enemyDataSO.actions[randomIndex];
        this.enemyDataSO = enemyDataSO;
    }
}
