using System;
using System.Collections.Generic;
using System.Text;
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
        private static long _lastTimeStamp;
        
        private List<Relationship> _relationships;
        private float timer = 0.0f;

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
            
            SetupDiscord();

        }

        void Update()
        {
            discord.RunCallbacks();
            timer += Time.deltaTime;
            if (timer > 2.0f)
            {
                timer -= 2.0f;
                UpdateActivity();
            }
        }

        private void SetupDiscord()
        {
            discord = new Discord.Discord(long.Parse(_applicationId), (System.UInt64)Discord.CreateFlags.Default);
            _lastTimeStamp = GetTimestamp();

            var relationshipManager = discord.GetRelationshipManager();
            _relationships = new List<Relationship>();
            
            var activityManager = discord.GetActivityManager();
            LobbyManager lobbyManager = discord.GetLobbyManager();

            var activity = new Activity();
            
            activityManager.OnActivityJoin += secret =>
            {
                Debug.Log("OnJoin " + secret);
                
                
                lobbyManager.ConnectLobbyWithActivitySecret(secret,
                    (Discord.Result result, ref Discord.Lobby lobby) =>
                    {
                        Debug.Log("Connected to lobby: " + lobby.Id);
                        
                        lobbyManager.ConnectVoice(lobby.Id, (Discord.Result voiceResult) => {

                            if (voiceResult == Discord.Result.Ok) {
                                Debug.Log("New User Connected to Voice! Say Hello! Result: " + voiceResult);
                            } else {
                                Debug.Log("Failed with Result: " + voiceResult);
                            };
                        });
                        
                       lobbyManager.ConnectNetwork(lobby.Id); 
                       lobbyManager.OpenNetworkChannel(lobby.Id, 0, true);

                       foreach (var user in lobbyManager.GetMemberUsers(lobby.Id))
                       {
                           //Send a hello message to everyone in the lobby
                           lobbyManager.SendNetworkMessage(lobby.Id, user.Id, 0,
                               Encoding.UTF8.GetBytes(string.Format("Hello, " + user.Username + "!")));
                       }
                       
                       var activity1 = new Discord.Activity
                       {
                           Details = _projectName,
                           State = "",
                           Assets =
                           {
                               LargeImage = "kongbab_test_image1",
                           },
                           Party =
                           {
                               Id = lobby.Id.ToString(),
                               Size = {
                                   CurrentSize = lobbyManager.MemberCount(lobby.Id),
                                   MaxSize = (int)lobby.Capacity,
                               },
                           },
                           Secrets =
                           {
                               Join = "123",
                           },
                           Instance = true,
                       };

                       activity = activity1;
                    });
            };
            
            activityManager.UpdateActivity(activity, result =>
            {
                Debug.Log("Discord Result: " + result);
            });

        }

        private long GetTimestamp()
        {
            long unixTimeStamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            return unixTimeStamp;
        }
        
        private void UpdateActivity()
        {
            var activityManager = discord.GetActivityManager();
            var lobbyManager = discord.GetLobbyManager();
            
            string currSceneState = _activeSceneName;
            
            var activity = new Discord.Activity
            {
                Details = _projectName,
                State = "",
                Timestamps =
                {
                    Start = _lastTimeStamp,
                },
                
                Assets =
                {
                    LargeImage = "kongbab_test_image1",
                },
                
                Secrets =
                {
                    Join = "123",
                },

                Instance = true,
            };
            
            if (_activeSceneName.Equals("Intro Scene"))
            {
                currSceneState = "";
            }
            else if (BeanRiceHeavenLobbyManager.Instance.roomPanel && 
                     BeanRiceHeavenLobbyManager.Instance.roomPanel.activeSelf && PhotonNetwork.InRoom)
            {
                currSceneState = "대기 중";
                
                activity.Party.Id = "Beans Party";
                activity.Party.Size.CurrentSize = PhotonNetwork.PlayerList.Length;
                activity.Party.Size.MaxSize = 4;
            }
            else if (_activeSceneName.Equals("LoadingScene"))
            {
                currSceneState = "로딩 중";
            }
            else if (_activeSceneName.Equals("GeneratedMap"))
            {
                currSceneState = "게임 중";
            }
            else if (_activeSceneName.Equals("EndingScene"))
            {
                currSceneState = "게임 종료";
            }
            
            activity.State = currSceneState;
            
            activityManager.UpdateActivity(activity, result =>
            {
                //Debug.Log("Discord Result: " + result + activity.State);
            });
        }

    void OnDisable()
    {
        Debug.Log("Discord: shutdown");
        discord.Dispose();
    }
    }
}