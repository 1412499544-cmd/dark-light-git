using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillHexTileConfigSO",menuName = "Room/SkillHexTileConfigSO")]
public class SkillHexTileConfigSO : ScriptableObject
{
    public SkillHexTile skillHexTile;
    public int addNumber;
}

[System.Serializable]
public class SkillHexTile
{
    [SerializeField] private int upMultiply, downMultiply, leftMultiply, rightMultiply, rightUpMultiply, leftDownMultiply, leftUpMultiply, rightDownMultiply;

    [HideInInspector]public Vector2Int up = new Vector2Int(0, -1);
    public Vector2Int Up
    {
        get => up * upMultiply;
        set => up = value;
    }
    
    [HideInInspector]public Vector2Int down = new Vector2Int(0, 1);
    public Vector2Int Down
    {
        get => down * downMultiply;
        set => down = value;
    }
    
    [HideInInspector]public Vector2Int right = new Vector2Int(2, -1);
    public Vector2Int Right
    {
        get => right * rightMultiply;
        set => right = value;
    }
    
    [HideInInspector]public Vector2Int left = new Vector2Int(-2, 1);
    public Vector2Int Left
    {
        get => left * leftMultiply;
        set => left = value;
    }
    
    [HideInInspector]public Vector2Int rightUp = new Vector2Int(1, -1);
    public Vector2Int RightUp
    {
        get => rightUp * rightUpMultiply;
        set => rightUp = value;
    }
    
    [HideInInspector]public Vector2Int rightDown = new Vector2Int(1, 0);
    public Vector2Int RightDown
    {
        get => rightDown * rightDownMultiply;
        set => rightDown = value;
    }
    
    [HideInInspector]public Vector2Int leftUp = new Vector2Int(-1,0);
    public Vector2Int LeftUp
    {
        get => leftUp * leftUpMultiply;
        set => leftUp = value;
    }
    
    [HideInInspector]public Vector2Int leftDown = new Vector2Int(-1, 1);
    public Vector2Int LeftDown
    {
        get => leftDown * leftDownMultiply;
        set => leftDown = value;
    }
}
