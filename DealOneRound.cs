using System.Collections.Generic;

using SFML.Graphics;

namespace Spider_Solitaire
{
    class DealOneRound : Command
    {
        private Model model;
        private View view;
        private Controller controller;
        private RenderWindow window;
        private List<Card> correctDrawPile;

        public DealOneRound(Model model_, View view_, Controller controller_, RenderWindow window_)
        {
            model = model_;
            view = view_;
            controller = controller_;
            window = window_;
            correctDrawPile = model.getCorrectDrawPile();
        }

        public override void execute()
        {
            if(correctDrawPile.Count != 0)
            {
                // move all cards into movePile
                while(correctDrawPile.Count != 0)
                {
                    model.getMovePile().Add(correctDrawPile[correctDrawPile.Count - 1]);
                    correctDrawPile.RemoveAt(correctDrawPile.Count - 1);
                }
                // create animation wrappers for each card
                List<AnimationWrapperST> aniList = new List<AnimationWrapperST>();
                int staggerAmt = 10;
                int numFrames = 30;
                int j = 1;
                int k = 0;
                for(int i = 0; i < 10 * staggerAmt; i += staggerAmt)
                {
                    AnimationWrapperST animation = new AnimationWrapperST(model.getMovePile()[k].getSprite(), view.getNextXPos(correctDrawPile),
                        view.getNextYPos(correctDrawPile), view.getNextXPos(model.getMainPile(j)), view.getNextYPos(model.getMainPile(j)), numFrames);
                    animation.setID(i);
                    aniList.Add(animation);
                    j++;
                    k++;
                }
                // iterate through aniList staggering cards according to ID
                for(int i = 0; i <= numFrames + 10 * staggerAmt; i++)
                {
                    foreach(AnimationWrapperST item in aniList)
                    {
                        if(item.getID() <= i)
                        {
                            item.advance(i - item.getID());
                        }
                    }
                    view.draw(window);
                }
                // put cards into correct piles and reveal cards
                for(int i = 0; i < 10; i++)
                {
                    model.getMainPile(i + 1).Add(model.getMovePile()[i]);
                    model.revealCard(model.getMainPile(i + 1));
                }
                model.getMovePile().Clear();
                // check for full stacks (every main pile)
                for(int i = 1; i < 11; i++)
                {
                    if (controller.checkFullStack(model.getMainPile(i)))
                    {
                        if(controller.getRedoStack().Count != 0)
                        {
                            controller.getRedoStack().RemoveAt(controller.getRedoStack().Count - 1);
                        }
                        // create FillFoundationPile command
                        FillFoundationPile fill = new FillFoundationPile(model, view, controller, window, model.getMainPile(i));
                        // push to undoStack
                        controller.getUndoStack().Add(fill);
                        // execute fill
                        fill.execute();
                    }
                }
            }
        }
        public override void undo()
        {
            // put 10 cards face down and put one card from each pile ino movePile
            for(int i = 1; i < 11; i++)
            {
                model.getMainPile(i)[model.getMainPile(i).Count - 1].FaceUp = false;
                model.getMovePile().Add(model.getMainPile(i)[model.getMainPile(i).Count - 1]);
                model.getMainPile(i).RemoveAt(model.getMainPile(i).Count - 1);
            }
            // create animation wrapper (self terminating) for each card
            List<AnimationWrapperST> aniList = new List<AnimationWrapperST>();
            int staggerAmt = 10;
            int numFrames = 30;
            int j = 10;
            int k = 9;
            for(int i = 0; i < 10 * staggerAmt; i += staggerAmt)
            {
                AnimationWrapperST animation = new AnimationWrapperST(model.getMovePile()[k].getSprite(), view.getNextXPos(model.getMainPile(j)),
                    view.getNextYPos(model.getMainPile(j)), view.getNextXPos(correctDrawPile), view.getNextYPos(correctDrawPile), numFrames);
                animation.setID(i);
                aniList.Add(animation);
                j--;
                k--;
            }
            // iterate through aniList staggering cards according to ID
            for(int i = 0; i <= numFrames + 10 * staggerAmt; i++)
            {
                foreach(AnimationWrapperST item in aniList)
                {
                    if(item.getID() <= i)
                    {
                        item.advance(i - item.getID());
                    }
                }
                view.draw(window);
            }
            // put cards back in draw pile
            model.cardsFromMove(correctDrawPile);
        }
    }
}
