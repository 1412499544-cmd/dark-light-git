using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillHexTileConfigSO",menuName = "Room/SkillHexTileConfigSO")]
public class SkillHexTileConfigSO : ScriptableObject
{
    public SkillHexTile skillHexTile;
    public int addNumber;
}

[System.Serializable]
public struct SkillHexTile
{
    [SerializeField]private int up, down, left, right, rightUp, leftDown, leftUp,rightDown;
    public Vector2Int Up => new Vector2Int(0, -1) * up;
    public Vector2Int Down => new Vector2Int(0, 1) * down;
    public Vector2Int Right => new Vector2Int(2, -1) * right;
    public Vector2Int Left => new Vector2Int(-2, 1) * left;
    public Vector2Int RightUp => new Vector2Int(1, -1) * rightUp;
    public Vector2Int RightDown => new Vector2Int(1, 0) * rightDown;
    public Vector2Int LeftUp => new Vector2Int(-1,0) * leftUp;
    public Vector2Int LeftDown => new Vector2Int(-1, 1) * leftDown;
}
