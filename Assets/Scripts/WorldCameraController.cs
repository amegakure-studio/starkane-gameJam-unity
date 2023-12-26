using System.Collections;
using System.Collections.Generic;
using Amegakure.Starkane.Entities;
using Cinemachine;
using UnityEngine;

public class WorldCameraController : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;
    void Start()
    {
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        GameObject[] playerGoList = GameObject.FindGameObjectsWithTag("Player");
        
        foreach( GameObject playerGo in playerGoList)
        {
            try
            {
                playerGo.TryGetComponent<Player>(out Player player);
                
                if(player != null)
                {
                    GameObject characterGo = player.CharacterDictionary[player.DefaultCharacter];
                    virtualCamera.LookAt = characterGo.transform;
                    virtualCamera.Follow = characterGo.transform;
                }
            }
            catch{}
        }
    }
}
