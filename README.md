# PlayFab_Demo

This repository is base on [PlayFab_Tanks](https://github.com/Rockiez/PlayFab_Tanks) and is showcasing how to build modern live-game systems driven by PlayFab's back-end technology.

## Demo illustrates:
  * Unified Player Profile - Accounts uses Email or Device ID Authentication
  * Dynamic Gameplay - Drive client behaviors using TitleData
  * Virtual Items, Currencies, and Stores - Configured via PlayFab's Game Manager
  * Statistics Tracking - Used to drive Achievements and real-time leaderboards
  * Push Notifications - for re-engagement campaigns and special offers
  * Cloud-hosted Logic - Cloud Script methods that call a variety of protected APIs
  * Lobby and matchmaking - Example of how to work with Photon PUN
  * Inventory Management - Browse and equip/consume item.

### Images

#### Lobby
Photon offers Lobby and Room feature and works with Playfab to authenticate Photon players with PlayFab.
![lobby](https://github.com/Rockiez/PlayFab_Demo/raw/master/image/lobby.jpg)

#### Room
![room](https://github.com/Rockiez/PlayFab_Demo/raw/master/image/room.jpg)

#### Shop
![shop](https://github.com/Rockiez/PlayFab_Demo/raw/master/image/shop.jpg)

#### Inventory
![inventory](https://github.com/Rockiez/PlayFab_Demo/raw/master/image/inventory.jpg)

#### Leaderboard
![leaderboard](https://github.com/Rockiez/PlayFab_Demo/raw/master/image/leaderboard.jpg)

#### Achievement
![leaderboard](https://github.com/Rockiez/PlayFab_Demo/raw/master/image/achievement.jpg)

#### UserMessage

![userMessage](https://github.com/Rockiez/PlayFab_Demo/raw/master/image/userMessage.jpg)

#### Payment
![payment](https://github.com/Rockiez/PlayFab_Demo/raw/master/image/payment.jpg)


## Configuration and Setup
### Prerequisites:
This project is a simple example that uses some essential functions of PlayFab and Photon.

- You should be familiar with Unity3d & uGUI 
- Have a [basic understanding](https://api.playfab.com/) of the PlayFab API
- Be familiar with fundamental concepts on which Photon Unity Networking (PUN) is based, and you are already familiar with Photon API


### Back-end Setup:


PlayFab_Demo is a demo game to show off how PlayFab works. Like most modern games, it's designed to be operated as a service, and depends on a back-end platform like PlayFab to operate the game post-launch.

These instructions describe how to set up your own personalized back-end for the game, so you can experiment with changing data and using all the PlayFab features without affecting anyone else. Following these steps, you will first create a new title, then upload a series of configuration files to that title using [UploadGui](https://github.com/Rockiez/UploadGui) to support PlayFab_Demo.


#### Create the new title
1. Go to the Game Manager and click "Create a new game"
2. Fill out the page for the new title. Click "Create Game".
3. You should now see your new title appear in your game studio. If it's not there right away, wait a minute then refresh your browser since it makes take a few seconds to show up. Take note of the Title ID for your new title.

#### Download required files
1. Download the configuration files.
2. Download  [UploadGui](https://github.com/Rockiez/UploadGui),this tool has a simple and intuitive GUI interface and designs around to make it easier for user to upload the configuration files to the PlayFab server to setup your new title.

### Configure for your new title
1. You need to read readme.md of UploadGui and follow it to upload configuration files.(Select the folder named PlayFabData)
2. Log into Game Manager
3. Click the new title.
4. Navigate to Settings then API Features
5. Ensure that your clients are allowed to post player statistics. 
* note: this would not be a good idea if this was a competitive game.
6. Navigate to Actomation then Rules, click New Rule and setup as picture below.
![Rule](https://github.com/Rockiez/PlayFab_Demo/raw/master/image/Rule.jpg)
7. When have some users in your title, you can create Scheduled Tasks to execute some setup method in CloudScript for each user.You can refer to the following figure to configure that.
![ScheduledTask](https://github.com/Rockiez/PlayFab_Demo/raw/master/image/ScheduledTask.jpg)
8. Click[Getting started with Photon and Unity](https://api.playfab.com/docs/tutorials/landing-tournaments/photon-unity), and follow the guide to set up Photon Multiplayer to work with PlayFab
9. If you build the project for Android device, Click[Push Notifications for Android](https://api.playfab.com/docs/tutorials/landing-players/push-notification-basics/push-notifications-for-android) to set up push notifications on Android.
### Client Setup:

#### Prerequisite:
Ensure that you have completed the Back-end Guide, and configured your own custom title with your own Title ID.


The next step is to get the client running on your own device, and communicating with your own back-end.

To compile yourself in Unity, you'll want to first download this entire Source Project onto your local PC.

1. The project is already set up to use [ PlayFab Editor Extension](https://github.com/PlayFab/UnityEditorExtensions) for Unity.  you need Login to your PlayFab account in the editor extension and select the title ID you set up previously. You can read more about the Editor Extensions on their GitHub repository page.
2. Add your appid from your created photon app to the PUN Wizard Window for setup Pun.


## More information:
For a complete list of available PlayFab APIs, check out the [online documentation](http://api.playfab.com/).

PlayFab Developer Team can assist with answering any questions as well as process any feedback you have about PlayFab services in Forums.
[Forums](https://community.playfab.com/index.html)

There is an [Online Documentation](http://doc.photonengine.com/en-us/pun/v2), which is considered a manual for PUN. This might become your primary source for information.
