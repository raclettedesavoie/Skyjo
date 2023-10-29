using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player 
{
    public List<Card> hand =new List<Card>();
    public int points;
    public List<Card> deckPile;
    public List<Card> discardPile;

    public bool drawFromDrawPile = false;
    public bool drawFromDiscardPile = false;

    public void InitialiseHand()
    {
        hand = new List<Card>();
    }
    public void ExecutePlayerTurn(List<Card> deckPile, List<Card> discardPile, int numberCard)
    {
            if (drawFromDrawPile)
            {
                DrawCardFromDrawPile(deckPile, numberCard);
            }
            if(drawFromDiscardPile)
            {
                DrawCardFromDiscardPile(discardPile, numberCard);
            }

        // Gérez d'autres actions du joueur, telles que jouer des cartes, échanger des cartes, etc.
    }

    private void DrawCardFromDrawPile(List<Card> deckPile, int numberCard)
    {
        if (deckPile.Count > 0)
        {
            Card card = deckPile[0];
            deckPile.RemoveAt(0);
            hand[numberCard - 1].value = card.value;
        }
        drawFromDrawPile = false;
    }

    private void DrawCardFromDiscardPile(List<Card> discardPile, int numberCard)
    {
        if (discardPile.Count > 0)
        {
            Card card = discardPile[discardPile.Count - 1];
            discardPile.RemoveAt(discardPile.Count - 1);
            hand[numberCard - 1].value = card.value;
        }
        drawFromDiscardPile= false;
    }


    public void CountPoints()
    {

        // Parcourez les cartes de la main du joueur et ajoutez les valeurs des cartes au total des points.
        foreach (Card card in hand)
        {
            points += card.value;
        }

    }

    public void RemoveCardFromHand(int numberColumn)
    {
        hand[numberColumn+8].value=0;
        hand[numberColumn+4].value=0;
        hand[numberColumn].value=0;
    }
}
