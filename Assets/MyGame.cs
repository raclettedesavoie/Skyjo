using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MyGame : MonoBehaviour
{
    public delegate void PlayerChooseCardDelegate(int cardNumber);

    public Button DeckButton;
    public List<Card> deckPile;
    public TextMeshProUGUI deckValue;
    public bool deckPileCanInteract = false;

    public List<Card> discardPile = null;
    public Button discardButton;
    public DiscardPileManager discardPileManager;

    public PlayerCards playerCards;
    public PlayerCardManager[] playerCardsManager;

    public List<Player> players = new();
    public Player currentPlayer;
    public List<Card> hand;
    public int numberOfPlayer = 4;
    public int currentPlayerIndex = 0;
    public int points;

    public bool isGameOver = false;
    public int playerLeftToPlay;

    public GeneratePlayers generatePlayers; 

    public WinnerPanel winnerPanel; 
    public WinnerText winnerText; 

    // Start is called before the first frame update
    void Start()
    {
        deckPileCanInteract = false;
        InitPlayer();
        currentPlayer = players[0];
        playerLeftToPlay = players.Count;
        InitializeDeck();
        ShuffleDeck();
        DistributeCards();
        discardPile = new List<Card>();
        MoveThePileCardToTheDiscardPile();
        PlayerRevealCardsAtStart();
    }

    // Update is called once per frame
    void Update()
    {

        deckValue.text = deckPile.Count.ToString() + " cartes";
        if(deckPile.Count == 0)
        {
            deckPile = discardPile;
            discardPile = new List<Card>();
            MoveThePileCardToTheDiscardPile();
            ShuffleDeck();
        }
    }
    void InitPlayer()
    {
        for (int i = 0; i < numberOfPlayer; i++)
        {
            players.Add(new());
        }
    }
    void InitializeDeck()
    {
        deckPile = new List<Card>();

        // Ajoutez 150 cartes à votre deck en respectant les quantités et les valeurs spécifiées.

        AddCardsToDeck(deckPile, -2, 5);
        AddCardsToDeck(deckPile, -1, 10);
        AddCardsToDeck(deckPile, 0, 15);
        for (int j = 1; j <= 12; j++)
        {
            AddCardsToDeck(deckPile, j, 10);
        }

    }

    public void PlayerRevealCardsAtStart()
    {
        currentPlayer = players[currentPlayerIndex  % players.Count];
        currentPlayer.numberCardRevealed++;
        playerCards = generatePlayers.GetPlayerCards(currentPlayerIndex % players.Count);
        playerCardsManager = generatePlayers.GetPlayerCardManagers(currentPlayerIndex % players.Count);
        PlayerChooseCardDelegate playerChooseCard = RevealCardAtStart;
        ActivatePlayerCard(playerChooseCard);
    }

    public void RevealCardAtStart(int numberCard)
    {
        playerCards.SetCardImage(numberCard, currentPlayer.hand[numberCard-1].value);

        NextPlayerAtStart();
        IsGameOver();
    }
    void NextPlayerAtStart()
    {
        DisableInteractCard();
        currentPlayerIndex = (currentPlayerIndex + 1) ;
        if(currentPlayerIndex < players.Count*2)
        {
            PlayerRevealCardsAtStart();
        }
        else {
            currentPlayerIndex = 0;
            deckPileCanInteract = true;
        }
    }
    public void DrawDiscardPile()
    {
        //Player choose to pick a card from the discard pile
        if (discardPile.Count > 0 && deckPileCanInteract == true)
        {
            deckPileCanInteract = false;
            currentPlayer = players[currentPlayerIndex];
            playerCards = generatePlayers.GetPlayerCards(currentPlayerIndex);
            playerCardsManager = generatePlayers.GetPlayerCardManagers(currentPlayerIndex);

            PlayerChooseCardDelegate playerChooseCardFromDraw = PlayerChoseCardFromDiscard;
            ActivatePlayerCard(PlayerChoseCardFromDiscard);
        }
    }

    public void DrawDeckPile()
    {
        currentPlayer = players[currentPlayerIndex % players.Count];
        if (deckPile.Count > 0 && deckPileCanInteract && currentPlayer.numberCardRevealed != 12)
        {
            deckPileCanInteract = false;
            DeckButton.image.sprite = SetCardImage(deckPile[0].value);

            playerCards = generatePlayers.GetPlayerCards(currentPlayerIndex);
            playerCardsManager = generatePlayers.GetPlayerCardManagers(currentPlayerIndex);

            //Player choose to not pick a card
            PlayerChooseCardDelegate playerRevealCard = PlayerRevealCard;
            discardPileManager.interactAfterDrawDeckPile = true;
            discardPileManager.OnInteractAfterDrawDeckPile.AddListener(()=>PlayerChooseDraw(playerRevealCard));

            //Player choose to pick card from the deck
            PlayerChooseCardDelegate playerChooseCard = PlayerChooseCardFromDeck;
            ActivatePlayerCard(playerChooseCard);
        }

    }
    public void PlayerChooseDraw(PlayerChooseCardDelegate del )
    {
        
        MoveThePileCardToTheDiscardPile();

        DisableInteractCard();
        ActivatePlayerCard(del);
        DisableInteractDiscardPile();
    }

    public void ActivatePlayerCard( PlayerChooseCardDelegate del)
    {
        for (int i = 0; i < playerCardsManager.Length; i++)
        {
            playerCardsManager[i].canInteract = true;
            int cardNumber = playerCardsManager[i].numberCard;
            playerCardsManager[i].OnInteracted.AddListener(() => {
                del( cardNumber);
            });
        }
    }

    public void DisableInteractCard()
    {
        for (int i = 0; i < playerCardsManager.Length; i++)
        {
            playerCardsManager[i].canInteract = false;
            playerCardsManager[i].OnInteracted.RemoveAllListeners();
        };
    }

    public void DisableInteractDiscardPile()
    {
        discardPileManager.OnInteractAfterDrawDeckPile.RemoveAllListeners();
    }

    public void PlayerChooseCardFromDeck(int numberCard)
    {
        currentPlayer.drawFromDrawPile = true;
        CheckIfCardIsAlreadyRevealed(numberCard);
        if (currentPlayer.numberCardRevealed == 12 )
        {
            isGameOver = true;
        }

        playerCards.SetCardImage(numberCard, deckPile[0].value);

        currentPlayer.ExecutePlayerTurn(deckPile, discardPile, numberCard);

        DeckButton.image.sprite = Resources.Load<Sprite>("BackCard");

        NextPlayer();
        IsGameOver();
    }
    public void PlayerChoseCardFromDiscard( int numberCard)
    {
        currentPlayer.drawFromDiscardPile = true;
        CheckIfCardIsAlreadyRevealed(numberCard);
        if (currentPlayer.numberCardRevealed == 12)
        {
            isGameOver = true;
        }

        playerCards.SetCardImage(numberCard, discardPile[discardPile.Count - 1].value);

        currentPlayer.ExecutePlayerTurn(deckPile, discardPile, numberCard);

        if(discardPile.Count > 0)
        {
            discardButton.image.sprite = SetCardImage(discardPile[0].value);
        }
        else
        {
            discardButton.image.color = new Color(1f, 1f, 1f, 0f);
        }

        NextPlayer();
        IsGameOver();
    }
    public void CheckIfCardIsAlreadyRevealed(int numberCard)
    {
        if (playerCards.cardImages[numberCard-1].sprite.name == "BackCard")
        {
            currentPlayer.numberCardRevealed++;
        }
    } 
    public void PlayerRevealCard(int numberCard)
    {
        currentPlayer.numberCardRevealed++;
        playerCards.SetCardImage(numberCard, currentPlayer.hand[numberCard-1].value);

        NextPlayer();
        IsGameOver();
    }


    void AddCardsToDeck(List<Card> cardList, int value, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Card newCard = new Card();
            newCard.value = value;
            cardList.Add(newCard);
        }
    }

    void ShuffleDeck()
    {
        int n = deckPile.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1); // Générez un index aléatoire entre 0 et n inclus.
            Card temp = deckPile[k];
            deckPile[k] = deckPile[n];
            deckPile[n] = temp;
        }
    }

    void DistributeCards()
    {
        int cardsToDistribute = 12; 

        foreach (Player player in players)
        {
            for (int i = 0; i < cardsToDistribute; i++)
            {
                if (deckPile.Count > 0)
                {
                    Card card = deckPile[0]; 
                    deckPile.RemoveAt(0); 
                    player.hand.Add(card); 
                }
            }
        }
    }

    public void IsGameOver()
    {
        if (isGameOver)
        {
            playerLeftToPlay--;
        }
        if (playerLeftToPlay == 0)
        {
            GameOver();
        }
    }
    void GameOver()
    {
        CalculateScores();
        AnnounceWinner();
    }

    void NextPlayer()
    {
        upadateDeckCount();
        DisableInteractCard();
        DisableInteractDiscardPile();
        deckPileCanInteract = true;
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
    }

    void CalculateScores()
    {
            int j = -1;
        foreach (Player player in players)
        {
            j++;
            for(int i = 0; i < 12; i++)
            {
                var playerCards = generatePlayers.GetPlayerCards(j);
                playerCards.SetCardImage(i+1, player.hand[i].value);
            }
            player.CountPoints(); 
        }
    }

    void AnnounceWinner()
    {
        Player winner = players[0];
        var winnerName = 0;
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].points < winner.points)
            {
                winnerName = i+1;
                winner = players[i];
            }
        }

        winnerPanel.AnnounceWinner();
        winnerText.AnnounceWinner(winnerName.ToString(),winner.points.ToString());
    }

    Sprite SetCardImage(int cardValue)
    {
        return Resources.Load<Sprite>($"{cardValue}");
    }

    private void upadateDeckCount()
    {
        deckValue.text = deckPile.Count.ToString() + " cartes";
    }

    public void MoveThePileCardToTheDiscardPile()
    {
        DeckButton.image.sprite = Resources.Load<Sprite>("BackCard");
        if(discardPile.Count==0)
        {
            discardButton.image.color = new Color(1f, 1f, 1f, 1f);
        }
        discardButton.image.sprite = SetCardImage(deckPile[0].value);
        discardPile.Add(deckPile[0]);
        deckPile.RemoveAt(0);
    }
}
