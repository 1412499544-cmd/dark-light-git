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
    public bool isFlatTopped = false;
    public Material material;
    [Tooltip("六边形在水平方向上的额外间距")]
    public float horizontalMargin = 0.1f;
    [Tooltip("六边形在垂直方向上的额外间距")]
    public float verticalMargin = 0.1f;
    public Vector3 quaternion;

    [Header("网格列表")] 
    public List<HexTileSceneData> hexTiles;
    
    [Header("预制体")]
    public HexRenderer hexTile;
    
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

        // gridSize.x 解释为圆形地图的“半径”
        // 这个半径现在是物理单位（Unity Units），而不是格子数量
        float mapRadiusInUnits = gridSize.x * outerSize;

        // 为了效率，我们计算半径的平方，以避免在循环中
        float radiusSquared = mapRadiusInUnits * mapRadiusInUnits;

        // 循环遍历一个足以包裹住整个圆形的六边形区域
        // 我们需要一个比物理半径稍大的格子半径来确保覆盖
        int hexRadius = Mathf.CeilToInt(gridSize.x / (isFlatTopped ? 1f : Mathf.Sqrt(3) / 2f * 0.75f));

        for (int q = -hexRadius; q <= hexRadius; q++)
        {
            for (int r = -hexRadius; r <= hexRadius; r++)
            {
                int s = -q - r;
                if (s >= -hexRadius && s <= hexRadius)
                {
                    // 1. 【先计算】出这个六边形将会被放置的物理位置
                    Vector3 hexPosition = GetPositionForHexFromAxial(q, r);

                    // 2. 【再判断】这个位置到中心的距离是否在圆形半径内
                    //    我们使用 .sqrMagnitude 来比较距离的平方，这比开方根(.magnitude)更高效
                    if (hexPosition.sqrMagnitude <= radiusSquared)
                    {
                        // 3. 【最后生成】只有在圆形内的六边形才会被创建
                        var tile = Instantiate(hexTile,transform);
                        tile.column = q;
                        tile.line = r;

                        // 使用我们已经计算好的位置
                        tile.transform.localPosition = hexPosition;
                        tile.transform.localRotation = Quaternion.Euler(quaternion);
                        
                        tile.m_meshRenderer.material = material;
                        tile.originalColor = material.color;
                        tile.isFlatTopped = isFlatTopped;
                        tile.outerSize = outerSize;
                        tile.innerSize = innerSize;
                        tile.height = height;
                        tile.DrawMesh();
                        tile.m_meshFilter.mesh = null;
                        
                        HexTileSceneData hexTileSceneData = new HexTileSceneData
                        {
                            column = q,
                            line = r,
                            hexRenderer = tile.GetComponent<HexRenderer>(),
                            hexPosition = new SerializeVector3(GetPositionForHexFromAxial(q, r)),
                            tileState = false
                        };
                        hexTiles.Add(hexTileSceneData);
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
    public void UpdateTilesState(int column, int line,bool isOpen)
    {
        foreach (var hexTile in hexTiles)
        {
            if (hexTile.column == column && hexTile.line == line && isOpen == true)
            {
                hexTile.tileState = true;
                hexTile.hexRenderer.m_meshFilter.mesh = hexTile.hexRenderer.m_mesh;
            }
            else if (hexTile.column == column && hexTile.line == line && isOpen == false)
            {
                hexTile.tileState = false;
                hexTile.hexRenderer.m_meshFilter.mesh = null;
            }
        }
    }

    /// <summary>
    /// 得到每个网格的另据网格
    /// </summary>
    public void UpdateHexTilesNeighbours()
    {
        foreach (var hexTile in hexTiles)
        {
            //TODO:List<HexRenderer> neighbours = GetNeighbours(hexTile.hexRenderer);
            //hexTile.hexRenderer.neighbours = neighbours;
        }
    }

    // private List<HexRenderer> GetNeighbours(HexRenderer hexTile)
    // {
    //     List<HexRenderer> neighbours = new List<HexRenderer>();
    //     
    //     
    //     
    // }
}

[System.Serializable]
public class HexTileSceneData
{
    public HexRenderer hexRenderer;
    public int column, line;
    public SerializeVector3 hexPosition;
    public bool tileState;
}

    
