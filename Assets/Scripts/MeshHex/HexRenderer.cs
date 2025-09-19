using System;
using System.Collections.Generic;
using UnityEngine;

//容器，映射每个网格的面
public struct Face
{
    //存储网格顶点
    public List<Vector3> vertices {get;private set; }
    //存储uv信息
    public List<Vector2>  uvs {get;private set; }
    //存储三角面
    public List<int> triangles {get;private set; }
    public Face(List<Vector3> vertices, List<Vector2> uvs, List<int> triangles)
    {
        this.vertices = vertices;
        this.uvs = uvs;
        this.triangles = triangles;
    }
}

//网格
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class HexRenderer : MonoBehaviour
{
    public Mesh m_mesh;
    public MeshFilter m_meshFilter;
    public MeshRenderer m_meshRenderer;
    
    private List<Face> m_faces = new List<Face>();
    
    public float innerSize;
    public float outerSize;
    public float height;
    public int column, line;
    
    //设置是否平顶六边形
    public bool isFlatTopped;

    private void Awake()
    {
        m_meshFilter =  GetComponent<MeshFilter>();
        m_meshRenderer = GetComponent<MeshRenderer>();

        m_mesh = new();
        m_mesh.name = "Hex";
        
        m_meshFilter.mesh = m_mesh;
    }

    private void OnEnable()
    {
        DrawMesh();
    }

    public void DrawMesh()
    {
        DrawFaces();
        CombineFaces();
    }

    //将面合并到网格
    private void CombineFaces()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();

        for (int i = 0; i < m_faces.Count; i++)
        {
            //添加网格顶点
            //AddRange 方法用于将一个集合的所有元素添加到另一个 List<T> 的末尾
            vertices.AddRange((m_faces[i].vertices));
            uvs.AddRange((m_faces[i].uvs));
            
            //为三角面添加偏移量
            int offset = 4 * i;
            foreach (var triangles in m_faces[i].triangles)
            {
                tris.Add(offset + triangles);
            }
        }
        
        //将网格值添加到mesh
        m_mesh.vertices = vertices.ToArray();
        m_mesh.uv = uvs.ToArray();
        m_mesh.triangles = tris.ToArray();
        m_mesh.RecalculateNormals();
    }

    private void DrawFaces()
    {
        m_faces = new List<Face>();
        
        //六边形六个顶点 即六个面
        for (int point = 0; point < 6; point++)
            m_faces.Add(CreateFace(innerSize,outerSize,height / 2f,height/2f,point));
    }

    /// <summary>
    /// 创建六边形的面
    /// </summary>
    /// <param name="innerRad">内弧度</param>
    /// <param name="outerRad">外弧度</param>
    /// <param name="heightA">大小</param>
    /// <param name="heightB">大小</param>
    /// <param name="point">当前点索引</param>
    /// <param name="reverse">是否反转所有值</param>
    /// <returns></returns>
    private Face CreateFace(float innerRad, float outerRad, float heightA, float heightB, int point,
        bool reverse = false)
    {
        Vector3 pointA = GetPoint(innerRad, heightB,point);
        Vector3 pointB = GetPoint(innerRad, heightB, (point < 5) ? point+1:0);
        Vector3 pointC = GetPoint(outerRad, heightA, (point < 5) ? point+1:0);
        Vector3 pointD = GetPoint(outerRad, heightA, point);
        
        List<Vector3> vertices = new List<Vector3>(){pointA, pointB, pointC,pointD};
        List<int> triangles = new List<int>(){0,1,2,2,3,0};
        List<Vector2> uvs = new List<Vector2>(){new Vector2(0,0),new Vector2(1,0),new Vector2(1,1),new Vector2(0,1)};

        if (reverse)
            vertices.Reverse();
        
        return new Face(vertices, uvs, triangles);
    }
    
    /// <summary>
    /// 计算顶点位置
    /// </summary>
    /// <param name="size"> 半径</param>
    /// <param name="height">与平面距离</param>
    /// <param name="index">六边形角的索引</param>
    /// <returns></returns>
    protected Vector3 GetPoint(float size,float height,int index)
    {
        
        //六个内角角度之和为360°
        float angle_deg = isFlatTopped ? 60 * index : 60*index - 30;
        //弧度计算公式
        float angle_rad = Mathf.PI / 180f * angle_deg;
        
        return new Vector3(size * Mathf.Cos(angle_rad),height, size * Mathf.Sin(angle_rad));
    }
}
