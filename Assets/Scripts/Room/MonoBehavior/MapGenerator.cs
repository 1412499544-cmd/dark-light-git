using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    [Header("地图配置表")]
    public MapConfigSO mapConfig;
    
    [Header("地图布局")]
    public MapLayoutSO mapLayout;
    
    [Header("预制体")]
    public Room roomPrefab;
    public LineRenderer linePrefab;

    [Header("基本参数")] 
    private float screenWidth; 
    private float screenHeight;
    private float columnWidth;   //由蓝图Count【列数】决定宽度
    public float border;
    private Vector3 generatePoint;   //房间生成点
    
    public List<Room> rooms = new();
    public List<LineRenderer> lines = new();
    public List<RoomDataSO> roomDataList = new();
    private Dictionary<RoomType,RoomDataSO> roomDataDict = new();

    private void Awake()
    {
        //Camera.main.orthographicSize为屏幕高度一半
        screenHeight = Camera.main.orthographicSize*2;
        //Camera.main.aspect为屏幕宽高比
        screenWidth = Camera.main.aspect*screenHeight;
        //房间列宽
        columnWidth = screenWidth / mapConfig.roomBluePrints.Count;
        
        foreach (var roomData in roomDataList)
        {
            roomDataDict.Add(roomData.roomType, roomData);
        }
    }

    private void OnEnable()
    {
        if(mapLayout.mapRoomDataList.Count>0)
            LoadMap();
        else
            CreateMap();
    }

    private void CreateMap()
    {
        //记录前一列房间 用于与后一列连线
        List<Room> previousColumnRooms = new List<Room>();
        
        for (int column = 0; column < mapConfig.roomBluePrints.Count; column++)
        {
            //获取房间蓝图 随机一列房间数量
            var bluePrint = mapConfig.roomBluePrints[column];
            var amount = Random.Range(bluePrint.min, bluePrint.max);
            
            //记录当前列房间列表
            List<Room> currentColumnRooms = new List<Room>();
            
            //设置房间起始点高度
            var startPointHeight = screenHeight/2 - screenHeight/ (amount + 1);
            //设置上下房间间隙
            var roomGapY = screenHeight / (amount + 1);
            
            //设置各房间生成点
            generatePoint = new Vector3(-screenWidth/2+border + column*columnWidth,startPointHeight,0);
            //记录生成点便于在创建时修改特殊点位置
            var newPosition = generatePoint;
            
            //循环当前列房间个数生成房间
            for (int i = 0; i < amount; i++)
            {
                if (column == mapConfig.roomBluePrints.Count - 1)
                {
                    //如果是最后一列,boss房间固定位置
                    newPosition.x = screenWidth / 2 - border;
                }
                else if (column != 0)
                {
                    //每个房间偏移
                    newPosition.x = generatePoint.x + Random.Range(-border / 4,border/4);
                }
                
                newPosition.y = startPointHeight - i*roomGapY;
                
                //创建房间并初始化 transform为生成位置
                var room = Instantiate(roomPrefab, newPosition, Quaternion.identity,transform);
                var roomType = GetRandomRoomType(mapConfig.roomBluePrints[column].roomType);

                //只有刚创建第一列可获取
                room.roomState = column == 0 ? RoomState.Attainable : RoomState.Locked;
                
                room.SetUpRoom(column,i,roomDataDict[roomType]);
                
                //添加到房间列表debug，添加当前列房间
                rooms.Add(room);
                currentColumnRooms.Add(room);
            }
            
            //判断当前列是否为第一列，如果不是则连接上一列
            if (previousColumnRooms.Count > 0)
            {
                //连线
                CreateConnections(previousColumnRooms,currentColumnRooms);
            }
            
            previousColumnRooms = currentColumnRooms;
        }
        
        SaveMap();
    }

    //随机获取房间类型
    private RoomType GetRandomRoomType(RoomType flags)
    {
        if ((int)flags == -1)
        {
            var array = Enum.GetValues(typeof(RoomType));
            int randomIndex = Random.Range(0, array.Length); // 生成随机索引
            return (RoomType)array.GetValue(randomIndex); // 获取随机元素
        }
        else
        {
            string[] options = flags.ToString().Split(',');
            string randomOption = options[Random.Range(0, options.Length)];
            return Enum.Parse<RoomType>(randomOption);
        }
    }
    
    // private RoomDataSO GetRoomData(RoomType roomType)
    // {
    //     return roomDataDict[roomType];
    // }

    /// <summary>
    /// 创建连线
    /// </summary>
    /// <param name="previousRooms">先前列房间</param>
    /// <param name="currentRooms">当前列房间</param>
    private void CreateConnections(List<Room> previousRooms,List<Room> currentRooms)
    {
        //当前已经被连接的房间列表
        HashSet<Room> currentConnectedRooms = new HashSet<Room>();

        //连接房间并添加到已连接房间列表
        foreach (var room in previousRooms)
        {
            var targetRoom = ConnectRamdomRoom(room,currentRooms,false);
            currentConnectedRooms.Add(targetRoom);
        }
        
        //为当前列未连线的房间 反向连线
        foreach (var room in currentRooms)
        {
            if(!currentConnectedRooms.Contains(room))
                ConnectRamdomRoom(room,previousRooms,true);
        }
    }

    /// <summary>
    /// 随机连线连接房间
    /// </summary>
    /// <param name="room">先前列房间</param>
    /// <param name="currentColumnRooms">当前列房间</param>
    /// <param name="reverse">是否反转当前房间和先前房间位置</param>
    /// <returns></returns>
    private Room ConnectRamdomRoom(Room room,List<Room>currentColumnRooms,bool reverse)
    {
        var targetRoom = currentColumnRooms[Random.Range(0, currentColumnRooms.Count)];
        
        if (reverse)
            targetRoom.linkTo.Add(new Vector2Int(room.column,room.line));
        else
            room.linkTo.Add(new Vector2Int(targetRoom.column,targetRoom.line));

        var line = Instantiate(linePrefab, transform);
        line.SetPosition(0,room.transform.position);
        line.SetPosition(1,targetRoom.transform.position);
        
        lines.Add(line);

        return targetRoom;
    }

    [ContextMenu("重新生成房间")]
    public void ReGenerateRooms()
    {
        foreach (var room in rooms)
            Destroy(room.gameObject);

        foreach (var line in lines)
            Destroy(line.gameObject);
        
        rooms.Clear();
        lines.Clear();
        
        CreateMap();
    }

    private void SaveMap()
    {
        mapLayout.mapRoomDataList = new List<MapRoomData>();
        foreach (var room in rooms)
        {
            var mapRoomData = new MapRoomData
            {
                posX = room.transform.position.x,
                posY = room.transform.position.y,
                column = room.column,
                line = room.line,
                roomData = room.roomData,
                roomState = room.roomState,
                linkTo = room.linkTo,
            };
            mapLayout.mapRoomDataList.Add(mapRoomData);
        }

        mapLayout.linePositionList =  new List<LinePosition>();
        foreach (var line in lines)
        {
            var linePosition = new LinePosition()
            {
                startPos = new SerializeVector3(line.GetPosition(0)),
                endPos = new SerializeVector3(line.GetPosition(1)),
            };
            mapLayout.linePositionList.Add(linePosition);
        }
    }

    private void LoadMap()
    {
        foreach (var mapRoomData in mapLayout.mapRoomDataList)
        {
            var newPosition = new Vector3(mapRoomData.posX, mapRoomData.posY, 0);
            var room = Instantiate(roomPrefab, newPosition, Quaternion.identity, transform);
            room.roomState = mapRoomData.roomState;
            room.SetUpRoom(mapRoomData.column, mapRoomData.line, mapRoomData.roomData);
            room.linkTo = mapRoomData.linkTo;
            rooms.Add(room);
        }

        foreach (var linePosition in mapLayout.linePositionList)
        {
            var line = Instantiate(linePrefab, transform);
            line.SetPosition(0, linePosition.startPos.ToVector3());
            line.SetPosition(1, linePosition.endPos.ToVector3());
            lines.Add(line);
        }
    }
}
