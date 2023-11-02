using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudCode;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

/*
 * Note: You must have a published script to use the Cloud Code SDK.
 * You can publish a script from the Unity Dashboard - https://dashboard.unity3d.com/
 */
public class CloudService : MonoBehaviour
{
    /*
     * CloudCodeResponse represents the response from the script, used for deserialization.
     * In this example, the script returns a JSON in the format
     * {"welcomeMessage": "Hello, arguments['name']. Welcome to Cloud Code!"}
     */

    private Lobby hostLobby;
    private Lobby joinedLobby;

    private float heartBeatTime;
    private float lobbyPollTimer;
    private float refreshLobbyListTimer;

    public event EventHandler OnLeftLobby;

    public event EventHandler<LobbyEventArgs> OnJoinedLobby;
    public event EventHandler<LobbyEventArgs> OnJoinedLobbyUpdate;
    public event EventHandler<LobbyEventArgs> OnKickedFromLobby;
    public event EventHandler<LobbyEventArgs> OnLobbyGameModeChanged;

    private string playerName;
    private string playerLogo;

    public class LobbyEventArgs : EventArgs
    {
        public Lobby lobby;
    }

    public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
    public class OnLobbyListChangedEventArgs : EventArgs
    {
        public List<Lobby> lobbyList;
    }
    class CloudCodeResponse
    {
        public string welcomeMessage;
    }

    /*
     * Initialize all Unity Services and Sign In an anonymous player.
     * You can perform this operation in a more centralized spot in your project
     */
    public async void Awake()
    {
        //Initialize options to make on same pc
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("signed in" + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void CreateLobby(bool isPrivate, string playerName,int numberOfPlayerMax, string logoPlayerName)
    {
        try
        {
                CreateLobbyOptions options = new CreateLobbyOptions()
                {
                    IsPrivate = isPrivate,
                    Player = GetPlayer(playerName, logoPlayerName),
/*                    Data =new Dictionary<string, DataObject>
                    {
                        {"GameMode",new DataObject(DataObject.VisibilityOptions.Public, "CaptureTheFlag",DataObject.IndexOptions.S1) }
                    }*/
                };

                Lobby lobby = await LobbyService.Instance.CreateLobbyAsync("test", numberOfPlayerMax, options);

                Debug.Log("Created lobby " + lobby.Name + " " + lobby.MaxPlayers +" " + lobby.LobbyCode);

                hostLobby = lobby;
                joinedLobby = hostLobby;

                OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
        }
        catch(LobbyServiceException ex)
        {
            Debug.Log(ex);
        }
    }

    private void Update()
    {
        HandleLobbyHeartBeat();
        HandleLobbyPolling();
        HandleRefreshLobbyList();
    }

    private async void HandleLobbyHeartBeat()
    {
        if( hostLobby != null )
        {
            heartBeatTime -= Time.deltaTime;
            if(heartBeatTime < 0)
            {
                float heartBeatTimeMax = 15;
                heartBeatTime = heartBeatTimeMax;
                try
                {
                    await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
                }
                catch (LobbyServiceException e)
                {
                    Debug.Log(e);
                }
            }
        }
    }

    private void HandleRefreshLobbyList()
    {
        if(UnityServices.State == ServicesInitializationState.Initialized && AuthenticationService.Instance.IsSignedIn)
        {
            refreshLobbyListTimer -= Time.deltaTime;
            if (refreshLobbyListTimer < 0 )
            {
                float refreshLobbyListTimerMax = 2f;
                refreshLobbyListTimer = refreshLobbyListTimerMax;
                RefreshLobbyList();  
            }
        }
    }
    /*
     * Populate a Dictionary<string,object> with the arguments and invoke the script.
     * Deserialize the response into a CloudCodeResponse object
     */
    private async void HandleLobbyPolling()
    {
        if (joinedLobby != null)
        {
            lobbyPollTimer -= Time.deltaTime;
            if (lobbyPollTimer < 0f)
            {
                float lobbyPollTimerMax = 1.1f;
                lobbyPollTimer = lobbyPollTimerMax;

                joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);

                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });

                if (!IsPlayerInLobby())
                {
                    // Player was kicked out of this lobby
                    Debug.Log("Kicked from Lobby!");

                    OnKickedFromLobby?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });

                    joinedLobby = null;
                }
            }
        }
    }
    private bool IsPlayerInLobby()
    {
        if (joinedLobby != null && joinedLobby.Players != null)
        {
            foreach (Unity.Services.Lobbies.Models.Player player in joinedLobby.Players)
            {
                if (player.Id == AuthenticationService.Instance.PlayerId)
                {
                    // This player is in this lobby
                    return true;
                }
            }
        }
        return false;
    }
    public bool IsLobbyHost()
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    public async void OnClick()
    {
        var arguments = new Dictionary<string, object> { { "name", "Unity" } };
        var response = await CloudCodeService.Instance.CallEndpointAsync<CloudCodeResponse>("hello-world", arguments);
    }

    public async void ListLobbies()
    {
        try
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            Debug.Log("Lobbies found : " + queryResponse.Results.Count);

            foreach( Lobby lobby in queryResponse.Results )
            {
                Debug.Log(lobby.Name + " " + lobby.MaxPlayers);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async Task<string> ListLobbiesByName(string lobbyName)
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions()
            {
                Filters= new List<QueryFilter> {
                    new QueryFilter(QueryFilter.FieldOptions.Name,lobbyName,QueryFilter.OpOptions.EQ),
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots,"0",QueryFilter.OpOptions.GT)
                }
            };

            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            if (queryResponse.Results.Count > 0)
            {
                foreach (Lobby lobby in queryResponse.Results)
                {
                    Debug.Log(lobby.Name + " " + lobby.MaxPlayers);
                }
                return queryResponse.Results[0].Id; 
            }
            else
            {
                Debug.Log("Aucun lobby ne répond aux critères.");
                return "";
            }
        }
        catch (LobbyServiceException e)
        {
            return "";
            Debug.Log(e);
        }
    }

    public async void JoinLobbyByName(Lobby lobby, string playerName,string logoPlayer)
    {
        try
        {
            JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions()
            {
                Player = GetPlayer(playerName, logoPlayer),
            };

            joinedLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id, joinLobbyByIdOptions);

            OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void JoinLobbyByCode(string LobbyCode,string playerName, string logoPlayer)
    {
        try
        {
            JoinLobbyByCodeOptions joinLobbyByIdOptions = new JoinLobbyByCodeOptions()
            {
                Player = GetPlayer(playerName, logoPlayer),
            };

            joinedLobby= await Lobbies.Instance.JoinLobbyByCodeAsync(LobbyCode);

            OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });

            Debug.Log("joined lobby");
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void QuickJoinLobby()
    {
        try
        {
            Lobby lobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public void SendPlayerData(Lobby lobby)
    {
        foreach(Unity.Services.Lobbies.Models.Player player in lobby.Players)
        {
            Debug.Log(player.Id + " " + player.Data["PlayerName"].Value /*+ lobby.Data["GameMode"].value*/);
        }
    } 
    
    public void SendJoinedPlayerData()
    {
        SendPlayerData(joinedLobby);
    }

    private Unity.Services.Lobbies.Models.Player GetPlayer(string playerName,string logoPlayer)
    {
        return new Unity.Services.Lobbies.Models.Player()
        {
            Data = new Dictionary<string, PlayerDataObject>
                {
                    {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member ,playerName) },
                    {"PlayerLogo", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,logoPlayer)}
                }
        };
    }
    public async void UpdatePlayerName(string playerName)
    {
        this.playerName = playerName;

        if (joinedLobby != null)
        {
            try
            {
                UpdatePlayerOptions options = new UpdatePlayerOptions();

                options.Data = new Dictionary<string, PlayerDataObject>() {
                    {
                        "PlayerName", new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Public,
                            value: playerName)
                    }
                };

                string playerId = AuthenticationService.Instance.PlayerId;

                Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, playerId, options);
                joinedLobby = lobby;

                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }
    public Lobby GetJoinedLobby()
    {
        return joinedLobby;
    }

    public async void UpdatePlayerLogo(string playerLogo)
    {
        this.playerLogo = playerLogo;

        if (joinedLobby != null)
        {
            try
            {
                UpdatePlayerOptions options = new UpdatePlayerOptions();

                options.Data = new Dictionary<string, PlayerDataObject>() {
                    {
                        "PlayerLogo", new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Public,
                            value: playerName)
                    }
                };

                string playerId = AuthenticationService.Instance.PlayerId;

                Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, playerId, options);
                joinedLobby = lobby;

                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public async void KickPlayer(string playerId)
    {
        if (IsLobbyHost())
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    private async void DeleteLobby()
    {
        try
        {
            await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
        }
        catch (LobbyServiceException e) { Debug.Log(e); }
    }

    public async void RefreshLobbyList()
    {
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 25;

            // Filter for open lobbies only
            options.Filters = new List<QueryFilter> {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0")
            };

            // Order by newest lobbies first
            options.Order = new List<QueryOrder> {
                new QueryOrder(
                    asc: false,
                    field: QueryOrder.FieldOptions.Created)
            };

            QueryResponse lobbyListQueryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs { lobbyList = lobbyListQueryResponse.Results });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void LeaveLobby()
    {
        if (joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);

                joinedLobby = null;

                OnLeftLobby?.Invoke(this, EventArgs.Empty);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }
}
