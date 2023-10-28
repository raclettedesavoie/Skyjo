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

    public int numberCardRevealed = 0;

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
            hand.RemoveAt(numberCard-1);
            hand.Insert(numberCard - 1, card);
        }
    }

    private void DrawCardFromDiscardPile(List<Card> discardPile, int numberCard)
    {
        if (discardPile.Count > 0)
        {
            Card card = discardPile[discardPile.Count - 1];
            discardPile.RemoveAt(discardPile.Count - 1);
            hand.RemoveAt(numberCard - 1);
            hand.Insert(numberCard - 1, card);
        }
    }


    public void CountPoints()
    {

        // Parcourez les cartes de la main du joueur et ajoutez les valeurs des cartes au total des points.
        foreach (Card card in hand)
        {
            points += card.value;
        }

        // Gérez les règles spéciales, par exemple, si le joueur a dévoilé ou placé 3 cartes identiques dans une colonne.
        // Si cette règle s'applique, soustrayez le nombre de points approprié.
    }
}
