using System;
using Discord;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script.Network
{
    public class DiscordController : MonoBehaviour
    {
        public Discord.Discord discord;
        public static DiscordController instance;

        private const string _applicationId = "817277716498743337";
        private const string _partyId = "ae488379-351d-4a4f-ad32-2b9b01c91657";
        private static string _version { get { return Application.unityVersion; } }
        private static string _projectName { get { return Application.productName; } }
        private static string _activeSceneName { get { return SceneManager.GetActiveScene().name; } }
        private static long _lastTimestamp;

        void Awake()
        {
            if (!instance)
            {
                instance = this;
            } 
            else if (instance != this)
            {
                Destroy(this);
            }
            DontDestroyOnLoad(this);
            
            discord = new Discord.Discord(long.Parse(_applicationId), (System.UInt64)Discord.CreateFlags.Default);

            var activityManager = discord.GetActivityManager();
            var activity = new Discord.Activity
            {
                Details = _projectName,
                State = "접속 중",
                Assets =
                {
                    LargeImage = "kongbab_test_image1",
                    LargeText = _version,
                    SmallImage = "kongbab_test_image2",
                    SmallText = "It's me! SmallText!"
                }
            };
            
            activityManager.UpdateActivity(activity, result =>
            {
                Debug.Log("Discord Result: " + result);
            });

        }

        void Update()
        {
            discord.RunCallbacks();
            UpdateActivity();
        }

        private void UpdateActivity()
        {
            var activityManager = discord.GetActivityManager();
            var lobbyManager = discord.GetLobbyManager();
            
            string currSceneState = _activeSceneName;
            
            Activity activity = new Activity
            {
                Details = _projectName,
                State = "",
                Assets =
                {
                    LargeImage = "kongbab_test_image1",
                    LargeText = _version,
                    SmallImage = "kongbab_test_image2",
                    SmallText = "It's me! SmallText!"
                }
            };
            
            if (_activeSceneName.Equals("BackgroundTest"))
            {
                currSceneState = "게임 중";
            }
            else if (BeanRiceHeavenLobbyManager.Instance.roomPanel.activeSelf)
            {
                currSceneState = "매칭 중";
                                
                activity.Party = new PartySize
               {
                   CurrentSize = PhotonNetwork.CurrentRoom.PlayerCount,
                   MaxSize = PhotonNetwork.CurrentRoom.MaxPlayers
               };
            }
            else if (_activeSceneName.Equals("LobbyScene"))
            {
                currSceneState = "접속 중";
            }
            
            activity.State = currSceneState;
            
            activityManager.UpdateActivity(activity, result =>
            {
               // Debug.Log("Discord Result: " + result);
            });
        }

    void OnDisable()
    {
        Debug.Log("Discord: shutdown");
        discord.Dispose();
    }
    }
}