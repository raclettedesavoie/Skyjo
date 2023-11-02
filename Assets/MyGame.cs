using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Services.Core;
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
    public int numberOfPlayers;
    public int currentPlayerIndex = 0;
    public int points;

    public bool isGameOver = false;
    public int playerLeftToPlay;

    public GeneratePlayers generatePlayers; 

    public WinnerPanel winnerPanel; 

    public PlayerTurn playerTurn;
    public PlayerTurnText playerTurnText;

    private float timeSinceLastAction = 0f;
    public bool deckPileInteracted = false;
    public bool actionWasMade = false;

    public int numberCardRevealed = 0;

    public static ServicesInitializationState State;

    public void InitGame(int nOP)
    {
        numberOfPlayers = nOP;
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
        playerTurnText.SetText("Player " + (currentPlayerIndex + 1) + "'s turn \n Choose two cards to reveal");
        StartCoroutine(playerTurn.ActivatePlayerTurn());
    }

    // Update is called once per frame
    void Update()
    {
        if (numberOfPlayers != 0)
        {
            deckValue.text = deckPile.Count.ToString() + " cartes";
            if(deckPile.Count == 0)
            {
                deckPile = discardPile;
                discardPile = new List<Card>();
                MoveThePileCardToTheDiscardPile();
                ShuffleDeck();
            }
           /* if (actionWasMade)
            {
                timeSinceLastAction = 0f;
                actionWasMade = false;
            }
            else
            {
                timeSinceLastAction += Time.deltaTime;

                if (timeSinceLastAction >= 10f)
                {
                    actionWasMade = true;
                    if(deckPileCanInteract == true)
                    {
                        StartCoroutine(StartDisplayHelpAfterDelay(null, discardButton.image));
                        StartCoroutine(StartDisplayHelpAfterDelay(null, DeckButton.image));
                    }
                    else if(deckPileInteracted == true)
                    {
                        StartCoroutine(StartDisplayHelpAfterDelay(playerCards.cardImages));
                        StartCoroutine(StartDisplayHelpAfterDelay(null, discardButton.image)); ;
                    }
                    else
                    {
                        StartCoroutine(StartDisplayHelpAfterDelay(playerCards.cardImages));
                    }
                }
            }*/
        }
    }
    void InitPlayer()
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
            players.Add(new());
        }
    }
    void InitializeDeck()
    {
        deckPile = new List<Card>();

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
        playerCards = generatePlayers.GetPlayerCards(currentPlayerIndex );
        playerCardsManager = generatePlayers.GetPlayerCardManagers(currentPlayerIndex);

        PlayerChooseCardDelegate playerChooseCard = RevealCardAtStart;
        ActivatePlayerCard(playerChooseCard);
    }

    public void RevealCardAtStart(int numberCard)
    {
        actionWasMade = true;
        playerCards.SetCardImage(numberCard, currentPlayer.hand[numberCard-1].value);
        currentPlayer.hand[numberCard - 1].cardRevealed = true;
        NextPlayerAtStart();
    }
    void NextPlayerAtStart()
    {
        if (numberCardRevealed == 0)
        {
            numberCardRevealed++;
        }
        else
        {
            numberCardRevealed = 0;
            currentPlayerIndex++;
        }
        DisableInteractCard();

        if (currentPlayerIndex < players.Count)
        {
            PlayerRevealCardsAtStart();
            playerTurnText.SetText("Player " + (currentPlayerIndex + 1) + "'s turn \n Choose two cards to reveal");
        }
        else
        {
            currentPlayerIndex = 0;
            deckPileCanInteract = true;
            playerTurnText.SetText("Player " + (currentPlayerIndex + 1) + "'s turn ");
        }

        StartCoroutine(playerTurn.ActivatePlayerTurn());    
        
        actionWasMade = false;
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
        currentPlayer = players[currentPlayerIndex];
        if (deckPile.Count > 0 && deckPileCanInteract && currentPlayer.hand.Where(card => card.cardRevealed).Count() != 12)
        {
            deckPileCanInteract = false;
            deckPileInteracted = true;
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
        actionWasMade = true;
        currentPlayer.drawFromDrawPile = true;
        CheckIfCardIsAlreadyRevealed(numberCard);
        if (currentPlayer.hand.Where(card=>card.cardRevealed).Count() == 12 )
        {
            isGameOver = true;
        }

        playerCards.SetCardImage(numberCard, deckPile[0].value);

        currentPlayer.ExecutePlayerTurn(deckPile, discardPile, numberCard);

        DeckButton.image.sprite = Resources.Load<Sprite>("BackCard");

        NextPlayer();
        IsGameOver();
    }
    public void PlayerChoseCardFromDiscard(int numberCard)
    {
        actionWasMade = true;
        currentPlayer.drawFromDiscardPile = true;
        CheckIfCardIsAlreadyRevealed(numberCard);
        if (currentPlayer.hand.Where(card => card.cardRevealed).Count() == 12 )
        {
            isGameOver = true;
        }

        playerCards.SetCardImage(numberCard, discardPile[0].value);

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
        if (currentPlayer.hand[numberCard-1].cardRevealed == false)
        {
            currentPlayer.hand[numberCard-1].cardRevealed = true;
        }
    } 
    public void PlayerRevealCard(int numberCard)
    {
        actionWasMade = true;
        if (currentPlayer.hand[numberCard - 1].cardRevealed == false)
        {
            CheckIfCardIsAlreadyRevealed(numberCard);
            playerCards.SetCardImage(numberCard, currentPlayer.hand[numberCard-1].value);
            NextPlayer();
            IsGameOver();
        }
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
        //display Help If needed
        actionWasMade = false;
        deckPileInteracted = false;
        deckValue.text = deckPile.Count.ToString() + " cartes";
        CheckIdenticalColumnManager();
        DisableInteractCard();
        DisableInteractDiscardPile();
        deckPileCanInteract = true;
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        playerTurnText.SetText("Player " + (currentPlayerIndex + 1) + "'s turn");
        StartCoroutine(playerTurn.ActivatePlayerTurn());
    }

    Sprite SetCardImage(int cardValue)
    {
        return Resources.Load<Sprite>($"{cardValue}");
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

    public void CheckIdenticalColumnManager()
    {
        for (int i = 0; i < 4; i++){
            var allCardsMatch = true;
            var cards = new List<Card> { currentPlayer.hand[i], currentPlayer.hand[i + 4], currentPlayer.hand[i + 8] };
            for (int j = 0; j < cards.Count; j++)
            {
                if (!cards[j].cardRevealed || cards[j].value != cards[0].value)
                {
                    allCardsMatch = false;
                    break;
                }
            }
            if(allCardsMatch)
            {
                currentPlayer.RemoveCardFromHand(i);
                playerCards.RemoveCardFromScene(i);
                playerCardsManager[i].DisableGameObject();
                playerCardsManager[i+4].DisableGameObject();
                playerCardsManager[i+8].DisableGameObject();
            }
        }
    }
    void CalculateScores()
    {
        int j = -1;
        foreach (Player player in players)
        {
            j++;
            for (int i = 0; i < 12; i++)
            {
                var playerCards = generatePlayers.GetPlayerCards(j);
                playerCards.SetCardImage(i + 1, player.hand[i].value);
            }
            player.CountPoints();
        }
    }

    void AnnounceWinner()
    {
        Player winner = players[0];
        var winnerName = 1;
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].points < winner.points)
            {
                winnerName = i + 1;
                winner = players[i];
            }
        }

        winnerPanel.AnnounceWinner("Le gagnant est le joueur " + winnerName + " avec " + winner.points + " points.");
    }


    private IEnumerator InvokeScaleImageWithDelay(Image image, float delay)
    {
        yield return new WaitForSeconds(delay); // Délai de 2 secondes.
        if (!actionWasMade)
        {
            StartCoroutine(ScaleImageEffect(image));
        }
    }
    private IEnumerator ScaleImageEffect(Image image)
    {
        var originalScale = image.transform.localScale;
        // Agrandir l'image
        float timer = 0f;

            while (timer < 1)
            {
                timer += Time.deltaTime;
                float scale = Mathf.Lerp(1, 1.1f, timer / 1); // Agrandir à 1.5x la taille d'origine
                image.transform.localScale = originalScale * scale;
                yield return null;
            }

            // Réduire l'image
            timer = 0f;
            while (timer < 1)
            {
                timer += Time.deltaTime;
                float scale = Mathf.Lerp(1.1f, 1, timer / 1); // Réduire à la taille d'origine
                image.transform.localScale = originalScale * scale;
                yield return null;
            }

        yield return new WaitForSeconds(2f);
    }

    IEnumerator StartDisplayHelpAfterDelay(Image[] images=null, Image image=null)
    {
        yield return new WaitForSeconds(2f);
        if (images != null)
        {
            foreach (Image i in images)
            {
                StartCoroutine(ScaleImageEffect(i));
            }
        }
        if (image != null)
        {
            StartCoroutine(ScaleImageEffect(image));
        }
        
    }

    public void ResetGame()
    {
        DeckButton = null;
        deckPile=null;
        deckValue=null;
        deckPileCanInteract =false;
        discardPile =null;
        discardButton=null;
        discardPileManager=null;
        playerCards=null;
        playerCardsManager=null;
        players =null;
        currentPlayer=null;
        hand=null;
        numberOfPlayers=0;
        currentPlayerIndex =0;
        points=0;
        isGameOver =false;
        playerLeftToPlay=0;

        generatePlayers = null;

        winnerPanel = null;    
        playerTurn = null;
        playerTurnText = null;
        timeSinceLastAction = 0f;
        deckPileInteracted = false;
        actionWasMade = false;

        numberCardRevealed = 0;
    }
    public void ReplayGame()
    {
        var nOP = numberOfPlayers;
        ResetGame();
        InitGame(nOP);
    }
}
