using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CaveEntranceController : MonoBehaviour
{
    [SerializeField] GameObject vfxSelectPrefab;

    private void OnMouseEnter()
    {
        if(vfxSelectPrefab != null)
            vfxSelectPrefab.SetActive(true);
            
    }

    private void OnMouseExit()
    {
        if(vfxSelectPrefab != null)
            vfxSelectPrefab.SetActive(false);
       
    }

    private void OnMouseDown()
    {
        SceneManager.LoadScene("Cave");
    }
}
