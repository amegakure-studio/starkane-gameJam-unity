using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class CameraZoom : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] float zoomPercentaje = 10f;
    private float cameraDistance;

    
    // Update is called once per frame
    void Update()
    {
        float mouseValue = Input.GetAxis("Mouse ScrollWheel");
        
        if(mouseValue != 0)
        {
            cameraDistance = mouseValue * zoomPercentaje;
            float actualZoomValue = virtualCamera.m_Lens.FieldOfView - cameraDistance;

            if(actualZoomValue >= 10 && actualZoomValue <= 100 )
            {
                virtualCamera.m_Lens.FieldOfView = actualZoomValue;
            }
        }
    }
}
