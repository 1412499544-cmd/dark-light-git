using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SceneLoadPanel : MonoBehaviour
{
    public Slider sceneLoadSlider;
    public TMP_Text sceneLoadText;

    public void SceneLoadUpdateSlider(float value)
    {
        sceneLoadSlider.value = value;
        sceneLoadText.text =$"[总进度]: {value * 100:F2}% ";
    }
}
