using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    private Camera m_camera;

    private void Awake()
    {
        m_camera = GetComponent<Camera>();
    }
    
    private void Update()
    {
        RaycastHit hit;
        var ray = m_camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            Transform objHit = hit.transform;
            
            if (objHit.TryGetComponent<HexRenderer>(out HexRenderer target))
            {
                //target.OnHighlightTile();
            }
        }
    }
}
