using Dojo;
using Dojo.Torii;
using dojo_bindings;
using UnityEngine;

public enum Direction
{
    None,
    Left,
    Right,
    Up,
    Down,

}

public class Moves : ModelInstance
{
    private dojo.FieldElement player;
    private byte remaining;
    private Direction lastDirection;

    public dojo.FieldElement Player 
    { 
        get => player;
        set 
        {
            if( !value.Equals(player))
            {
                player = value;
                Debug.Log("Changed Player field");

            }
        } 
    }
    public byte Remaining 
    { 
        get => remaining;
        set 
        {
            if(value != remaining)
            {
                remaining = value;
                Debug.Log("Changed ramining");

            }
        } 
    }
    public Direction LastDirection { get => lastDirection; set => lastDirection = value; }

    public override void Initialize(Model model) {
        player = model.members["player"].ty.ty_primitive.contract_address;
        remaining = model.members["remaining"].ty.ty_primitive.u8;
        lastDirection = (Direction)model.members["last_direction"].ty.ty_primitive.u8;
    }
}