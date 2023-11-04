using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class MenuPanelLobbyCreation : MonoBehaviour
{
    public Button previousBtn;
    public Button nextBtn;
    public GeneratePlayers generatePlayers;
    public TextMeshProUGUI numberOfPlayerTxt;
    public int numberOfPlayers = 4;

    public GameObject MenuPanelLobby;
    public GameObject MenuPanelListLobby;

    public CloudService cloudService;

    public TextMeshProUGUI lobbyName;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        numberOfPlayers = 4;
        numberOfPlayerTxt.text = numberOfPlayers.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (numberOfPlayers == 8)
        {
            nextBtn.interactable = false;
        }
        else
        {
            nextBtn.interactable = true;
        }
        if (numberOfPlayers == 3)
        {
            previousBtn.interactable = false;
        }
        else
        {
            previousBtn.interactable = true;
        }
    }

    public void updatePlayerName(string updatedlobbyName)
    {
        lobbyName.text = updatedlobbyName;
    }
    public void NextBtnPressed()
    {
        numberOfPlayers++;
        numberOfPlayerTxt.text = numberOfPlayers.ToString();
    }

    public void PreviousBtnPressed()
    {
        numberOfPlayers--;
        numberOfPlayerTxt.text = numberOfPlayers.ToString();
    }

    public void CreateLobby()
    {
        var lobbyNameLocal = lobbyName.text.Length== 1? "lobby" + UnityEngine.Random.Range(1000, 10000) : lobbyName.text;
        cloudService.CreateLobby(false, lobbyNameLocal, "Player 1", numberOfPlayers, UnityEngine.Random.Range(0, SpriteRandomHelper.sprites.Length).ToString());
        gameObject.SetActive(false);
        MenuPanelLobby.SetActive(true);
    }

    public void LeaveCreateLobby()
    {
        gameObject.SetActive(false);
        MenuPanelListLobby.SetActive(true);
    }
}
