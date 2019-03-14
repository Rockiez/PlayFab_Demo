handlers.SetupPlayerLeaderBoard = function (args:any,context: IPlayFabContext ) {
  let request: PlayFabServerModels.UpdatePlayerStatisticsRequest = {
    PlayFabId: context.playerProfile.PlayerId,
    Statistics: [{
      StatisticName: "TotalWin",
      Value: Math.floor(Math.random()*100)+1
  },{
    StatisticName: "TotalKill",
    Value: Math.floor(Math.random()*1000)+1
  },{
    StatisticName: "KillPerDeath",
    Value: Math.floor(Math.random()*10)+1
  }
]
  }
  server.UpdatePlayerStatistics(request)
}


handlers.SetupPlayerData = function (args:any,context: IPlayFabContext ) {
  let request : PlayFabDataModels.SetObjectsRequest={
    Entity: server.GetUserAccountInfo({PlayFabId:context.playerProfile.PlayerId}).UserInfo.TitleInfo.TitlePlayerAccount,
    Objects:[{
      ObjectName:"PlayerData",
      DataObject:{"LV": Math.floor(Math.random()*10)+1,
                  "Exp": Math.floor(Math.random()*1000)+1,
                  "EquipedWeapon": "weapon20"}
    }
    ]
  }
  entity.SetObjects(request)

  server.UpdateUserReadOnlyData({PlayFabId:context.playerProfile.PlayerId,Data:{
    "Achievement":JSON.stringify({    
    "Achievement0":false,
    "Achievement1":false,
    "Achievement2":false,
    "Achievement3":false,
    "Achievement4":false,
    "Achievement5":false,
    "Achievement6":false,
    "Achievement7":false,
    "Achievement8":false,
    "Achievement9":false,
    "Achievement10":false})
  },Permission: "Public"})
}

handlers.CheckAchievement = function(args:any, context:IPlayFabContext){
  let eventData:PlayStreamModels.character_statistic_changed = <PlayStreamModels.character_statistic_changed>context.playStreamEvent;
  let statisticName = eventData.StatisticName;
  let newStatisticValue = eventData.StatisticValue;

  let titleAlldata = server.GetTitleData({}).Data


  let playerAchievementStata =JSON.parse( server.GetUserReadOnlyData({PlayFabId: currentPlayerId}).Data["Achievement"].Value)

  for (const key in titleAlldata) {
    if (titleAlldata.hasOwnProperty(key)) {

      const element = JSON.parse(titleAlldata[key]);
      if (element["Description"] == statisticName) {

        for (const key in playerAchievementStata) {
          if (playerAchievementStata.hasOwnProperty(key)) {
            const element = playerAchievementStata[key];

            if (newStatisticValue>= element["Count"] && element == false) {
              server.SendPushNotification({Recipient:context.playerProfile.PlayerId,Message:"You have new achievement can get."})
            }
          }
        }

      }
    }
  }



}

handlers.GetAchievement = function(args:any,context: IPlayFabContext){
  let titledata:object
    titledata = JSON.parse(server.GetTitleData( {Keys:args}).Data[args])

  let playSta:number = server.GetPlayerStatistics(
    {
      PlayFabId:currentPlayerId,
      StatisticNames:[titledata["Description"]]
    })
    .Statistics[0]
    .Value

  let o =JSON.parse( server.GetUserReadOnlyData({PlayFabId: currentPlayerId}).Data["Achievement"].Value)

  let playAchSta:boolean = o[args]
    
  let result;
  if (playSta>= titledata["Count"] && playAchSta == false) {
    result = server.GrantItemsToUser({PlayFabId:currentPlayerId,ItemIds:["AU50Bundle"]}).ItemGrantResults[0].Result;
    let AchievementName:string = args ;
      o[AchievementName] = true;
      log.debug(JSON.stringify(o));
    server.UpdateUserReadOnlyData({PlayFabId:currentPlayerId,Data:{
      "Achievement":JSON.stringify(o)
    },Permission: "Public"})
  }else{
    return {"Result":"flase"}
  }
  return {"Result":result}
}



handlers.ExchangeGold = function(args)
{
  let GMCost = 4;
  let AUGet = 500;
  
  let subtractDCResult = server.SubtractUserVirtualCurrency({
    PlayFabId : currentPlayerId,
    VirtualCurrency : "GM",
    Amount : GMCost
  });
  let GMResult = subtractDCResult["Balance"];
  
  let addGCResult = server.AddUserVirtualCurrency({
    PlayFabId : currentPlayerId,
    VirtualCurrency : "AU",
    Amount : AUGet
  });
  let GCResult = addGCResult["Balance"];
  
  return { diamondCurrencyResult : GMResult,
        goldCurrencyResult : GCResult};
}

handlers.PurchaseDiamond = function (args)
{
  let GMResult =  server.AddUserVirtualCurrency({
    PlayFabId : currentPlayerId,
    VirtualCurrency : "GM",
    Amount : Number.parseInt(args)
  }).Balance;
  return { diamondCurrencyResult : GMResult};
}

