using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class MenuPanelLobby : MonoBehaviour
{
    public CloudService cloudService;

    public Canvas canvas;

    public List<PlayerInfo> playersInfo;

    public GameObject playerInfoPrefab;

    public Lobby joinedLobby;

    public GeneratePlayers generatePlayers;

    public Button leaveLobbyButton;

    public GameObject menuListLobby;
    private void Awake()
    {
        //TODO changeLogo & change Name
        /*changeLogoButton.onClick.AddListener(() => {
            LobbyManager.Instance.UpdatePlayerCharacter(LobbyManager.PlayerCharacter.Marine);
        });*/
        leaveLobbyButton.onClick.AddListener(() => {
            cloudService.LeaveLobby();
        });
    }

    void Start()
    {
        gameObject.SetActive(false);
        cloudService.OnJoinedLobby += UpdateLobby_Event;
        cloudService.OnJoinedLobbyUpdate += UpdateLobby_Event;
        cloudService.OnLobbyGameModeChanged += UpdateLobby_Event;
        cloudService.OnLeftLobby += LobbyManager_OnLeftLobby;
        cloudService.OnKickedFromLobby += LobbyManager_OnLeftLobby;

        Hide();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void LobbyManager_OnLeftLobby(object sender, System.EventArgs e)
    {
        ClearLobby();
        Hide();
        menuListLobby.SetActive(true);
    }

    private void UpdateLobby_Event(object sender, CloudService.LobbyEventArgs e)
    {
        UpdateLobby();
    }

    private void UpdateLobby()
    {
        UpdateLobby(cloudService.GetJoinedLobby());
    }

    private void UpdateLobby(Lobby lobby)
    {
        ClearLobby();

        joinedLobby = lobby;

        foreach (Unity.Services.Lobbies.Models.Player player in lobby.Players)
        {
            var playerInfoInstanceGameObject = Instantiate(playerInfoPrefab, canvas.transform);

            var playerInfoInstance = playerInfoInstanceGameObject.GetComponent<PlayerInfo>();

            playerInfoInstance.cloudService = cloudService;

            playersInfo.Add(playerInfoInstance);

            playerInfoInstance.SetKickPlayerButtonVisible(
                cloudService.IsLobbyHost() &&
                player.Id != AuthenticationService.Instance.PlayerId // Don't allow kick self
            );

            playerInfoInstance.UpdatePlayer(player);
        }

    }

    private void ClearLobby()
    {
        foreach (PlayerInfo playerInfo in playersInfo)
        {
            if (playerInfo.gameObject!= null)
            {
                Destroy(playerInfo.gameObject);
            }
        }
        playersInfo = new List<PlayerInfo>();
    }

    public void StartGame()
    {
        if(joinedLobby != null /*&& joinedLobby.MaxPlayers -joinedLobby.AvailableSlots > 2*/)
        {
            gameObject.SetActive(false);
            generatePlayers.GeneratePlayerCards(joinedLobby.MaxPlayers - joinedLobby.AvailableSlots);
        }
    }
}
