using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject gameWinPanel;
    public GameObject sceneLoadPanel;
    
    /// <summary>
    /// 游戏胜利加载地图事件
    /// </summary>
    public void LoadMapEvent(object value)
    {
        gameWinPanel.SetActive(false);
    }

    public void LoadSceneEvent(float value)
    {
        sceneLoadPanel.SetActive(!(value >= 1));
    }
}
