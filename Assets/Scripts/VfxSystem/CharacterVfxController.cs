using Amegakure.Starkane.Id;
using Amegakure.Starkane.PubSub;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Amegakure.Starkane.VfxSystem
{
    public class CharacterVfxController : MonoBehaviour
    {
        [SerializeField] GameObject vfxSelectPrefab;

        private Dictionary<CharacterId, GameObject> vfxSelectCharacterMap;

        private void Awake()
        {
            vfxSelectCharacterMap = new();
        }

        private void OnEnable()
        {
            EventManager.Instance.Subscribe(GameEvent.INPUT_CHARACTER_SELECTED, HandleCharacterSelected);
            EventManager.Instance.Subscribe(GameEvent.INPUT_CHARACTER_UNSELECTED, HandleCharacterUnselected);
        }

        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe(GameEvent.INPUT_CHARACTER_SELECTED, HandleCharacterSelected);
            EventManager.Instance.Unsubscribe(GameEvent.INPUT_CHARACTER_UNSELECTED, HandleCharacterUnselected);
        }

        private void HandleCharacterUnselected(Dictionary<string, object> context)
        {
            try
            {
                CharacterId id = (CharacterId)context["CharacterId"];
                RemoveVfxInteract(id);
            }
            catch { }
        }

        private void HandleCharacterSelected(Dictionary<string, object> context)
        {
            try
            {
                CharacterId id = (CharacterId)context["CharacterId"];
                GameObject characterGo = (GameObject)context["CharacterGo"];

                AddVfxInteract(id, characterGo);
            }
            catch { }
        }

        private void AddVfxInteract(CharacterId id, GameObject characterGo)
        {
            if (!vfxSelectCharacterMap.ContainsKey(id))
            {
                Vector3 vfxPosition = new(characterGo.transform.position.x, 0, characterGo.transform.position.z);
                GameObject vfxSelect = Instantiate(vfxSelectPrefab, vfxPosition, Quaternion.identity);              
                
                vfxSelect.transform.localScale = new Vector3(.75f, .25f, .75f);
                vfxSelectCharacterMap.Add(id, vfxSelect);
            }
        }

        private void RemoveVfxInteract(CharacterId id)
        {
            if (vfxSelectCharacterMap.ContainsKey(id))
            {
                GameObject vfxSelect = vfxSelectCharacterMap[id];
                Destroy(vfxSelect);
                vfxSelectCharacterMap.Remove(id);
            }
        }
    }
}
