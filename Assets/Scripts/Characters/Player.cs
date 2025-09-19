using UnityEngine;

public class Player : MonoBehaviour
{
    public ObjectEventSO eventSO;
    
    [ContextMenu("测试呼叫事件")]
    public void PlayerRaiseEvent()
    {
        eventSO.RaiseEvent(this,this);
    }
}
