using Amegakure.Starkane.PubSub;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LoadingScreenController : MonoBehaviour
{
    [SerializeField] Sprite[] loadingBgs;

    private VisualElement container;

    private void Start()
    {
        VisualElement root = GameObject.FindAnyObjectByType<UIDocument>().rootVisualElement;
        container = root.Q<VisualElement>("LoadingScreen");
    }

    private void OnEnable()
    {
        EventManager.Instance.Subscribe(GameEvent.GAME_LOADING_START, HandleGameLoadingStart);
    }

    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe(GameEvent.GAME_LOADING_START, HandleGameLoadingStart);
    }

    private void HandleGameLoadingStart(Dictionary<string, object> context)
    {
        try
        {
            int bgIndex = UnityEngine.Random.Range(0, loadingBgs.Length);
            Sprite sprite = loadingBgs[bgIndex];

            container.style.backgroundImage = new StyleBackground(sprite);

        } catch { }
        
        container?.RemoveFromClassList("hidden");
    }

}
