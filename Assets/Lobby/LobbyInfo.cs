using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyInfo : MonoBehaviour
{
    public Lobby lobby;

    public TextMeshProUGUI lobbyName;

    public TextMeshProUGUI lobbyNumberOfPlayer;

    public CloudService cloudService;

    public GameObject menuListLobby;
    public GameObject menuPanelLobby;

    public void AwakeFunction()
    {
        if (lobby != null)
        {
            GetComponentInChildren<Button>().onClick.AddListener(() =>
            {
                menuListLobby.SetActive(false);
                menuPanelLobby.SetActive(true);
                cloudService.JoinLobbyByName(lobby, "Player" + (lobby.MaxPlayers - lobby.AvailableSlots + 1), UnityEngine.Random.Range(0, SpriteRandomHelper.sprites.Length).ToString());
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (lobby != null)
        {
            lobbyName.text = lobby.Name;
            lobbyNumberOfPlayer.text = lobby.Players.Count + "/" + lobby.MaxPlayers;
        }
        else
        {
            lobbyName.text = "No lobbies found ";
        }
    }
}
