handlers.SetupPlayerLeaderBoard = function (args, context) {
    var request = {
        PlayFabId: context.playerProfile.PlayerId,
        Statistics: [{
                StatisticName: "TotalWin",
                Value: Math.floor(Math.random() * 100) + 1
            }, {
                StatisticName: "TotalKill",
                Value: Math.floor(Math.random() * 1000) + 1
            }, {
                StatisticName: "KillPerDeath",
                Value: Math.floor(Math.random() * 10) + 1
            }
        ]
    };
    server.UpdatePlayerStatistics(request);
};
handlers.SetupPlayerData = function (args, context) {
    var request = {
        Entity: server.GetUserAccountInfo({ PlayFabId: context.playerProfile.PlayerId }).UserInfo.TitleInfo.TitlePlayerAccount,
        Objects: [{
                ObjectName: "PlayerData",
                DataObject: { "LV": Math.floor(Math.random() * 10) + 1,
                    "Exp": Math.floor(Math.random() * 1000) + 1,
                    "EquipedWeapon": "weapon20" }
            }
        ]
    };
    entity.SetObjects(request);
    server.UpdateUserReadOnlyData({ PlayFabId: context.playerProfile.PlayerId, Data: {
            "Achievement": JSON.stringify({
                "Achievement0": false,
                "Achievement1": false,
                "Achievement2": false,
                "Achievement3": false,
                "Achievement4": false,
                "Achievement5": false,
                "Achievement6": false,
                "Achievement7": false,
                "Achievement8": false,
                "Achievement9": false,
                "Achievement10": false
            })
        }, Permission: "Public" });
};
handlers.CheckAchievement = function (args, context) {
    var eventData = context.playStreamEvent;
    var statisticName = eventData.StatisticName;
    var newStatisticValue = eventData.StatisticValue;
    var titleAlldata = server.GetTitleData({}).Data;
    var playerAchievementStata = JSON.parse(server.GetUserReadOnlyData({ PlayFabId: currentPlayerId }).Data["Achievement"].Value);
    for (var key in titleAlldata) {
        if (titleAlldata.hasOwnProperty(key)) {
            var element = JSON.parse(titleAlldata[key]);
            if (element["Description"] == statisticName) {
                for (var key_1 in playerAchievementStata) {
                    if (playerAchievementStata.hasOwnProperty(key_1)) {
                        var element_1 = playerAchievementStata[key_1];
                        if (newStatisticValue >= element_1["Count"] && element_1 == false) {
                            server.SendPushNotification({ Recipient: context.playerProfile.PlayerId, Message: "You have new achievement can get." });
                        }
                    }
                }
            }
        }
    }
};
handlers.GetAchievement = function (args, context) {
    var titledata;
    titledata = JSON.parse(server.GetTitleData({ Keys: args }).Data[args]);
    var playSta = server.GetPlayerStatistics({
        PlayFabId: currentPlayerId,
        StatisticNames: [titledata["Description"]]
    })
        .Statistics[0]
        .Value;
    var o = JSON.parse(server.GetUserReadOnlyData({ PlayFabId: currentPlayerId }).Data["Achievement"].Value);
    var playAchSta = o[args];
    var result;
    if (playSta >= titledata["Count"] && playAchSta == false) {
        result = server.GrantItemsToUser({ PlayFabId: currentPlayerId, ItemIds: ["AU50Bundle"] }).ItemGrantResults[0].Result;
        var AchievementName = args;
        o[AchievementName] = true;
        log.debug(JSON.stringify(o));
        server.UpdateUserReadOnlyData({ PlayFabId: currentPlayerId, Data: {
                "Achievement": JSON.stringify(o)
            }, Permission: "Public" });
    }
    else {
        return { "Result": "flase" };
    }
    return { "Result": result };
};
handlers.ExchangeGold = function (args) {
    var GMCost = 4;
    var AUGet = 500;
    var subtractDCResult = server.SubtractUserVirtualCurrency({
        PlayFabId: currentPlayerId,
        VirtualCurrency: "GM",
        Amount: GMCost
    });
    var GMResult = subtractDCResult["Balance"];
    var addGCResult = server.AddUserVirtualCurrency({
        PlayFabId: currentPlayerId,
        VirtualCurrency: "AU",
        Amount: AUGet
    });
    var GCResult = addGCResult["Balance"];
    return { diamondCurrencyResult: GMResult,
        goldCurrencyResult: GCResult };
};
handlers.PurchaseDiamond = function (args) {
    var GMResult = server.AddUserVirtualCurrency({
        PlayFabId: currentPlayerId,
        VirtualCurrency: "GM",
        Amount: Number.parseInt(args)
    }).Balance;
    return { diamondCurrencyResult: GMResult };
};
