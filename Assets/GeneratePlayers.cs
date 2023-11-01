using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GeneratePlayers : MonoBehaviour
{
    public GameObject playerCardPrefab; 
    public GameObject myGamePrefab;
    public GameObject mygame;
    public int numberOfPlayersToGenerate ; 
    public Canvas canvas;
    public List<GameObject> playerCards;
    public WinnerPanel winnerPanel;

    public PlayerTurn playerTurn;
    public PlayerTurnText playerTurnText;

    public Vector3[] playerPosition;
    public float scaleFactor;

    public void GeneratePlayerCards(int numberOfPlayers)
    {
        playerCards = new List<GameObject>();
        numberOfPlayersToGenerate = numberOfPlayers;
        CardPositionHelper.GetPositionHelper(numberOfPlayersToGenerate,out playerPosition,out scaleFactor);

        for (int i = 0; i < numberOfPlayersToGenerate; i++)
        {
            playerCards.Add(Instantiate(playerCardPrefab, canvas.transform));
            playerCards[i].transform.position = playerPosition[i];
            TextMeshProUGUI textMeshPro = playerCards[i].GetComponentInChildren<TextMeshProUGUI>();
            textMeshPro.text = "Player " + (i + 1);

            Vector3 currentScale = playerCards[i].transform.localScale;
            Vector3 newScale = new Vector3(currentScale.x * scaleFactor, currentScale.y * scaleFactor, currentScale.z * scaleFactor);
            playerCards[i].transform.localScale = newScale;
        }
        mygame = Instantiate(myGamePrefab, canvas.transform);
        MyGame myGameComponent = mygame.GetComponent<MyGame>(); // Remplacez MyGameScript par le nom réel de votre script.
        if (myGameComponent != null)
        {
            myGameComponent.generatePlayers =this;
            myGameComponent.winnerPanel =winnerPanel;
            myGameComponent.playerTurn =playerTurn;
            myGameComponent.playerTurnText = playerTurnText;
            myGameComponent.InitGame(numberOfPlayersToGenerate);
        }
    }

    public PlayerCardManager[] GetPlayerCardManagers(int playerIndex)
    {
        PlayerCards playerCards = GetPlayerCards(playerIndex);
        if (playerCards != null)
        {
            Image[] cardImages = playerCards.cardImages;
            PlayerCardManager[] managers = new PlayerCardManager[cardImages.Length];

            for (int i = 0; i < cardImages.Length; i++)
            {
                managers[i] = cardImages[i].GetComponent<PlayerCardManager>();
            }

            return managers;
        }
        else
        {
            Debug.LogError("PlayerCards component not found for playerIndex: " + playerIndex);
            return new PlayerCardManager[0];
        }
    }

    public PlayerCards GetPlayerCards(int playerIndex)
    {
        return playerCards[playerIndex].GetComponent<PlayerCards>();
    }

}
