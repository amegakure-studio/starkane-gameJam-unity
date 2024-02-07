using System;
using System.Globalization;
using System.Numerics;
using Dojo;
using Dojo.Torii;
using dojo_bindings;
using UnityEngine;

public class MatchState : ModelInstance
{
    [ModelField("id")]
    private UInt32 id;

    [ModelField("turn")]
    private UInt32 turn;

    [ModelField("player_turn")]
    private dojo.FieldElement player_turn;

    [ModelField("map_id")]
    private UInt32 map_id;

    [ModelField("winner")]
    private dojo.FieldElement winner;

    private BigInteger winnerId;
    private BigInteger playerTurnId;


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
        base.Initialize(model);

        var player_turn_string = BitConverter.ToString(player_turn.data.ToArray()).Replace("-", "").ToLower();
        PlayerTurnId = BigInteger.Parse( player_turn_string, NumberStyles.AllowHexSpecifier );

        var winner_id_string = BitConverter.ToString(winner.data.ToArray()).Replace("-", "").ToLower();
        WinnerId = BigInteger.Parse( winner_id_string, NumberStyles.AllowHexSpecifier );
    }
}
