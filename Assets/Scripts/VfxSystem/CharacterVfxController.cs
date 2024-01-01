using Amegakure.Starkane.EntitiesWrapper;
using Amegakure.Starkane.PubSub;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Amegakure.Starkane.VfxSystem
{
    public class CharacterVfxController : MonoBehaviour
    {
        [SerializeField] GameObject vfxSelectPrefab;

        private Dictionary<Character, GameObject> vfxSelectCharacterMap;

        private void Awake()
        {
            vfxSelectCharacterMap = new();
        }

        private void OnEnable()
        {
            EventManager.Instance.Subscribe(GameEvent.INPUT_CHARACTER_SELECTED, HandleCharacterSelected);
            EventManager.Instance.Subscribe(GameEvent.INPUT_CHARACTER_UNSELECTED, HandleCharacterUnselected);
            EventManager.Instance.Subscribe(GameEvent.INPUT_CHARACTER_HOVER, HandleCharacterSelected);
            EventManager.Instance.Subscribe(GameEvent.INPUT_CHARACTER_UNHOVER, HandleCharacterUnselected);
            EventManager.Instance.Subscribe(GameEvent.TILE_SELECTED, HandleTileSelected);
        }

        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe(GameEvent.INPUT_CHARACTER_SELECTED, HandleCharacterSelected);
            EventManager.Instance.Unsubscribe(GameEvent.INPUT_CHARACTER_UNSELECTED, HandleCharacterUnselected);
            EventManager.Instance.Unsubscribe(GameEvent.INPUT_CHARACTER_HOVER, HandleCharacterSelected);
            EventManager.Instance.Unsubscribe(GameEvent.INPUT_CHARACTER_UNHOVER, HandleCharacterUnselected);
            EventManager.Instance.Unsubscribe(GameEvent.TILE_SELECTED, HandleTileSelected);
        }

        private void HandleTileSelected(Dictionary<string, object> context)
        {
           try
            {
                DeselectAll();
            }
            catch { }
        }

        private void HandleCharacterUnselected(Dictionary<string, object> context)
        {
            try
            {
                Character character = (Character)context["Character"];
                RemoveVfxInteract(character);
            }
            catch { }
        }

        private void HandleCharacterSelected(Dictionary<string, object> context)
        {
            try
            {
                foreach(Character charactervfx in vfxSelectCharacterMap.Keys)
                {
                    RemoveVfxInteract(charactervfx);
                }
                
                Character character = (Character)context["Character"];

                AddVfxInteract(character);
            }
            catch { }
        }

        private void AddVfxInteract(Character character)
        {
            if (!vfxSelectCharacterMap.ContainsKey(character))
            {
                Vector3 vfxPosition = new(character.gameObject.transform.position.x, 0, character.gameObject.transform.position.z);
                GameObject vfxSelect = Instantiate(vfxSelectPrefab, vfxPosition, Quaternion.identity);              
                
                vfxSelect.transform.localScale = new Vector3(.75f, .25f, .75f);
                vfxSelectCharacterMap.Add(character, vfxSelect);
            }
        }

        private void RemoveVfxInteract(Character character)
        {
            if (vfxSelectCharacterMap.ContainsKey(character))
            {
                GameObject vfxSelect = vfxSelectCharacterMap[character];
                Destroy(vfxSelect);
                vfxSelectCharacterMap.Remove(character);
            }
        }

        private void DeselectAll()
        {
            foreach(Character character in vfxSelectCharacterMap.Keys)
            {
                GameObject vfxSelect = vfxSelectCharacterMap[character];
                Destroy(vfxSelect);
            }
            vfxSelectCharacterMap.Clear();
        }

    }
}
