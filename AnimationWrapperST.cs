/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;*/
using SFML.System;
using SFML.Graphics;

namespace Spider_Solitaire
{
    class AnimationWrapperST
    {
        private Sprite cardSprite;
        private float beginXPos;
        private float beginYPos;
        private float endXPos;
        private float endYPos;
        private int numFrames;
        private float xToMove;
        private float yToMove;
        private int wrapperID = 0;

        public AnimationWrapperST(Sprite cardSprite_, float beginXPos_, float beginYPos_, float endXPos_, float endYPos_, int numFrames_)
        {
            cardSprite = cardSprite_;
            beginXPos = beginXPos_;
            beginYPos = beginYPos_;
            endXPos = endXPos_;
            endYPos = endYPos_;
            numFrames = numFrames_;
            xToMove = (float)((endXPos - beginXPos) / numFrames);
            yToMove = (float)((endYPos - beginYPos) / numFrames);
            cardSprite.Position = new Vector2f(beginXPos, beginYPos);
        }

        // advances position of sprite one small step and changes begin position
        public void advance(int frame)
        {
            // self terminate by making numFrames the limit of movement
            if(frame > numFrames)
            {
                frame = numFrames;
            }
            float bXPos = beginXPos + (xToMove * frame);
            float bYPos = beginYPos + (yToMove * frame);
            cardSprite.Position = new Vector2f(bXPos, bYPos);
        }

        public void setID(int num)
        {
            wrapperID = num;
        }

        public int getID()
        {
            return wrapperID;
        }

        public Sprite getSprite()
        {
            return cardSprite;
        }
    }
}
