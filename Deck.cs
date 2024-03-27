using System;
using System.IO;
using System.Collections.Generic;
using SFML.Graphics;

namespace Spider_Solitaire
{
    class Deck
    {
        private List<Card> deckOfCards = new List<Card>();
        private List<Image> imageList = new List<Image>();
        private List<Image> backImageList = new List<Image>();
        private List<Texture> frontTextureList = new List<Texture>();
        private List<Texture> backTextureList = new List<Texture>();
        private List<Sprite> frontSpriteList = new List<Sprite>();
        private List<Sprite> backSpriteList = new List<Sprite>();
        private float scaleX = .50f;
        private float scaleY = .50f;

        public Deck()
        {
            createDeck();
            shuffleDeck();
        }

        protected void createDeck()
        {
            // create front images
            List<string> cardNameList = new List<string>{ "aceOfSpades.png", "twoOfSpades.png", "threeOfSpades.png", "fourOfSpades.png" , "fiveOfSpades.png",
                "sixOfSpades.png", "sevenOfSpades.png", "eightOfSpades.png", "nineOfSpades.png", "tenOfSpades.png", "jackOfSpades.png", "queenOfSpades.png",
                "kingOfSpades.png" };
            // load 13 images 4 times
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    Image im = new Image(Path.Combine(Environment.CurrentDirectory, "../../Images", cardNameList[j]));
                    imageList.Add(im);
                }
            }
            //create front textures
            for (int i = 0; i < 52; i++)
            {
                Texture tx = new Texture(imageList[i]);
                frontTextureList.Add(tx);
            }
            // create front sprites
            for (int i = 0; i < 52; i++)
            {
                // TODO: need to set scale or size here.....
                Sprite frontSprite = new Sprite(frontTextureList[i]);
                frontSprite.Scale = new SFML.System.Vector2f(scaleX, scaleY);
                frontSpriteList.Add(frontSprite);
            }
            // create back image
            Image backImage = new Image(Path.Combine(Environment.CurrentDirectory, "../../Images", "backOfCard.png"));
            backImageList.Add(backImage);
            // create back texture
            Texture backTexture = new Texture(backImageList[0]);
            backTextureList.Add(backTexture);
            // create back sprites
            for (int i = 0; i < 52; i++)
            {
                Sprite backSprite = new Sprite(backTextureList[0]);
                // set scale here
                backSprite.Scale = new SFML.System.Vector2f(scaleX, scaleY);
                backSpriteList.Add(backSprite);
            }
            // create cards
            List<string> symbolList = new List<string> { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
            // spades
            // create 13 cards 4 times
            for (int h = 0; h < 4; h++)
            {
                for (int i = 1; i < 14; i++)
                {
                    Card card = new Card(symbolList[i - 1], "S", "B", i, frontSpriteList[i - 1 + 13 * h], backSpriteList[i - 1 + 13 * h]);
                    deckOfCards.Add(card);
                }
            }
        }

        public void shuffleDeck()
        {
            // create copyList and copy to it
            List<Card> copyList = new List<Card>();
            while (deckOfCards.Count != 0)
            {
                copyList.Add((deckOfCards[deckOfCards.Count - 1]));
                deckOfCards.RemoveAt(deckOfCards.Count - 1);
            }
            // put back at random
            System.Random random = new System.Random();
            while (copyList.Count != 0)
            {
                int idx = random.Next(0, copyList.Count);
                deckOfCards.Add(copyList[idx]);
                copyList.RemoveAt(idx);
            }
        }

        public bool isEmpty()
        {
            if (deckOfCards.Count == 0)
            {
                return true;
            }
            return false;
        }

        public int dealCard(List<Card> pile)
        {
            if (deckOfCards.Count != 0)
            {
                Card card = deckOfCards[deckOfCards.Count - 1];
                card.FaceUp = false;
                pile.Add(card);
                deckOfCards.RemoveAt(deckOfCards.Count - 1);
                return 1;
            }
            return -1;
        }

        public void returnCards(List<Card> pile)
        {
            while (pile.Count != 0)
            {
                Card card = pile[pile.Count - 1];
                card.FaceUp = false;
                deckOfCards.Add(card);
                pile.RemoveAt(pile.Count - 1);
            }
        }

        public Sprite getTopSprite()
        {
            return deckOfCards[deckOfCards.Count - 1].getSprite();
        }
    }
}
