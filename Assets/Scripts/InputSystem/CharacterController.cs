using Amegakure.Starkane;
using Amegakure.Starkane.Id;
using Amegakure.Starkane.PubSub;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    private CharacterId id;

    public CharacterId Id { get => id; set => id = value; }

    private void OnMouseEnter()
    {
        CharacterHoverEnter();
    }

    private void OnMouseExit()
    {
        CharacterHoverExit();
    }

    private void OnMouseDown()
    {
        CharacterSelected();
    }

    public void CharacterHoverEnter()
    {
        
    }

    public void CharacterHoverExit()
    {

    }

    public void CharacterSelected()
    {
        EventManager.Instance.Publish(GameEvent.INPUT_CHARACTER_SELECTED, new() { { "CharacterId", id }, { "CharacterGo", gameObject } });
    }
}
