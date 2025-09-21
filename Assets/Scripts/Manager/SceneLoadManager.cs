using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class SceneLoadManager : MonoBehaviour
{
    [Header("场景")]
    //Addressable场景
    public AssetReference currentScene;
    public AssetReference map;
    
    [Header("基本参数")]
    public Room currentRoom;
    public FadePanel fadePanel;
    public Vector2Int currentMapRoomVector;

    public float totalLoadProgress;
    [SerializeField] private float visualLoadProgress = 5f;


    [Header("广播")] 
    public ObjectEventSO updateRoomEvent;

    public ObjectEventSO afterEnemyRoomLoaded;
    public FloatEventSO updateSceneLoadPanelEvent;
    
    private void Awake()
    {
        currentMapRoomVector =  Vector2Int.one * -1;
    }

    /// <summary>
    /// 加载房间事件
    /// </summary>
    /// <param name="value">RoomPrefab</param>
    public async void LoadRoomEvent(object value)
    {
        if (value is Room room)
        {
            currentRoom = room;
            currentMapRoomVector = new Vector2Int(room.column, room.line);
            currentScene = currentRoom.roomData.sceneToLoad;
        }

        await UnLoadSceneTask();
        await LoadSceneTask();

        if (currentRoom.roomData.roomType == RoomType.MinorEnemy ||
            currentRoom.roomData.roomType == RoomType.EliteEnemy)
        {
            //Enemy房间被加载之后事件 获取场景内所有敌人添加到活着的敌人列表
            afterEnemyRoomLoaded.RaiseEvent(currentRoom,this);
        }
    }

    #region 场景加载卸载

    private async UniTask LoadSceneTask()
    {
        float progressA = 0f;
        float progressB = 0f;
        
        //加载场景进程
        var loadOperationProgress = Progress.Create<float>(p => { progressA = p; });

        //虚拟加载进程
        var virtualAnimationProgress = Progress.Create<float>(p => { progressB = p; });
        
        UniTask loadAnimAsync = LoadAnimationAsync(visualLoadProgress, virtualAnimationProgress);
    
        //LoadSceneMode.Additive叠加式场景加载 直接在现有场景上加载下个场景
        var loadOperation = currentScene.LoadSceneAsync(LoadSceneMode.Additive,activateOnLoad:false);
        
        while (!loadOperation.IsDone || !loadAnimAsync.Status.IsCompleted())
        {
            // 每当场景进度更新，就重新计算总进度
            loadOperationProgress?.Report(loadOperation.PercentComplete);
            totalLoadProgress = Mathf.Lerp(totalLoadProgress, (progressA+progressB)/2, Time.deltaTime * 5f);
            updateSceneLoadPanelEvent.RaiseEvent(totalLoadProgress,this);
            await UniTask.Yield();
        }
        
        totalLoadProgress = 1f; 
        //延时执行
        updateSceneLoadPanelEvent.RaiseEvent(totalLoadProgress,this);
        
        await loadOperation.Result.ActivateAsync();
        SceneManager.SetActiveScene(loadOperation.Result.Scene);
        await WaitForFade(false);
    }
    
    // 一个模拟加载的通用方法
    private async UniTask LoadAnimationAsync(float duration, IProgress<float> progress)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            progress?.Report(Mathf.Clamp01(timer / duration));
            await UniTask.Yield(); // 等待下一帧
        }
        //Debug.Log("[其他进程]: 模拟完成。");
    }

    private async UniTask UnLoadSceneTask()
    {
        await WaitForFade(true);
        await SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        fadePanel.Hide();
    }

    private async UniTask WaitForFade(bool fadeInState)
    {
        var fadeTween = fadeInState ? fadePanel.FadeIn(0.5f) : fadePanel.FadeOut(0.5f);
        if (fadeInState)
            await fadeTween.AsyncWaitForCompletion();
        
    }
    
    #endregion

    /// <summary>
    /// 战斗结束后事件调用加载地图事件
    /// </summary>
    public async void LoadMap(object value)
    {
        if(currentScene != null)
            await UnLoadSceneTask();

        if (currentMapRoomVector != Vector2Int.one * -1)
        {
            updateRoomEvent.RaiseEvent(currentMapRoomVector,this);
        }
        
        currentScene = map;
        await LoadSceneTask();
    }

}
