using System;
using UnityEngine;
using UnityEngine.PlayerLoop;
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

    [Header("广播")] public ObjectEventSO highlightHexRenderer;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // protected override void Update()
    // {
    //     base.Update();
    // }

    private void OnMouseDown()
    {
        highlightHexRenderer.RaiseEvent(this,this);
        Debug.Log("点击了:"+this.gameObject.name);
    }

    public void SetUpEnemy(int column, int line,EnemyDataSO enemyDataSO)
    {
        this.column = column;
        this.line = line;
        this.enemyDataSO = enemyDataSO;
        enemyType = enemyDataSO.enemyType;
        spriteRenderer.sprite = enemyDataSO.sprite;
        var randomIndex = Random.Range(0, enemyDataSO.actions.Count);
        //TODO:action数量更改
        //currentAction = enemyDataSO.actions[randomIndex];
    }
}
