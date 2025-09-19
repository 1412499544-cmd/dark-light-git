using System;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("地图位置")]
    public int column, line;    //纵,横
    
    [Header("房间基本参数")]
    private SpriteRenderer spriteRenderer;   //直接获取Renderer保存sprite
    public RoomDataSO roomData;
    public RoomState roomState;

    [Header("地图连线存储连线房间位置")] 
    public List<Vector2Int> linkTo = new List<Vector2Int>();

    [Header("广播")]
    //点击房间进入房间加载房间
    public ObjectEventSO LoadRoomEvent;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    
    /// <summary>
    /// 对有碰撞体的GameObject触发
    /// </summary>
    private void OnMouseDown()
    {
        if(roomState == RoomState.Attainable)
            LoadRoomEvent.RaiseEvent(this,this);
    }

    /// <summary>
    /// 建立房间函数 创建房间时初始化
    /// </summary>
    /// <param name="column">房间纵向 x Index</param>
    /// <param name="line">房间横向 y Index</param>
    /// <param name="roomData">房间数据</param>
    public void SetUpRoom(int column, int line, RoomDataSO roomData)
    {
        this.column = column;
        this.line = line;
        this.roomData = roomData;
        spriteRenderer.sprite = roomData.roomSprite;
        spriteRenderer.color = roomState switch
        {
            RoomState.Locked => new Color(0.5f, 0.5f, 0.5f, 1f),
            RoomState.Visited => new Color(0.5f, 0.8f, 0.5f, 0.5f),
            RoomState.Attainable => Color.white,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
