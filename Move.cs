
using SFML.Graphics;
using System.Collections.Generic;

namespace Spider_Solitaire
{
    class Move : Command
    {
        private Model model;
        private View view;
        private Controller controller;
        private RenderWindow window;
        private List<Card> fromPile;
        private int fromIndex;
        private List<Card> toPile;
        private int toIndex;
        private bool cardRevealed = false;

        public Move(Model model_, View view_, Controller controller_, RenderWindow window_, List<Card> fromPile_, int fromIndex_, List<Card> toPile_, int toIndex_)
        {
            model = model_;
            view = view_;
            controller = controller_;
            window = window_;
            fromPile = fromPile_;
            fromIndex = fromIndex_;
            toPile = toPile_;
            toIndex = toIndex_;
        }

        public override void execute()
        {
            int lastStackDistance;
            if(model.getMovePile().Count == 0)
            {
                // movePile empty: first put cards into movePile
                // first get correct stack distance for animation
                lastStackDistance = view.getStackDistance(fromPile);
                model.cardsToMove(fromPile, fromIndex);
            }
            else
            {
                if(model.getMovePile().Count == 1)
                {
                    lastStackDistance = 0;
                }
                else
                {
                    lastStackDistance = (int)(model.getMovePile()[0].getSprite().Position.Y - model.getMovePile()[1].getSprite().Position.Y);
                }
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
            // move cards into new pile
            model.cardsFromMove(toPile);
            cardRevealed = model.revealCard(fromPile);
            // check if toPile has full stack (ace to king) and create and execute FillFoundationPile
            if (controller.checkFullStack(toPile))
            {
                if(controller.getRedoStack().Count != 0)
                {
                    controller.getRedoStack().RemoveAt(controller.getRedoStack().Count - 1);
                }
                // create FillFoundationPile command
                Command fill = new FillFoundationPile(model, view, controller, window, toPile);
                // push to undoStack
                controller.getUndoStack().Add(fill);
                // execute fill
                fill.execute();
            }
        }

        public override void undo()
        {
            // first check if movePile is empty
            if(model.getMovePile().Count == 0)
            {
                int lastStackDistance = view.getStackDistance(toPile);
                model.cardsToMove(toPile, toIndex);

                // animate movePile: create aniList
                List<AnimationWrapper> aniList = new List<AnimationWrapper>();
                int numFrames = 30;
                int j = 0;
                for(int i = model.getMovePile().Count - 1; i >= 0; i--)
                {
                    aniList.Add(new AnimationWrapper(model.getMovePile()[i].getSprite(), model.getMovePile()[i].getSprite().Position.X, model.getMovePile()[i].getSprite().Position.Y,
                        view.getNextXPos(fromPile), view.getNextYPos(fromPile) + lastStackDistance * j, numFrames));
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
                // if card was revealed, turn to face down
                if (cardRevealed)
                {
                    if(fromPile.Count != 0)
                    {
                        fromPile[fromPile.Count - 1].FaceUp = false;
                    }
                }
                // move cards into fromPile
                model.cardsFromMove(fromPile);
            }
        }
    }
}
