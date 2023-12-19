using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class CameraZoom : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] float zoomPercentaje = 10f;

    CinemachineComponentBase componentBase;
    float cameraDistance;

    
    // Update is called once per frame
    void Update()
    {
        if(componentBase == null)
        {
            componentBase = virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);   
        }
        float mouseValue = Input.GetAxis("Mouse ScrollWheel");
        
        if( mouseValue != 0)
        {
            cameraDistance = mouseValue * zoomPercentaje;
            virtualCamera.m_Lens.FieldOfView -= cameraDistance;
        }
    }
}
