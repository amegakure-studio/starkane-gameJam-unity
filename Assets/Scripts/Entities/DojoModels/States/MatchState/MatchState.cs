using System;
using System.Globalization;
using System.Numerics;
using Dojo;
using Dojo.Starknet;
using Dojo.Torii;

public class MatchState : ModelInstance
{
    [ModelField("id")]
    public UInt32 id;

    [ModelField("turn")]
    public UInt32 turn;

    [ModelField("player_turn")]
    public FieldElement player_turn;

    [ModelField("map_id")]
    public UInt32 map_id;

    [ModelField("winner")]
    public FieldElement winner;

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

        //var player_turn_string = BitConverter.ToString(player_turn.data.ToArray()).Replace("-", "").ToLower();
        //PlayerTurnId = BigInteger.Parse( player_turn_string, NumberStyles.AllowHexSpecifier );

        //var winner_id_string = BitConverter.ToString(winner.data.ToArray()).Replace("-", "").ToLower();
        //WinnerId = BigInteger.Parse( winner_id_string, NumberStyles.AllowHexSpecifier );
    }
}
