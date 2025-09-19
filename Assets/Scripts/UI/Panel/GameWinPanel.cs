using System;
using UnityEngine;
using UnityEngine.UI;

public class GameWinPanel : MonoBehaviour
{
    private Button backToMapButton;

    [Header("广播")] public ObjectEventSO loadMapEvent;

    private void OnEnable()
    {
        backToMapButton = transform.Find("BackToMapButton").GetComponent<Button>();
        backToMapButton.onClick.AddListener(BackToMapButtonClicked);
    }

    private void OnDisable()
    {
        backToMapButton.onClick.RemoveAllListeners();
    }

    private void BackToMapButtonClicked()
    {
        loadMapEvent.RaiseEvent(null,this);
    }
}
