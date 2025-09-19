using System;

/// <summary>
/// 房间类型 按2的幂次
/// </summary>
[Flags]
public enum RoomType
{
    MinorEnemy = 1,
    EliteEnemy = 2,
    Shop = 4,
    PlayRoom = 8,
    EventRoom = 16,
    BonusRoom = 32,
}

public enum EnemyType
{
    MinorEnemy,
    EliteEnemy,
    MinorBoss,
    EliteBoss,
    FinalBoss,
}

public enum RoomState
{
    //可访问
    Attainable,
    //已经进入
    Visited,
    //无法进入
    Locked
}

