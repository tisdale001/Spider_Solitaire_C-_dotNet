using System;
using SFML.Graphics;
using System.Collections.Generic;


namespace Spider_Solitaire
{
    class FillFoundationPile : Command
    {
        private Model model;
        private View view;
        private Controller controller;
        private RenderWindow window;
        private List<Card> fromPile;
        private List<Card> toPile;
        private bool cardRevealed = false;

        public FillFoundationPile(Model model_, View view_, Controller controller_, RenderWindow window_, List<Card> fromPile_)
        {
            model = model_;
            view = view_;
            controller = controller_;
            window = window_;
            fromPile = fromPile_;
            toPile = model.getCorrectFoundationPile();
        }

        public override void execute()
        {
            int lastStackDistance = view.getStackDistance(fromPile);
            // move 13 cards from fromPile into movePile
            for(int i = 0; i < 13; i++)
            {
                model.getMovePile().Add(fromPile[fromPile.Count - 1]);
                fromPile.RemoveAt(fromPile.Count - 1);
            }
            // animate movePile: create aniList
            List<AnimationWrapper> aniList = new List<AnimationWrapper>();
            int numFrames = 30;
            int j = 0;
            for(int i = model.getMovePile().Count - 1; i >= 0; i--)
            {
                aniList.Add(new AnimationWrapper(model.getMovePile()[i].getSprite(), model.getMovePile()[i].getSprite().Position.X, model.getMovePile()[i].getSprite().Position.Y,
                    view.getNextXPos(toPile), view.getNextYPos(toPile) + lastStackDistance * j, numFrames));
                j++;
            }
            // iterate through aniList: calling advance()
            for(int i = 0; i < numFrames; i++)
            {
                foreach(AnimationWrapper item in aniList)
                {
                    item.advance(i);
                }
                view.draw(window);
            }
            // create second animation which shrinks the pile
            List<AnimationWrapperST> aniList2 = new List<AnimationWrapperST>();
            numFrames = 0;
            int count = 0;
            for(int i = model.getMovePile().Count - 1; i >= 0; i--)
            {
                aniList2.Add(new AnimationWrapperST(model.getMovePile()[i].getSprite(), model.getMovePile()[i].getSprite().Position.X, model.getMovePile()[i].getSprite().Position.Y,
                    view.getNextXPos(toPile), view.getNextYPos(toPile), count));
                count += numFrames;
            }
            // iterate through aniList2: calling advance()
            for(int i = 0; i < count; i++)
            {
                foreach (AnimationWrapperST item in aniList2)
                {
                    item.advance(i);
                }
                view.draw(window);
            }
            // move cards into new pile
            model.cardsFromMove(toPile);
            //reveal card if necessary
            cardRevealed = model.revealCard(fromPile);
            if (controller.checkWin())
            {
                Console.WriteLine("WINNER!!!!");
                // set up random number
                // play animations for win
                Random random = new Random();
                int num = random.Next(0, 2);
                if(num == 0)
                {
                    view.animateWin(window);
                }
                else
                {
                    view.animateWin2(window);
                }
            }
        }

        public override void undo()
        {
            // flip target card on fromPile if necessary
            if (cardRevealed)
            {
                fromPile[fromPile.Count - 1].FaceUp = false;
            }
            // get newStackDistance from fromPile
            int newStackDistance = view.getStackDistance(fromPile);
            // create animation that correctly resizes stack
            while(toPile.Count != 0)
            {
                model.getMovePile().Add(toPile[toPile.Count - 1]);
                toPile.RemoveAt(toPile.Count - 1);
            }
            List<AnimationWrapper> aniList = new List<AnimationWrapper>();
            int numFrames = 0;
            int count = 12 * numFrames;
            int IDNum = 0;
            int j = model.getMovePile().Count - 1;
            for(int i = 0; i < model.getMovePile().Count; i++)
            {
                AnimationWrapper animation = new AnimationWrapper(model.getMovePile()[i].getSprite(), model.getMovePile()[i].getSprite().Position.X, model.getMovePile()[i].getSprite().Position.Y,
                    view.getNextXPos(toPile), view.getNextYPos(toPile) + newStackDistance * j, count);
                animation.setID(IDNum);
                aniList.Add(animation);
                j--;
                count -= numFrames;
                IDNum += numFrames;
            }
            // iterate through aniList: calling advance(): staggering movement
            for(int i = 0; i <= 12 *numFrames; i++)
            {
                foreach(AnimationWrapper item in aniList)
                {
                    if(item.getID() <= i)
                    {
                        item.advance(i - item.getID());
                    }
                }
                view.draw(window);
            }
            // create second animation that moves stack back to fromPile
            List<AnimationWrapper> aniList2 = new List<AnimationWrapper>();
            numFrames = 30;
            j = 0;
            for(int i = model.getMovePile().Count - 1; i >= 0; i--)
            {
                AnimationWrapper animation = new AnimationWrapper(model.getMovePile()[i].getSprite(), model.getMovePile()[i].getSprite().Position.X, model.getMovePile()[i].getSprite().Position.Y,
                    view.getNextXPos(fromPile), view.getNextYPos(fromPile) + newStackDistance * j, numFrames);
                aniList2.Add(animation);
                j++;
            }
            // iterate through aniList2: calling advance
            for(int i = 0; i < numFrames; i++)
            {
                foreach(AnimationWrapper item in aniList2)
                {
                    item.advance(i);
                }
                view.draw(window);
            }
            // put cards onto fromPile
            model.cardsFromMove(fromPile);
            // call undo() for previous move
            controller.Undo();
        }
    }
}
