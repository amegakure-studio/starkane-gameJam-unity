using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DojoTxFactory : MonoBehaviour
{
    [SerializeField] bool isLocalFactory = true;
    void Awake()
    {
        GameObject dojoTxConfigGo = Instantiate(new GameObject());
        dojoTxConfigGo.name = "DojoTxConfig";
        DojoTxConfig dojoTxConfig = dojoTxConfigGo.AddComponent<DojoTxConfig>();
        DontDestroyOnLoad(dojoTxConfigGo);

        if(isLocalFactory)
            createLocalDojoConfig(dojoTxConfig);
        else
            createSlotDojoConfig(dojoTxConfig);
    }

    private void createSlotDojoConfig(DojoTxConfig dojoTxConfig)
    {
        dojoTxConfig.RpcUrl = "https://api.cartridge.gg/x/starkane/katana";
        dojoTxConfig.ToriiUrl = "https://api.cartridge.gg/x/starkane/torii/grpc";
        dojoTxConfig.KatanaPrivateKey = "0x4c81c51faaf9075d23b039ff99202da294d473c2b1e7c90b510634375d263b1";
        dojoTxConfig.KatanaAccounAddress = "0x22dfe4742be8566663279fe3986b82146a027f921258e2f6926c0925fb81145";
        dojoTxConfig.ActionSystemActionAddress = "0x68705e426f391541eb50797796e5e71ee3033789d82a8c801830bb191aa3bf1";
        dojoTxConfig.CharacterSystemActionAddress = "0x57a6556e89380b76465e525c725d8ed065a03b47fb9a4c9b676a1afea8177c5";
        dojoTxConfig.MapCCSystemActionAddress = "0x1e4dca0e18e12a6ba359a036b1eed1e0156dceab53f2f222aaaec866781131c";
        dojoTxConfig.MatchSystemActionAddress = "0x2d4efd349d469a05680cb7f1186024b8d95c33bd11563de07fe687ddcbfa483";
        dojoTxConfig.MoveSystemActionAddress = "0xf95f269a39505092b2d4eea3268e2e8da83cfd12a20b0eceb505044ecaabf2";
        dojoTxConfig.RankingSystemActionAddress = "0x69de93331fbd6465ec0325e91cc31505e1db03dface725da02132dbd6fd51f0";
        dojoTxConfig.RecommendationSystemActionAddress = "0x692be1ba2ac534e58442b452ead9e9ba4464533aea84b845a23e8d6339c972";
        dojoTxConfig.SkillSystemActionAddress = "0x35bb3ad3ba19a57e4930ec2c85d2d65cc915ed9d78267aaa7b2c62c687b4143";
        dojoTxConfig.StatisticsSystemActionAddress = "0x496c84098701676f2af3d0bbafeb09cee6fb8b1ffb59134419a0f878df2d7df";
        dojoTxConfig.TurnSystemActionAddress = "0x61231db30a04f42b3c3e57cd13b0dee6053f8ed7c350135735e67c254b60454";
    }

    private void createLocalDojoConfig(DojoTxConfig dojoTxConfig)
    {
        dojoTxConfig.RpcUrl = "http://localhost:5050";
        dojoTxConfig.ToriiUrl = "http://localhost:8080";
        dojoTxConfig.KatanaPrivateKey = "0x1800000000300000180000000000030000000000003006001800006600";
        dojoTxConfig.KatanaAccounAddress = "0x6162896d1d7ab204c7ccac6dd5f8e9e7c25ecd5ae4fcb4ad32e57786bb46e03";
        dojoTxConfig.ActionSystemActionAddress = "0x68705e426f391541eb50797796e5e71ee3033789d82a8c801830bb191aa3bf1";
        dojoTxConfig.CharacterSystemActionAddress = "0x57a6556e89380b76465e525c725d8ed065a03b47fb9a4c9b676a1afea8177c5";
        dojoTxConfig.MapCCSystemActionAddress = "0x1e4dca0e18e12a6ba359a036b1eed1e0156dceab53f2f222aaaec866781131c";
        dojoTxConfig.MatchSystemActionAddress = "0x2d4efd349d469a05680cb7f1186024b8d95c33bd11563de07fe687ddcbfa483";
        dojoTxConfig.MoveSystemActionAddress = "0xf95f269a39505092b2d4eea3268e2e8da83cfd12a20b0eceb505044ecaabf2";
        dojoTxConfig.RankingSystemActionAddress = "0x69de93331fbd6465ec0325e91cc31505e1db03dface725da02132dbd6fd51f0";
        dojoTxConfig.RecommendationSystemActionAddress = "0x692be1ba2ac534e58442b452ead9e9ba4464533aea84b845a23e8d6339c972";
        dojoTxConfig.SkillSystemActionAddress = "0x35bb3ad3ba19a57e4930ec2c85d2d65cc915ed9d78267aaa7b2c62c687b4143";
        dojoTxConfig.StatisticsSystemActionAddress = "0x496c84098701676f2af3d0bbafeb09cee6fb8b1ffb59134419a0f878df2d7df";
        dojoTxConfig.TurnSystemActionAddress = "0x61231db30a04f42b3c3e57cd13b0dee6053f8ed7c350135735e67c254b60454";
    }

}
