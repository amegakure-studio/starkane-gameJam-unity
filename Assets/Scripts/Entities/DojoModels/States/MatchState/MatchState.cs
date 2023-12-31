using System;
using System.Globalization;
using System.Numerics;
using Dojo;
using Dojo.Torii;
using dojo_bindings;
using UnityEngine;

public class MatchState : ModelInstance
{
    private UInt32 id;
    private UInt32 turn;
    private dojo.FieldElement player_turn;
    private BigInteger playerTurnId;
    private UInt32 map_id;
    private dojo.FieldElement winner;
    private BigInteger winnerId;
    public event Action<BigInteger> playerTurnIdChanged;
    public event Action<BigInteger> winnerChanged;

    public uint Id { get => id; set => id = value; }
    public uint Turn { get => turn; set => turn = value; }
    public BigInteger PlayerTurnId
    {
        get => playerTurnId;
        set 
        {
            playerTurnId = value;
            playerTurnIdChanged?.Invoke(playerTurnId);
        } 
    }
    public uint Map_id { get => map_id; set => map_id = value; }
    public BigInteger WinnerId
    { 
        get => winnerId;
        set
        {
            if(!winnerId.Equals(value))
            {
                winnerId = value; 
                winnerChanged?.Invoke(winnerId);
            }
        }
    }

    public override void Initialize(Model model)
    {
        id = model.members["id"].ty.ty_primitive.u32;
        turn = model.members["turn"].ty.ty_primitive.u32;
        player_turn = model.members["player_turn"].ty.ty_primitive.felt252;
        map_id = model.members["map_id"].ty.ty_primitive.u32;
        winner = model.members["winner"].ty.ty_primitive.felt252;

        var player_turn_string = BitConverter.ToString(player_turn.data.ToArray()).Replace("-", "").ToLower();
        PlayerTurnId = BigInteger.Parse( player_turn_string, NumberStyles.AllowHexSpecifier );

        var winner_id_string = BitConverter.ToString(winner.data.ToArray()).Replace("-", "").ToLower();
        WinnerId = BigInteger.Parse( winner_id_string, NumberStyles.AllowHexSpecifier );

        // Debug.Log("Match_state: \n id: " + id + "\n"
        //             + "turn: " + turn + "\n" +
        //             "playerTurnId: "+ playerTurnId + "\n"
        //             + "map_id: " + map_id + "\n" 
        //             + "winnerId: " + winnerId + "\n");
    }
}
