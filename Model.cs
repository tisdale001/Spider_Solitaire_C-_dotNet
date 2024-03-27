using System;/*
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;*/
using System.Collections.Generic;


namespace Spider_Solitaire
{
    class Model
    {
        private List<Card> mainPile1 = new List<Card>();
        private List<Card> mainPile2 = new List<Card>();
        private List<Card> mainPile3 = new List<Card>();
        private List<Card> mainPile4 = new List<Card>();
        private List<Card> mainPile5 = new List<Card>();
        private List<Card> mainPile6 = new List<Card>();
        private List<Card> mainPile7 = new List<Card>();
        private List<Card> mainPile8 = new List<Card>();
        private List<Card> mainPile9 = new List<Card>();
        private List<Card> mainPile10 = new List<Card>();
        private List<Card> foundationPile1 = new List<Card>();
        private List<Card> foundationPile2 = new List<Card>();
        private List<Card> foundationPile3 = new List<Card>();
        private List<Card> foundationPile4 = new List<Card>();
        private List<Card> foundationPile5 = new List<Card>();
        private List<Card> foundationPile6 = new List<Card>();
        private List<Card> foundationPile7 = new List<Card>();
        private List<Card> foundationPile8 = new List<Card>();
        private List<Card> drawPile1 = new List<Card>();
        private List<Card> drawPile2 = new List<Card>();
        private List<Card> drawPile3 = new List<Card>();
        private List<Card> drawPile4 = new List<Card>();
        private List<Card> drawPile5 = new List<Card>();
        private List<Card> movePile = new List<Card>();
        private List<Card> dealPile = new List<Card>();

        private Deck deck1 = new Deck();
        private Deck deck2 = new Deck();

        public Model()
        {
            allDeckCardsToDealPile(deck2);
            allCardsReturnToDeck(deck1);
            shuffleCards();
        }

        public void allDeckCardsToDealPile(Deck deck)
        {
            while (!deck.isEmpty())
            {
                deck.dealCard(dealPile);
            }
        }

        public void allCardsReturnToDeck(Deck deck)
        {
            for (int i = 1; i < 11; i++)
            {
                deck.returnCards(getMainPile(i));
            }
            for (int i = 1; i < 9; i++)
            {
                deck.returnCards(getFoundationPile(i));
            }
            for (int i = 1; i < 6; i++)
            {
                deck.returnCards(getDrawPile(i));
            }
            deck.returnCards(movePile);
            deck.returnCards(dealPile);
        }

        public void shuffleCards()
        {
            deck1.shuffleDeck();
        }

        public void cardsToMove(List<Card> fromPile, int index)
        {
            int size = fromPile.Count - index;
            for (int i = size; i > 0; i--)
            {
                movePile.Add(fromPile[fromPile.Count - 1]);
                fromPile.RemoveAt(fromPile.Count - 1);
            }
        }

        public void cardsFromMove(List<Card> toPile)
        {
            while(movePile.Count != 0)
            {
                toPile.Add(movePile[movePile.Count - 1]);
                movePile.RemoveAt(movePile.Count - 1);
            }
        }

        public void dealOneCardFromPile(List<Card> fromPile, List<Card> toPile)
        {
            if (fromPile.Count != 0)
            {
                toPile.Add(fromPile[fromPile.Count - 1]);
                fromPile.RemoveAt(fromPile.Count - 1);
            }
        }

        // reveals top card of pile: returns true if card is turned over
        public bool revealCard(List<Card> pile)
        {
            if (pile.Count != 0)
            {
                if (!pile[pile.Count - 1].FaceUp)
                {
                    pile[pile.Count - 1].FaceUp = true;
                    return true;
                }
            }
            return false;
        }

        //getter: returns appropriate mainPile
        public List<Card> getMainPile(int num)
        {
            switch (num)
            {
                case 1:
                    return mainPile1;
                case 2:
                    return mainPile2;
                case 3:
                    return mainPile3;
                case 4:
                    return mainPile4;
                case 5:
                    return mainPile5;
                case 6:
                    return mainPile6;
                case 7:
                    return mainPile7;
                case 8:
                    return mainPile8;
                case 9:
                    return mainPile9;
                case 10:
                    return mainPile10;
                default:
                    return null;
            }
        }

        //getter: returns appropriate foundation pile
        public List<Card> getFoundationPile(int num)
        {
            switch (num)
            {
                case 1:
                    return foundationPile1;
                case 2:
                    return foundationPile2;
                case 3:
                    return foundationPile3;
                case 4:
                    return foundationPile4;
                case 5:
                    return foundationPile5;
                case 6:
                    return foundationPile6;
                case 7:
                    return foundationPile7;
                case 8:
                    return foundationPile8;
                default:
                    return null;
            }
        }

        //getter: returns appropriate draw pile
        public List<Card> getDrawPile(int num)
        {
            switch(num)
            {
                case 1:
                    return drawPile1;
                case 2:
                    return drawPile2;
                case 3:
                    return drawPile3;
                case 4:
                    return drawPile4;
                case 5:
                    return drawPile5;
                default:
                    return null;
            }
        }

        // getter: returns movePile
        public List<Card> getMovePile()
        {
            return movePile;
        }

        // getter: returns dealPile
        public List<Card> getDealPile()
        {
            return dealPile;
        }

        //getter: returns deck1
        public Deck getDeck()
        {
            return deck1;
        }

        // getter: returns the next/correct drawPile or null if all draw piles are empty
        public List<Card> getCorrectDrawPile()
        {
            for (int i = 5; i > 0; i--)
            {
                if (getDrawPile(i).Count != 0)
                {
                    return getDrawPile(i);
                }
            }
            return null;
        }

        // returns true if pile is a draw pile
        public bool checkDrawPile(List<Card> pile)
        {
            for (int i = 5; i > 0; i--)
            {
                if (pile == getDrawPile(i))
                {
                    return true;
                }
            }
            return false;
        }

        // getter: return correct/NEXT foundation pile
        public List<Card> getCorrectFoundationPile()
        {
            for (int i = 8; i > 0; i--)
            {
                if (getFoundationPile(i).Count == 0)
                {
                    return getFoundationPile(i);
                }
            }
            return null;
        }

    }
}
