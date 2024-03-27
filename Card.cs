using System;
using SFML.Graphics;

namespace Spider_Solitaire
{
    class Card
    {
        public string Symbol { get; }
        public string Suit { get; }
        public string Color { get; }
        public int Value { get; }
        private Sprite frontSprite;
        private Sprite backSprite;
        public bool FaceUp { get; set; }

        public Card(string symbol_, string suit_, string color_, int value_, Sprite frontSprite_, Sprite backSprite_)
        {
            Symbol = symbol_;
            Suit = suit_;
            Color = color_;
            Value = value_;
            frontSprite = frontSprite_;
            backSprite = backSprite_;
            FaceUp = false;
        }

        public Sprite getSprite()
        {
            if (FaceUp)
            {
                return frontSprite;
            }
            return backSprite;
        }
    }
}
