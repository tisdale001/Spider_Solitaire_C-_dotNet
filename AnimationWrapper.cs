/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;*/
using SFML.System;
using SFML.Graphics;

namespace Spider_Solitaire
{
    class AnimationWrapper
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

        public AnimationWrapper(Sprite cardSprite_, float beginXPos_, float beginYPos_, float endXPos_, float endYPos_, int numFrames_)
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

        public void advance(int frame)
        {
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
