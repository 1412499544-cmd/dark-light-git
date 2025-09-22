using System;
using System.Collections.Generic;
using UnityEngine;

public class HexGridLayouts : Singleton<HexGridLayouts>
{
    [Header("Grid Settings")]
    //定义地图大小
    public Vector2Int gridSize;

    [Header("Tile Settings")]
    public float outerSize = 1f;
    public float innerSize = 0f;
    public float height = 1f;
    public bool isFlatTopped = true;
    public Material material;
    [Tooltip("六边形在水平方向上的额外间距")]
    public float horizontalMargin = 0.1f;
    [Tooltip("六边形在垂直方向上的额外间距")]
    public float verticalMargin = 0.1f;
    public Vector3 quaternion;

    [Header("网格列表")]
    public List<HexTileSceneData> hexTileList;
    public Dictionary<Vector2Int, HexTileSceneData> hexTileDict = new Dictionary<Vector2Int, HexTileSceneData>();

    [Header("预制体")]
    public HexRenderer hexTile;

    public SkillHexTileConfigSO skillHexTileConfigSO;

    private void OnEnable()
    {
        LayoutGrid();
    }

    private void LayoutGrid()
    {
        // 清理旧网格
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            if (Application.isPlaying)
                Destroy(transform.GetChild(i).gameObject);
            else
                DestroyImmediate(transform.GetChild(i).gameObject);
        }

        // 清理列表和字典，防止重复运行导致错误
        hexTileList.Clear();
        hexTileDict.Clear();

        float mapRadiusInUnits = gridSize.x * outerSize;
        float radiusSquared = mapRadiusInUnits * mapRadiusInUnits;
        int hexRadius = Mathf.CeilToInt(gridSize.x / (isFlatTopped ? 1f : Mathf.Sqrt(3) / 2f * 0.75f));

        for (int q = -hexRadius; q <= hexRadius; q++)
        {
            for (int r = -hexRadius; r <= hexRadius; r++)
            {
                int s = -q - r;
                if (s >= -hexRadius && s <= hexRadius)
                {
                    Vector3 hexPosition = GetPositionForHexFromAxial(q, r);

                    if (hexPosition.sqrMagnitude <= radiusSquared)
                    {
                        var tile = Instantiate(hexTile, transform);
                        tile.column = q;
                        tile.line = r;

                        tile.transform.localPosition = hexPosition;
                        tile.transform.localRotation = Quaternion.Euler(quaternion);

                        tile.m_meshRenderer.material = material;
                        // 假设你的 HexRenderer 中有 originalColor 属性
                        tile.originalColor = material.color;
                        tile.isFlatTopped = isFlatTopped;
                        tile.outerSize = outerSize;
                        tile.innerSize = innerSize;
                        tile.height = height;
                        tile.DrawMesh();

                        HexTileSceneData hexTileSceneData = new HexTileSceneData
                        {
                            column = q,
                            line = r,
                            hexRenderer = tile.GetComponent<HexRenderer>(),
                            hexPosition = new SerializeVector3(GetPositionForHexFromAxial(q, r)),
                            tileState = false
                        };
                        hexTileList.Add(hexTileSceneData);
                        
                        // 【修正 #1】将字典的键修正为 (q, r)，即 (column, line) 的顺序
                        hexTileDict.Add(new Vector2Int(q, r), hexTileSceneData);
                    }
                }
            }
        }
    }


    /// <summary>
    /// 【新的坐标计算方法】根据轴向坐标(q,r)计算本地位置。
    /// </summary>
    public Vector3 GetPositionForHexFromAxial(int q, int r)
    {
        float xPosition;
        float zPosition;
        float size = outerSize;

        if (!isFlatTopped)
        {
            // 对于尖顶六边形 (Pointy-Topped)
            float hexWidth = Mathf.Sqrt(3) * size;
            float hexHeight = 2f * size;

            float horizontalSpacing = hexWidth + horizontalMargin;
            float verticalSpacing = (hexHeight * 0.75f) + verticalMargin;

            xPosition = (q * horizontalSpacing) + (r * horizontalSpacing / 2f);
            zPosition = r * verticalSpacing;
        }
        else
        {
            // 对于平顶六边形 (Flat-Topped)
            float hexWidth = 2f * size;
            float hexHeight = Mathf.Sqrt(3) * size;

            float horizontalSpacing = (hexWidth * 0.75f) + horizontalMargin;
            float verticalSpacing = hexHeight + verticalMargin;

            xPosition = q * horizontalSpacing;
            zPosition = (r * verticalSpacing) + (q * verticalSpacing / 2f);
        }

        return new Vector3(xPosition, 0, zPosition);
    }

    //更新网格显示
    public void UpdateTilesState(int column, int line, bool isOpen)
    {
        Vector2Int key = new Vector2Int(column, line);
        if (hexTileDict.ContainsKey(key))
        {
            var hexTile = hexTileDict[key];
            hexTile.tileState = isOpen;
            hexTile.hexRenderer.m_meshFilter.mesh = isOpen ? hexTile.hexRenderer.m_mesh : null;
        }
    }

    /// <summary>
    /// 得到每个网格的另据网格
    /// </summary>
    [ContextMenu("更新获取周围瓦片")]
    public void UpdateHexTilesNeighbours()
    {
        foreach (var hexTile in hexTileList)
        {
            List<HexRenderer> neighbours = GetSkillHexTiles(hexTile.hexRenderer);
            hexTile.hexRenderer.neighbours = neighbours;
        }
    }

    /// <summary>
    /// 获取与网格相连的几个格子
    /// </summary>
    /// <param name="hexTile"></param>
    /// <returns></returns>
    private List<HexRenderer> GetNeighbours(HexRenderer hexTile)
    {
        List<HexRenderer> neighbours = new List<HexRenderer>();
        
        //网格方向的格子
        Vector2Int[] neighbourCoords = new Vector2Int[]
        {
            new Vector2Int(1, 0),   // 右下
            new Vector2Int(1, -1),  // 右上
            new Vector2Int(0, -1),  // 上
            new Vector2Int(-1, 0),  // 左上
            new Vector2Int(-1, 1),  // 左下
            new Vector2Int(0, 1),   // 下
            new Vector2Int(-2, 1),  //(-2,1),(-4,2),(-6,3) 左
            new Vector2Int(2, -1)   //(2,-1),(4,-2),(6,-3) 右
        };

        // 获取当前六边形的坐标 (q, r)
        Vector2Int currentCoord = new Vector2Int(hexTile.column, hexTile.line);

        // 遍历邻居坐标，并检查是否存在于字典中
        foreach (var offset in neighbourCoords)
        {
            Vector2Int neighbourPosition = currentCoord + offset;

            // 现在这里的 neighbourPosition 是 (q, r)，与字典的键一致，可以正确查找到
            if (hexTileDict.ContainsKey(neighbourPosition))
            {
                neighbours.Add(hexTileDict[neighbourPosition].hexRenderer);
            }
        }
        return neighbours;
    }
    
    /// <summary>
    /// 获取技能范围的格子
    /// </summary>
    /// <param name="hexTile">网格</param>
    /// <returns></returns>
    private List<HexRenderer> GetSkillHexTiles(HexRenderer hexTile)
    {
        List<HexRenderer> neighbours = new List<HexRenderer>();
        
        //网格方向的格子
        List<Vector2Int> neighbourCoords = new();
        AddHexTileNeighbours(skillHexTileConfigSO,neighbourCoords);
        
        // 获取当前六边形的坐标 (q, r)
        Vector2Int currentCoord = new Vector2Int(hexTile.column, hexTile.line);

        // 遍历邻居坐标，并检查是否存在于字典中
        foreach (var offset in neighbourCoords)
        {
            Vector2Int neighbourPosition = currentCoord + offset;

            // 现在这里的 neighbourPosition 是 (q, r)，与字典的键一致，可以正确查找到
            if (hexTileDict.ContainsKey(neighbourPosition))
            {
                neighbours.Add(hexTileDict[neighbourPosition].hexRenderer);
            }
        }
        return neighbours;
    }
    
    private void AddHexTileNeighbours(SkillHexTileConfigSO hexTileConfig, List<Vector2Int> neighbourCoords)
    {
        for (int i = 0; i < hexTileConfig.addNumber+1; i++)
        {
            neighbourCoords.Add(hexTileConfig.skillHexTile.up *i + hexTileConfig.skillHexTile.Up);
            neighbourCoords.Add(hexTileConfig.skillHexTile.down *i + hexTileConfig.skillHexTile.Down);
            neighbourCoords.Add(hexTileConfig.skillHexTile.left *i + hexTileConfig.skillHexTile.Left);
            neighbourCoords.Add(hexTileConfig.skillHexTile.right *i + hexTileConfig.skillHexTile.Right);
            neighbourCoords.Add(hexTileConfig.skillHexTile.leftUp *i + hexTileConfig.skillHexTile.LeftUp);
            neighbourCoords.Add(hexTileConfig.skillHexTile.leftDown *i + hexTileConfig.skillHexTile.LeftDown);
            neighbourCoords.Add(hexTileConfig.skillHexTile.rightUp *i + hexTileConfig.skillHexTile.RightUp);
            neighbourCoords.Add(hexTileConfig.skillHexTile.rightDown *i + hexTileConfig.skillHexTile.RightDown);
        }
    }
}

[System.Serializable]
public class HexTileSceneData
{
    public HexRenderer hexRenderer;
    public int column, line;
    public SerializeVector3 hexPosition;
    public bool tileState;
}