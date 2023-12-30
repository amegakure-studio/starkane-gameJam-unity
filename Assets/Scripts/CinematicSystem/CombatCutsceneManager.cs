using Amegakure.Starkane.EntitiesWrapper;
using Amegakure.Starkane.PubSub;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Cinemachine;

public class CombatCutsceneManager : MonoBehaviour
{
    [SerializeField] PlayableDirector attackDirector;
    [SerializeField] Transform attackerCutsceneTransform;
    [SerializeField] Transform receiverCutsceneTransform;

    private GameObject attackerGo;
    private GameObject receiverGo;

    private Vector3 attackerPreviousPosition;
    private Vector3 receiverPreviousPosition;
    private Quaternion attackerPreviousRotation;
    private Quaternion receiverPreviousRotation;

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
                if (output.streamName == "CutSceneCamera")
                {
                    // Access the binding of the GameObject track
                    var trackBinding = attackDirector.GetGenericBinding(output.sourceObject) as GameObject;

                    // Check if the GameObject has the name "CutSceneCamera"
                    // if (trackBinding != null)
                    // {
                    //     CinemachineVirtualCamera virtualCamera = trackBinding.GetComponent<CinemachineVirtualCamera>();
                    //     virtualCamera.LookAt = characterFrom.transform;
                    //     virtualCamera.Follow = characterFrom.transform;
                    // }
                }

                if (output.streamName == "AttackerGo")
                {
                    attackDirector.SetGenericBinding(output.sourceObject, characterFrom.gameObject);
                    attackerGo = characterFrom.gameObject;
                    attackerPreviousPosition = attackerGo.transform.position;
                    attackerPreviousRotation = attackerGo.transform.rotation;
                }

                if (output.streamName == "AttackerSignal")
                {
                    SignalReceiver characterFromSignal = characterFrom.GetComponent<SignalReceiver>();
                    attackDirector.SetGenericBinding(output.sourceObject, characterFromSignal);
                }

                if (output.streamName == "ReceiverGo")
                {
                    attackDirector.SetGenericBinding(output.sourceObject, characterReceiver.gameObject);
                    receiverGo = characterReceiver.gameObject;
                    receiverPreviousPosition = receiverGo.transform.position;
                    receiverPreviousRotation = receiverGo.transform.rotation;
                }

                if (output.streamName == "ReceiverSignal")
                {
                    SignalReceiver characterReceiverSignal = characterReceiver.GetComponent<SignalReceiver>();
                    attackDirector.SetGenericBinding(output.sourceObject, characterReceiverSignal);
                }
            }

            ExecuteCutscene(attackDirector);


        } catch (Exception e) { }
    }

    private void ExecuteCutscene(PlayableDirector director)
    {
        EventManager.Instance.Publish(GameEvent.CUTSCENE_COMBAT_START,
        new Dictionary<string, object>());
        attackerGo.transform.position = attackerCutsceneTransform.position;
        attackerGo.transform.rotation = attackerCutsceneTransform.rotation;

        receiverGo.transform.position = receiverCutsceneTransform.position;     
        receiverGo.transform.rotation = receiverCutsceneTransform.rotation;

        director.Stop();
        director.time = 0;
        director.Play();
    }

    public void ResetPlayersPositions()
    {
        attackerGo.transform.position = attackerPreviousPosition;
        attackerGo.transform.rotation = attackerPreviousRotation;

        receiverGo.transform.position = receiverPreviousPosition;
        receiverGo.transform.rotation = receiverPreviousRotation;
        EventManager.Instance.Publish(GameEvent.CUTSCENE_COMBAT_END,
        new Dictionary<string, object>());        
    }
}
