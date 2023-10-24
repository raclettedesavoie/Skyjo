using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MyGame : MonoBehaviour
{
    public delegate void PlayerChooseCardDelegate(Player currentPlayer, int cardNumber, PlayerCards playerCards, PlayerCardManager[] playerCardManager);

    public Button DeckButton;
    public List<Card> deckPile;
    public Text deckValue;
    public bool deckPileCanInteract = true;

    public List<Card> discardPile = null;
    public Button discardButton;
    public DiscardPileManager discardPileManager;


    public List<Player> players = new();
    public List<Card> hand;
    public int currentPlayerIndex = 0;
    public int points;

    public bool isGameOver = false;

    public GeneratePlayers generatePlayers; 

    // Start is called before the first frame update
    void Start()
    {
        InitPlayer();

        InitializeDeck();
        ShuffleDeck();
        DistributeCards();
        discardPile = new List<Card>();
        MoveThePileCardToTheDiscardPile();

    }

    // Update is called once per frame
    void Update()
    {
        deckValue.text = "Nombre de carte :"+deckPile.Count.ToString();
        IsGameOver();
        if (!isGameOver)
        {
           if(deckPile.Count == 0)
            {
                deckPile = discardPile;
                discardPile = new List<Card>();
                MoveThePileCardToTheDiscardPile();
                ShuffleDeck();
            }
        }
        else
        {
            Debug.Log("La partie est terminée.");
            CalculateScores();
            AnnounceWinner();
        }
    }
    void InitPlayer()
    {
        Player player = new();
        players.Add(player);
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

    public void DrawDiscardPile()
    {
        //Player choose to pick a card from the discard pile
        if (discardPile.Count > 0 && deckPileCanInteract == true)
        {
            deckPileCanInteract = false;
            Player currentPlayer = players[currentPlayerIndex];
            var playerCards = generatePlayers.GetPlayerCards(currentPlayerIndex);
            var playerCardsManager = generatePlayers.GetPlayerCardManagers(currentPlayerIndex);

            PlayerChooseCardDelegate playerChooseCardFromDraw = PlayerChoseCardFromDraw;
            ActivatePlayerCard(playerCardsManager, playerCards, currentPlayer, PlayerChoseCardFromDraw);
        }
    }

    public void DrawDeckPile()
    {
        if (deckPile.Count > 0 && deckPileCanInteract)
        {
        deckPileCanInteract = false;
        DeckButton.image.sprite = SetCardImage(deckPile[0].value);

            Player currentPlayer = players[currentPlayerIndex];
        var playerCards = generatePlayers.GetPlayerCards(currentPlayerIndex);
        var playerCardsManager = generatePlayers.GetPlayerCardManagers(currentPlayerIndex);

        //Player choose to not pick a card
        //TODO vérifier que le n'a pas toutes ses carte retournés !
        PlayerChooseCardDelegate playerChooseCardAfterPickDraw = PlayerChooseCardAfterPickDraw;
        discardPileManager.interactAfterDrawDeckPile = true;
        discardPileManager.OnInteractAfterDrawDeckPile.AddListener(()=>PlayerChooseDraw(playerCardsManager, playerCards, currentPlayer, playerChooseCardAfterPickDraw));

        //Player choose to pick card from the deck
        PlayerChooseCardDelegate playerChooseCard = PlayerChooseCard;
        ActivatePlayerCard(playerCardsManager, playerCards,currentPlayer, playerChooseCard);
        }

    }

    public void PlayerChooseDraw(PlayerCardManager[] playerCardsManager, PlayerCards playerCards, Player currentPlayer, PlayerChooseCardDelegate del )
    {
        
        MoveThePileCardToTheDiscardPile();

        DisableInteractCard(playerCardsManager);
        ActivatePlayerCard(playerCardsManager, playerCards, currentPlayer, del);
        DisableInteractDiscardPile();
    }

    public void ActivatePlayerCard(PlayerCardManager[] playerCardsManager,PlayerCards playerCards, Player currentPlayer, PlayerChooseCardDelegate del)
    {
        for (int i = 0; i < playerCardsManager.Length; i++)
        {
            playerCardsManager[i].canInteract = true;
            int cardNumber = playerCardsManager[i].numberCard;
            playerCardsManager[i].OnInteracted.AddListener(() => {
                del(currentPlayer, cardNumber, playerCards, playerCardsManager);
            });
        }
    }

    public void DisableInteractCard(PlayerCardManager[] playerCardsManager)
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

    public void PlayerChooseCard(Player currentPlayer,int numberCard,PlayerCards playerCards, PlayerCardManager[] playerCardsManager)
    {
        currentPlayer.drawFromDrawPile = true;

        playerCards.SetCardImage(numberCard, deckPile[0].value);

        currentPlayer.ExecutePlayerTurn(deckPile, discardPile, numberCard);

        DeckButton.image.sprite = Resources.Load<Sprite>("BackCard");

        NextPlayer(playerCardsManager);
    }
    public void PlayerChoseCardFromDraw(Player currentPlayer, int numberCard, PlayerCards playerCards, PlayerCardManager[] playerCardsManager)
    {
        currentPlayer.drawFromDiscardPile = true;

        playerCards.SetCardImage(numberCard, discardPile[0].value);

        currentPlayer.ExecutePlayerTurn(deckPile, discardPile, numberCard);

        if(discardPile.Count > 0)
        {
            discardButton.image.sprite = SetCardImage(discardPile[0].value);
        }
        else
        {
            discardButton.image.sprite = null;
        }

        NextPlayer(playerCardsManager);
    }
    public void PlayerChooseCardAfterPickDraw(Player currentPlayer, int numberCard, PlayerCards playerCards, PlayerCardManager[] playerCardsManager)
    {
        //TODO gérer le cas ou la pile est vide
        playerCards.SetCardImage(numberCard, numberCard);

        NextPlayer(playerCardsManager);
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
        int cardsToDistribute = 12; // Le nombre de cartes à distribuer à chaque joueur.

        foreach (Player player in players)
        {
            for (int i = 0; i < cardsToDistribute; i++)
            {
                if (deckPile.Count > 0)
                {
                    Card card = deckPile[deckPile.Count - 1]; // Prenez la carte du dessus du deck.
                    deckPile.RemoveAt(deckPile.Count - 1); // Retirez la carte du deck.
                    player.hand.Add(card); // Ajoutez la carte à la main du joueur.
                }
                else
                {
                    // Gérez le cas où le deck est vide (si le deck devait être épuisé dans votre jeu).
                }
            }
        }
    }

    void IsGameOver()
    {
        foreach (Player player in players)
        {
            if (player.hand.Count == 0)
            {
                // Si la main d'un joueur est vide, cela signifie qu'il a dévoilé sa dernière carte.
                isGameOver= true; // La partie est terminée.
            }
        }

        isGameOver= false; // La partie n'est pas encore terminée.
    }

    void NextPlayer(PlayerCardManager[] playerCardsManager)
    {
        upadateDeckCount();
        DisableInteractCard(playerCardsManager);
        DisableInteractDiscardPile();
        deckPileCanInteract = true;
        // Passez au joueur suivant.
        /*currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;*/
    }

    void CalculateScores()
    {
        foreach (Player player in players)
        {
            player.CountPoints(); // Vous devrez implémenter cette méthode dans la classe Player.
        }
    }

    void AnnounceWinner()
    {
        Player winner = players[0];
        foreach (Player player in players)
        {
            if (player.points < winner.points)
            {
                winner = player;
            }
        }

        Debug.Log("Le gagnant est le joueur  avec " + winner.points + " points.");
    }

    Sprite SetCardImage(int cardValue)
    {
        return Resources.Load<Sprite>($"{cardValue}");
    }

    private void upadateDeckCount()
    {
        deckValue.text = "Nombre de carte :" + deckPile.Count.ToString();
    }

    public void MoveThePileCardToTheDiscardPile()
    {
        DeckButton.image.sprite = Resources.Load<Sprite>("BackCard");
        discardButton.image.sprite = SetCardImage(deckPile[0].value);
        discardPile.Add(deckPile[0]);
        deckPile.RemoveAt(0);
    }
}
