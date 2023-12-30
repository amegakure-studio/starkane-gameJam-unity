using Amegakure.Starkane.EntitiesWrapper;
using Amegakure.Starkane.PubSub;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CombatCutsceneManager : MonoBehaviour
{
    [SerializeField] PlayableDirector attackDirector;

    private void OnEnable()
    {
        EventManager.Instance.Subscribe(GameEvent.COMBAT_SKILL_DONE, HandleSkillDone);
    }

    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe(GameEvent.COMBAT_SKILL_DONE, HandleSkillDone);
    }

    private void HandleSkillDone(Dictionary<string, object> context)
    {
        try
        {
            Player playerFrom = (Player)context["PlayerFrom"];
            Character characterFrom = (Character)context["CharacterFrom"];
            Skill skill = (Skill)context["Skill"];
            Player playerReceiver = (Player)context["PlayerReceiver"];
            Character characterReceiver = (Character)context["CharacterReceiver"];

            foreach (var output in attackDirector.playableAsset.outputs)
            {
                if (output.streamName == "AttackerGo")
                {
                    attackDirector.SetGenericBinding(output.sourceObject, characterFrom.gameObject);
                }

                if (output.streamName == "AttackerSignal")
                {
                    SignalReceiver characterFromSignal = characterFrom.GetComponent<SignalReceiver>();
                    attackDirector.SetGenericBinding(output.sourceObject, characterFromSignal);
                }

                if (output.streamName == "ReceiverGo")
                {
                    attackDirector.SetGenericBinding(output.sourceObject, characterReceiver.gameObject);
                }

                if (output.streamName == "ReceiverSignal")
                {
                    SignalReceiver characterReceiverSignal = characterFrom.GetComponent<SignalReceiver>();
                    attackDirector.SetGenericBinding(output.sourceObject, characterReceiverSignal);
                }
            }

            ExecuteCutscene(attackDirector);


        } catch (Exception e) { }
    }

    private void ExecuteCutscene(PlayableDirector director)
    {
        director.Stop();
        director.time = 0;
        director.Play();
    }
}
