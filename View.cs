using System;/*
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;*/
using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;

namespace Spider_Solitaire
{
    class View
    {
        private uint boundsWidth = 1400;
        private uint boundsHeight = 1000;
        private int stackDistance = 30;
        private int distBtwPiles = 40;
        private int cardHeight = 0; // set cardHeight
        private int cardWidth = 0; // set cardWidth
        private int startPointX = 100;
        private int startPointY = 100;
        private long frameStartTime;
        private long maxTicksPerFrame = (long)(1000 / 300);
        private Image image; // initiate
        private Texture texture; // initiate
        private Sprite sprite; // initiate
        private List<Card> mainPile1; // initiate all below
        private List<Card> mainPile2;
        private List<Card> mainPile3;
        private List<Card> mainPile4;
        private List<Card> mainPile5;
        private List<Card> mainPile6;
        private List<Card> mainPile7;
        private List<Card> mainPile8;
        private List<Card> mainPile9;
        private List<Card> mainPile10;
        private List<Card> foundationPile1;
        private List<Card> foundationPile2;
        private List<Card> foundationPile3;
        private List<Card> foundationPile4;
        private List<Card> foundationPile5;
        private List<Card> foundationPile6;
        private List<Card> foundationPile7;
        private List<Card> foundationPile8;
        private List<Card> drawPile1;
        private List<Card> drawPile2;
        private List<Card> drawPile3;
        private List<Card> drawPile4;
        private List<Card> drawPile5;
        private List<Card> movePile;
        private List<Card> dealPile;
        private Deck deck;

        private List<Card> lastPile;
        private int lastStackDistance;

        public void initiateView(List<Card> mainPile1_, List<Card> mainPile2_, List<Card> mainPile3_, List<Card> mainPile4_, List<Card> mainPile5_, List<Card> mainPile6_, List<Card> mainPile7_,
            List<Card> mainPile8_, List<Card> mainPile9_, List<Card> mainPile10_, List<Card> foundationPile1_, List<Card> foundationPile2_, List<Card> foundationPile3_, List<Card> foundationPile4_,
            List<Card> foundationPile5_, List<Card> foundationPile6_, List<Card> foundationPile7_, List<Card> foundationPile8_, List<Card> drawPile1_, List<Card> drawPile2_, List<Card> drawPile3_,
            List<Card> drawPile4_, List<Card> drawPile5_, List<Card> movePile_, List<Card> dealPile_, Deck deck_)
        {
            mainPile1 = mainPile1_;
            mainPile2 = mainPile2_;
            mainPile3 = mainPile3_;
            mainPile4 = mainPile4_;
            mainPile5 = mainPile5_;
            mainPile6 = mainPile6_;
            mainPile7 = mainPile7_;
            mainPile8 = mainPile8_;
            mainPile9 = mainPile9_;
            mainPile10 = mainPile10_;
            foundationPile1 = foundationPile1_;
            foundationPile2 = foundationPile2_;
            foundationPile3 = foundationPile3_;
            foundationPile4 = foundationPile4_;
            foundationPile5 = foundationPile5_;
            foundationPile6 = foundationPile6_;
            foundationPile7 = foundationPile7_;
            foundationPile8 = foundationPile8_;
            drawPile1 = drawPile1_;
            drawPile2 = drawPile2_;
            drawPile3 = drawPile3_;
            drawPile4 = drawPile4_;
            drawPile5 = drawPile5_;
            movePile = movePile_;
            dealPile = dealPile_;
            deck = deck_;
            cardHeight = (int)deck.getTopSprite().GetGlobalBounds().Height;
            cardWidth = (int)deck.getTopSprite().GetGlobalBounds().Width;
            image = new Image(boundsWidth, boundsHeight, Color.Black);
            texture = new Texture(image);
            sprite = new Sprite(texture);
            lastStackDistance = stackDistance;
        }

        // getClick() returns pile and "out" index clicked on in view
        public List<Card> getClick(Vector2f mousePos, out int index)
        {
            for (int i = 1; i < 11; i++)
            {
                if(getMainPile(i).Count != 0)
                {
                    for(int j = getMainPile(i).Count - 1; j >= 0; j--)
                    {
                        if (getMainPile(i)[j].getSprite().GetGlobalBounds().Contains(mousePos.X, mousePos.Y))
                        {
                            index = j;
                            return getMainPile(i);
                        }
                    }
                }
            }
            // check draw piles
            for(int i = 5; i > 0; i--)
            {
                if(getDrawPile(i).Count != 0)
                {
                    if(getDrawPile(i)[getDrawPile(i).Count - 1].getSprite().GetGlobalBounds().Contains(mousePos.X, mousePos.Y))
                    {
                        index = 0;
                        return getDrawPile(i);
                    }
                }
            }
            // check if clicked in empty slots (main piles)
            float x = mousePos.X;
            float y = mousePos.Y;
            for(int i = 0; i < 10; i++)
            {
                if(x > startPointX - distBtwPiles / 2 + cardWidth * i + distBtwPiles * i && x < startPointX + distBtwPiles / 2 + cardWidth + cardWidth * i +
                    distBtwPiles * i && y > startPointY - distBtwPiles / 2 + cardHeight + distBtwPiles && y < startPointY + distBtwPiles / 2 + cardHeight * 2 + distBtwPiles)
                {
                    index = 0;
                    return getMainPile(i + 1);
                }
            }
            index = 0;
            return null;
        }

        // getRelease() returns pile click-released on
        public List<Card> getRelease(Vector2f mousePos, out int index)
        {
            float x = mousePos.X;
            float y = mousePos.Y;
            // check main piles
            for(int i = 0; i < 10; i++)
            {
                if(getMainPile(i + 1).Count != 0)
                {
                    // main pile not empty: include perimeter around stack
                    if(x > startPointX - distBtwPiles / 2 + cardWidth * i + distBtwPiles * i && x < startPointX + distBtwPiles / 2 + cardWidth + cardWidth * i + distBtwPiles * i &&
                        y > startPointY + cardHeight + distBtwPiles - distBtwPiles / 2 && y < startPointY + cardHeight * 2 + distBtwPiles + distBtwPiles / 2 +
                        (getMainPile(i + 1).Count - 1) * getStackDistance(getMainPile(i + 1)))
                    {
                        index = 0;
                        return getMainPile(i + 1);
                    }
                }
                else
                {
                    // main pile empty: empty space plus perimeter
                    if( x > startPointX - distBtwPiles / 2 + cardWidth * i + distBtwPiles * i && x < startPointX + distBtwPiles / 2 + cardWidth + cardWidth * i + distBtwPiles * i &&
                        y > startPointY + cardHeight + distBtwPiles - distBtwPiles / 2 && y < startPointY + cardHeight * 2 + distBtwPiles + distBtwPiles / 2)
                    {
                        index = 0;
                        return getMainPile(i + 1);
                    }
                }
            }
            // check draw piles with global bounds
            for(int i = 5; i > 0; i--)
            {
                if(getDrawPile(i).Count != 0)
                {
                    if(getDrawPile(i)[getDrawPile(i).Count - 1].getSprite().GetGlobalBounds().Contains(mousePos.X, mousePos.Y))
                    {
                        index = 0;
                        return getDrawPile(i);
                    }
                }
            }
            index = 0;
            return null;
        }

        // set position of each sprite for normal display
        public void setPosition()
        {
            //set position of all sprites except movePile
            for(int i = 0; i < 10; i++)
            {
                if(getMainPile(i + 1).Count != 0)
                {
                    for(int j = 0; j < getMainPile(i + 1).Count; j++)
                    {
                        getMainPile(i + 1)[j].getSprite().Position = new Vector2f(startPointX + cardWidth * i + distBtwPiles * i, startPointY + cardHeight + distBtwPiles + getStackDistance(getMainPile(i + 1)) * j);
                    }
                }
            }
            // back sprites of draw piles
            for(int i = 0; i < 5; i++)
            {
                if(getDrawPile(i + 1).Count != 0)
                {
                    getDrawPile(i + 1)[getDrawPile(i + 1).Count - 1].getSprite().Position = new Vector2f(startPointX + stackDistance * i, startPointY);
                }
            }
            // back sprites of foundation piles
            for(int i = 0; i < 8; i++)
            {
                if(getFoundationPile(i + 1).Count != 0)
                {
                    getFoundationPile(i + 1)[getFoundationPile(i + 1).Count - 1].getSprite().Position = new Vector2f(startPointX + cardWidth * 2 + distBtwPiles * 2 + cardWidth * i + distBtwPiles * i, startPointY);
                }
            }
            // back sprite of daelPile
            if(dealPile.Count != 0)
            {
                dealPile[dealPile.Count - 1].getSprite().Position = new Vector2f(startPointX, startPointY); 
            }
        }

        //draw all sprites in non-empty piles
        public void draw(RenderWindow window)
        {
            frameStartTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            window.Clear();
            // set all sprite positions
            setPosition();

            // main piles
            for(int i = 1; i < 11; i++)
            {
                if(getMainPile(i).Count != 0)
                {
                    for(int j = 0; j < getMainPile(i).Count; j++)
                    {
                        window.Draw(getMainPile(i)[j].getSprite());
                    }
                }
            }
            // draw piles
            for(int i = 1; i < 6; i++)
            {
                if(getDrawPile(i).Count != 0)
                {
                    window.Draw(getDrawPile(i)[getDrawPile(i).Count - 1].getSprite());
                }
            }
            // foundation piles
            for(int i = 1; i < 9; i++)
            {
                if(getFoundationPile(i).Count != 0)
                {
                    window.Draw(getFoundationPile(i)[getFoundationPile(i).Count - 1].getSprite());
                }
            }
            // deal pile
            if(dealPile.Count != 0)
            {
                window.Draw(dealPile[dealPile.Count - 1].getSprite());
            }
            // movePile
            for(int i = movePile.Count - 1; i >= 0; i--)
            {
                window.Draw(movePile[i].getSprite());
            }

            window.Display();
            limitFPS();
        }

        // frame-capping function: sleeps between animation frames
        public void limitFPS()
        {
            long ms = (long)(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);
            long frameTicks = ms - frameStartTime;
            if(frameTicks < maxTicksPerFrame)
            {
                System.Threading.Thread.Sleep((int)(maxTicksPerFrame - frameTicks));
            }
        }

        public void animateWin(RenderWindow window)
        {
            Random random = new Random();
            Deck deck1 = new Deck();
            Deck deck2 = new Deck();
            Deck deck3 = new Deck();
            Deck deck4 = new Deck();
            List<Card> cardList = new List<Card>();
            while (!deck1.isEmpty())
            {
                deck1.dealCard(cardList);
                cardList[cardList.Count - 1].FaceUp = true;
            }
            while (!deck2.isEmpty())
            {
                deck2.dealCard(cardList);
                cardList[cardList.Count - 1].FaceUp = true;
            }
            while (!deck3.isEmpty())
            {
                deck3.dealCard(cardList);
                cardList[cardList.Count - 1].FaceUp = true;
            }
            while (!deck4.isEmpty())
            {
                deck4.dealCard(cardList);
                cardList[cardList.Count - 1].FaceUp = true;
            }
            // continue while loop till key is pressed or mose clicked
            bool animation = true;
            window.Closed += new EventHandler((sender, e) => onClose(sender, e, out animation));
            window.MouseButtonReleased += new EventHandler<SFML.Window.MouseButtonEventArgs>((sender, e) => handleMouseRelease(sender, e, out animation));
            window.KeyReleased += new EventHandler<SFML.Window.KeyEventArgs>((sender, e) => handleKeyRelease(sender, e, out animation));

            while (animation)
            {
                window.DispatchEvents();
                if(animation == false)
                {
                    break;
                }
                // code card burst here
                int numFrames = 200;
                List<AnimationWrapper> aniList = new List<AnimationWrapper>();
                int i = 0;
                foreach(Card item in cardList)
                {
                    AnimationWrapper wrapper = new AnimationWrapper(item.getSprite(), boundsWidth / 2 - cardWidth / 2, boundsHeight / 2 - cardHeight / 2,
                        random.Next(-100, (int)boundsWidth + 100), random.Next(-100, (int)boundsHeight + 100), numFrames);
                    wrapper.setID(i);
                    aniList.Add(wrapper);
                    i++;
                }
                // iterate through aniList, adding one each time
                for(i = 0; i < aniList.Count - 1; i++)
                {
                    frameStartTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                    window.Clear();
                    for(int j = 0; j < i + 1; j++)
                    {
                        aniList[j].advance(i - aniList[j].getID());
                        window.Draw(aniList[j].getSprite());
                    }
                    window.Display();
                    // delay for frame-capping
                    limitFPS();
                }
                // keep animating more frames
                for(i = aniList.Count; i < aniList.Count + 600; i++)
                {
                    frameStartTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                    window.Clear();
                    for(int j = 0; j < aniList.Count; j++)
                    {
                        aniList[j].advance(i - aniList[j].getID());
                        window.Draw(aniList[j].getSprite());
                    }
                    window.Display();
                    limitFPS();
                }
            }
        }

        public void animateWin2(RenderWindow window)
        {
            Random random = new Random();
            Deck deck1 = new Deck();
            Deck deck2 = new Deck();
            Deck deck3 = new Deck();
            Deck deck4 = new Deck();
            List<Card> cardList = new List<Card>();
            while (!deck1.isEmpty())
            {
                deck1.dealCard(cardList);
                cardList[cardList.Count - 1].FaceUp = true;
            }
            while (!deck2.isEmpty())
            {
                deck2.dealCard(cardList);
                cardList[cardList.Count - 1].FaceUp = true;
            }
            while (!deck3.isEmpty())
            {
                deck3.dealCard(cardList);
                cardList[cardList.Count - 1].FaceUp = true;
            }
            while (!deck4.isEmpty())
            {
                deck4.dealCard(cardList);
                cardList[cardList.Count - 1].FaceUp = true;
            }
            // continue while loop till key is pressed or mose clicked
            bool animation = true;
            window.Closed += new EventHandler((sender, e) => onClose(sender, e, out animation));
            window.MouseButtonReleased += new EventHandler<SFML.Window.MouseButtonEventArgs>((sender, e) => handleMouseRelease(sender, e, out animation));
            window.KeyReleased += new EventHandler<SFML.Window.KeyEventArgs>((sender, e) => handleKeyRelease(sender, e, out animation));

            while (animation)
            {
                window.DispatchEvents();
                if (animation == false)
                {
                    break;
                }
                //code card burst here
                int numFrames = 200;
                List<AnimationWrapper> aniList = new List<AnimationWrapper>();
                int i = 0;
                foreach(Card item in cardList)
                {
                    AnimationWrapper wrapper = new AnimationWrapper(item.getSprite(), random.Next(-100, (int)boundsWidth + 100), random.Next(-100, (int)boundsHeight + 100),
                        random.Next(-100, (int)boundsWidth + 100), random.Next(-100, (int)boundsHeight + 100), numFrames);
                    wrapper.setID(i);
                    aniList.Add(wrapper);
                    i++;
                }
                // iterate through aniList, adding one each tinme
                for(i = 0; i < aniList.Count - 1; i++)
                {
                    frameStartTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                    window.Clear();
                    for(int j = 0; j < i + 1; j++)
                    {
                        aniList[j].advance(i - aniList[j].getID());
                        window.Draw(aniList[j].getSprite());
                    }
                    window.Display();
                    limitFPS();
                }
                // keep animating more frames
                for(i = aniList.Count; i < aniList.Count + 600; i++)
                {
                    frameStartTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                    window.Clear();
                    for(int j = 0; j < aniList.Count; j++)
                    {
                        aniList[j].advance(i - aniList[j].getID());
                        window.Draw(aniList[j].getSprite());
                    }
                    window.Display();
                    limitFPS();
                }

            }
        }

        public void onClose(object sender, EventArgs e, out bool animation)
        {
            animation = false;
        }

        public void handleKeyRelease(object sender, EventArgs e, out bool animation)
        {
            animation = false;
        }

        public void handleMouseRelease(object sender, EventArgs e, out bool animation)
        {
            animation = false;
        }

        //getter
        public uint getBoundsWidth()
        {
            return boundsWidth;
        }
        //getter
        public uint getBoundsHeight()
        {
            return boundsHeight;
        }
        //getter
        public int getLastStackDistance()
        {
            return lastStackDistance;
        }

        //getter: returns last pile
        public List<Card> getLastPile()
        {
            return lastPile;
        }

        // getter for proper stack distance: shrinks as pile gets larger
        public int getStackDistance(List<Card> pile)
        {
            if(pile.Count != 0)
            {
                if(pile == movePile)
                {
                    return lastStackDistance;
                }
                else if(pile == lastPile)
                {
                    return lastStackDistance;
                }
                else if(pile.Count > 18)
                {
                    return (int)(stackDistance * (1 - 0.025 * (pile.Count - 18)));
                }
            }
            return stackDistance;
        }

        //getter: main pile by number
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

        //getter: returns foundation pile by number
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

        //getter: returns draw pile by number
        public List<Card> getDrawPile(int num)
        {
            switch (num)
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

        // returns x coordinate of the next position in pile
        public int getNextXPos(List<Card> pile)
        {
            for(int i = 0; i < 10; i++)
            {
                if(pile == getMainPile(i + 1))
                {
                    return (startPointX + cardWidth * i + distBtwPiles * i);
                }
            }
            // check draw piles
            for(int i = 0; i < 5; i++)
            {
                if(pile == getDrawPile(i + 1))
                {
                    return (startPointX + stackDistance * i);
                }
            }
            // check foundation piles starting with last pile
            for(int i = 7; i >= 0; i--)
            {
                if(pile == getFoundationPile(i + 1))
                {
                    return (startPointX + cardWidth * 2 + distBtwPiles * 2 + cardWidth * i + distBtwPiles * i);
                }
            }

            return 0;
        }

        // returns y coordinate of the next position in pile
        public int getNextYPos(List<Card> pile)
        {
            for(int i = 1; i < 11; i++)
            {
                if(pile == getMainPile(i))
                {
                    return (startPointY + cardHeight + distBtwPiles + getStackDistance(getMainPile(i)) * getMainPile(i).Count);
                }
            }
            // check draw piles
            for(int i = 1; i < 6; i++)
            {
                if(pile == getDrawPile(i))
                {
                    return startPointY;
                }
            }
            // check foundation piles starting with last pile
            for(int i = 7; i >= 0; i--)
            {
                if(pile == getFoundationPile(i + 1))
                {
                    return startPointY;
                }
            }
            return 0;
        }

        public void setLastStackDistance(int stackDist)
        {
            lastStackDistance = stackDist;
        }

        public void setLastPile(List<Card> pile)
        {
            lastPile = pile;
        }
        

    }
}
