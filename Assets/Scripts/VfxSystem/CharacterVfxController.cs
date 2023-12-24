using System.Collections.Generic;
using UnityEngine;
using Amegakure.Starkane.Entities;

namespace Amegakure.Starkane.VfxSystem
{
    public class CharacterVfxController : MonoBehaviour
    {
        [SerializeField] GameObject vfxInteractPrefab;

        private Dictionary<Character, GameObject> vfxCharacterMap;

        private void Awake()
        {
            vfxCharacterMap = new();
        }

        private void AddVfxInteract(Character character)
        {
            if (!vfxCharacterMap.ContainsKey(character))
            {
                Vector3 vfxPosition = new(character.transform.position.x, 0, character.transform.position.z);
                GameObject vfxInteract = Instantiate(vfxInteractPrefab, vfxPosition, Quaternion.identity);
                
                vfxInteract.transform.localScale = new Vector3(.75f, .25f, .75f);
                vfxCharacterMap.Add(character, vfxInteract);
            }
        }

        private void RemoveVfxInteract(Character character)
        {
            if (vfxCharacterMap.ContainsKey(character))
            {
                GameObject vfxInteract = vfxCharacterMap[character];
                Destroy(vfxInteract);
                vfxCharacterMap.Remove(character);
            }
        }
    }
}
