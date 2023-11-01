using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinnerPanel : MonoBehaviour
{
    public MyGame myGame;
    public MenuStartPanel menuStartPanel;
    public GeneratePlayers generatePlayers;
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void AnnounceWinner(string winnerText )
    {
        gameObject.SetActive(true);
        GetComponentInChildren<TextMeshProUGUI>().text = winnerText;
    }

    public void Replay()
    {
        if (generatePlayers.mygame != null)
        {
            Destroy(generatePlayers.mygame);
        }
        if (generatePlayers.playerCards != null)
        {
            foreach(GameObject playerCards in generatePlayers.playerCards)
            {
                Destroy(playerCards);
            }
        }
        gameObject.SetActive(false);
        menuStartPanel.gameObject.SetActive(true);
    }

    public void Leave()
    {
        Application.Quit();
    }
}
