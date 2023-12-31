using Amegakure.Starkane.EntitiesWrapper;
using Amegakure.Starkane.PubSub;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Amegakure.Starkane.SoundSystem
{
    [RequireComponent(typeof(AudioSource))]
    public class CombatSoundManager : MonoBehaviour
    {
        [SerializeField] AudioClip defaultSound;
        [SerializeField] AudioClip playerSound;
        [SerializeField] AudioClip rivalSound;

        private AudioSource audioSource;
        private Player sessionPlayer;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();         
            audioSource.clip = defaultSound;
            audioSource.loop = true;
            audioSource.playOnAwake = true;
        }
        private void OnEnable()
        {
            EventManager.Instance.Subscribe(GameEvent.COMBAT_SKILL_DONE, HandleCombatSkillDone);
            EventManager.Instance.Subscribe(GameEvent.CUTSCENE_COMBAT_END, HandleCutsceneCombatEnd);
        }

        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe(GameEvent.COMBAT_SKILL_DONE, HandleCombatSkillDone);
            EventManager.Instance.Unsubscribe(GameEvent.CUTSCENE_COMBAT_END, HandleCutsceneCombatEnd);
        }

        private void Start()
        {
            sessionPlayer = GameObject.FindAnyObjectByType<Session>().Player;
        }

        private void HandleCutsceneCombatEnd(Dictionary<string, object> context)
        {
            audioSource.clip = default;
        }

        private void HandleCombatSkillDone(Dictionary<string, object> context)
        {
            try
            {
                Player playerFrom = (Player)context["PlayerFrom"];
                audioSource.clip = playerFrom == sessionPlayer ? playerSound : rivalSound;

            } catch { }       
        }
    }
}
