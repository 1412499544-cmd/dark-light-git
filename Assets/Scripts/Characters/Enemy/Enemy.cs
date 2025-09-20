using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Enemy : MonoBehaviour
{
    [Header("地图位置")]
    public int column, line;    //纵,横
    
    public EnemyActionDataSO enemyData;
    
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void UpdateSelfInScene(int column, int line,EnemyActionDataSO enemyData)
    {
        this.column = column;
        this.line = line;
        this.enemyData = enemyData;
        spriteRenderer.sprite = enemyData.sprite;
    }
}
