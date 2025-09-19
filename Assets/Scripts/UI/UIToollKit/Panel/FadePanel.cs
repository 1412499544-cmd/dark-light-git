using System;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class FadePanel : MonoBehaviour
{
    private UIDocument fadePanelDocument;
    private VisualElement fadePanel;
    
    private void Awake()
    {
        fadePanelDocument = GetComponent<UIDocument>();
        var root = fadePanelDocument.rootVisualElement;
        fadePanel = root.Q<VisualElement>("FadePanel");

        FadeOut(0.5f);
    }

    public Tween FadeIn(float duration)
    {
        // 确保动画是从不透明开始
        fadePanel.style.opacity = 0;
        fadePanel.style.display = DisplayStyle.Flex;
        
        return DOVirtual.Float(0,1, duration, value =>
        {
            if(fadePanel!=null)
                fadePanel.style.opacity = value;
        }).SetEase(Ease.InQuad).SetTarget(this);
    }

    public Tween FadeOut(float duration)
    {
        
        // 确保动画是从不透明开始
        fadePanel.style.opacity = 1;
        fadePanel.style.display = DisplayStyle.Flex;
        
        return DOVirtual.Float(1,0, duration, value =>
        {
            if(fadePanel!=null)
                fadePanel.style.opacity = value;
        }).SetEase(Ease.InQuad).SetTarget(this).OnComplete(() =>
        {
            // 【关键】动画结束后，通过样式彻底隐藏它
            fadePanel.style.display = DisplayStyle.None; 
        });
    }
    
    public void Hide()
    {
        if(fadePanel == null) return;
        fadePanel.style.display = DisplayStyle.None;
    }
}
