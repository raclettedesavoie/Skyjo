using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class MenuListLobby : MonoBehaviour
{
    public CloudService cloudService;

    public GameObject lobbyInfoPrefab;

    public Canvas canvas;

    public List<LobbyInfo> lobbiesInfo;

    public GameObject menuPanelLobbyCreation;

    public GameObject menuPanelLobby;

    void Start()
    {
        cloudService.OnLobbyListChanged += LobbyManager_OnLobbyListChanged;
        NoLobbyFound();
    }

    private void LobbyManager_OnLobbyListChanged(object sender, CloudService.OnLobbyListChangedEventArgs e)
    {
        UpdateLobbyList(e.lobbyList);
    }

    private void UpdateLobbyList(List<Lobby> lobbyList)
    {
        foreach (LobbyInfo lobby in lobbiesInfo)
        {
            if (lobby.gameObject != null)
            {
                Destroy(lobby.gameObject);
            }
        }
        lobbiesInfo = new List<LobbyInfo>();
        if (lobbyList.Count == 0)
        {
            NoLobbyFound();
        }
        else
        {
            foreach (Lobby lobby in lobbyList)
            {
                var lobbyInfoInstance = Instantiate(lobbyInfoPrefab, canvas.transform);

                lobbyInfoInstance.GetComponent<LobbyInfo>().lobby = lobby;
                lobbyInfoInstance.GetComponent<LobbyInfo>().cloudService = cloudService;
                lobbyInfoInstance.GetComponent<LobbyInfo>().menuListLobby = this.gameObject;
                lobbyInfoInstance.GetComponent<LobbyInfo>().menuPanelLobby = menuPanelLobby;

                lobbyInfoInstance.GetComponent<LobbyInfo>().AwakeFunction();

                lobbiesInfo.Add(lobbyInfoInstance.GetComponent<LobbyInfo>());
            }
        }
    }

    private void NoLobbyFound()
    {
        var lobbyInfoInstance = Instantiate(lobbyInfoPrefab, canvas.transform);

        lobbiesInfo.Add(lobbyInfoInstance.GetComponent<LobbyInfo>());
    }

    public void CreateLobby()
    {
        gameObject.SetActive(false);
        menuPanelLobbyCreation.SetActive(true);
    }
}
