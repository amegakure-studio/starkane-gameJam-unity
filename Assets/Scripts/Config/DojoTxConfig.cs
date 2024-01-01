using System.Collections;
using System.Collections.Generic;
using Dojo.Starknet;
using UnityEngine;

public class DojoTxConfig : MonoBehaviour
{
    private string rpcUrl;
    private string toriiUrl;
    private string  katanaPrivateKey;
    private string katanaAccounAddress;
    private string actionSystemActionAddress;
    private string characterSystemActionAddress;
    private string mapCCSystemActionAddress;
    private string matchSystemActionAddress;
    private string moveSystemActionAddress;
    private string rankingSystemActionAddress;
    private string recommendationSystemActionAddress;
    private string skillSystemActionAddress;
    private string statisticsSystemActionAddress;
    private string turnSystemActionAddress;

    public string RpcUrl { get => rpcUrl; set => rpcUrl = value; }
    public string ToriiUrl{ get => toriiUrl; set => toriiUrl = value;}
    public string KatanaPrivateKey { private get => katanaPrivateKey; set => katanaPrivateKey = value; }
    public string KatanaAccounAddress { get => katanaAccounAddress; set => katanaAccounAddress = value; }
    public string ActionSystemActionAddress { get => actionSystemActionAddress; set => actionSystemActionAddress = value; }
    public string CharacterSystemActionAddress { get => characterSystemActionAddress; set => characterSystemActionAddress = value; }
    public string MapCCSystemActionAddress { get => mapCCSystemActionAddress; set => mapCCSystemActionAddress = value; }
    public string MatchSystemActionAddress { get => matchSystemActionAddress; set => matchSystemActionAddress = value; }
    public string MoveSystemActionAddress { get => moveSystemActionAddress; set => moveSystemActionAddress = value; }
    public string RankingSystemActionAddress { get => rankingSystemActionAddress; set => rankingSystemActionAddress = value; }
    public string RecommendationSystemActionAddress { get => recommendationSystemActionAddress; set => recommendationSystemActionAddress = value; }
    public string SkillSystemActionAddress { get => skillSystemActionAddress; set => skillSystemActionAddress = value; }
    public string StatisticsSystemActionAddress { get => statisticsSystemActionAddress; set => statisticsSystemActionAddress = value; }
    public string TurnSystemActionAddress { get => turnSystemActionAddress; set => turnSystemActionAddress = value; }

    public SigningKey GetKatanaPrivateKey()
    {
        return new SigningKey(katanaPrivateKey);
    }
}
