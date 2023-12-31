using Amegakure.Starkane.PubSub;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Amegakure.Starkane.SoundSystem
{
    [RequireComponent(typeof(AudioSource))]
    public class UISfxManager : MonoBehaviour
    {
        [SerializeField] AudioClip characterSelectedSfx;
        [SerializeField] AudioClip characterUnselectedSfx;

        private AudioSource m_AudioSource;

        private void Awake()
        {
            m_AudioSource = GetComponent<AudioSource>();
            m_AudioSource.loop = false;
            m_AudioSource.playOnAwake = false;
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

        private void HandleCharacterSelected(Dictionary<string, object> context)
        {
            PlaySfx(characterSelectedSfx);
        }

        private void HandleCharacterUnselected(Dictionary<string, object> context)
        {
            PlaySfx(characterUnselectedSfx);
        }

        private void PlaySfx(AudioClip clip)
        {
            m_AudioSource.clip = clip;
            m_AudioSource.Play();
        }
    }
}
