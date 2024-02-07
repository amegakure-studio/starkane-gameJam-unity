using System.Collections;
using System.Collections.Generic;
using Amegakure.Starkane.PubSub;
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
        StartCoroutine(nameof(LoadAsyncScene));
    }

    IEnumerator LoadAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(2);
        asyncLoad.allowSceneActivation = false;

        EventManager.Instance.Publish(GameEvent.GAME_LOADING_START, new Dictionary<string, object>());

        while (!asyncLoad.isDone)
        {
            //Debug.Log("Still here: " + asyncLoad.progress);
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
            
            yield return null;
        }
    }
}
