using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Spider_Solitaire
{
    class Controller
    {
        private Model model = new Model();
        private View view = new View();
        private bool pressedLastFrame = false;
        private bool once = true;
        private int xPos = 0;
        private int yPos = 0;
        private int xDiff = 0;
        private int yDiff = 0;
        private List<Command> undoStack = new List<Command>();
        private List<Command> redoStack = new List<Command>();

        public void playGame()
        {
            view.initiateView(model.getMainPile(1), model.getMainPile(2), model.getMainPile(3), model.getMainPile(4), model.getMainPile(5), model.getMainPile(6), model.getMainPile(7),
                model.getMainPile(8), model.getMainPile(9), model.getMainPile(10), model.getFoundationPile(1), model.getFoundationPile(2), model.getFoundationPile(3), model.getFoundationPile(4),
                model.getFoundationPile(5), model.getFoundationPile(6), model.getFoundationPile(7), model.getFoundationPile(8), model.getDrawPile(1), model.getDrawPile(2), model.getDrawPile(3),
                model.getDrawPile(4), model.getDrawPile(5), model.getMovePile(), model.getDealPile(), model.getDeck());

            // set up random number here

            RenderWindow window = new RenderWindow(new VideoMode(view.getBoundsWidth(), view.getBoundsHeight()), " Spider Solitaire", Styles.Close);

            // deal hand
            model.allDeckCardsToDealPile(model.getDeck());
            dealTableau(window);
            view.draw(window);

            window.Closed += new EventHandler(onClose);
            window.MouseButtonPressed += new EventHandler<SFML.Window.MouseButtonEventArgs>(handleMousePress);
            window.MouseButtonReleased += new EventHandler<SFML.Window.MouseButtonEventArgs>(handleMouseRelease);
            window.MouseMoved += new EventHandler<SFML.Window.MouseMoveEventArgs>(handleMouseMove);
            window.KeyReleased += new EventHandler<SFML.Window.KeyEventArgs>(handleKeyRelease);
            
            //main loop
            while (window.IsOpen)
            {
                window.DispatchEvents();
                view.draw(window);

            }
        }

        public void handleKeyRelease(object sender, KeyEventArgs e)
        {
            RenderWindow window = (RenderWindow)sender;
            switch (e.Code)
            {
                case Keyboard.Key.Escape:
                    window.Close();
                    break;
                case Keyboard.Key.Z:
                    Undo();
                    break;
                case Keyboard.Key.Y:
                    Redo();
                    break;
                case Keyboard.Key.D:
                    redeal(window);
                    break;
                case Keyboard.Key.M:
                    view.animateWin2(window);
                    break;
                case Keyboard.Key.N:
                    view.animateWin(window);
                    break;
                default:
                    break;
            }
        }

        public void onClose(object sender, EventArgs e)
        {
            RenderWindow window = (RenderWindow)sender;
            window.Close();
        }

        public void handleMousePress(object sender, MouseButtonEventArgs e)
        {
            // check for left button press
            RenderWindow window = (RenderWindow)sender;
            if(e.Button == Mouse.Button.Left)
            {
                Vector2f mousePos = new Vector2f(Mouse.GetPosition(window).X, Mouse.GetPosition(window).Y);
                int index = 0;
                List<Card> clickPile = view.getClick(mousePos, out index);
                if(clickPile != null)
                {
                    // continue with move, setup lastPile
                    if(clickPile == model.getCorrectDrawPile())
                    {
                        // deal out a round of cards if LEGAL DEAL
                        if (checkLegalDeal())
                        {
                            // create DealOneRound, add to undoStack
                            Command deal = new DealOneRound(model, view, this, window);
                            undoStack.Add(deal);
                            deal.execute();
                            redoStack.Clear();
                        }
                        else
                        {
                            // print directions if deal not legal
                            Console.WriteLine("There must be at least one card in each pile in order to deal.");
                        }
                    }
                    else
                    {
                        // mainPile clicked on
                        if(clickPile.Count != 0)
                        {
                            if (clickPile[index].FaceUp)
                            {
                                // check if legal stack
                                if(checkMove(clickPile, index))
                                {
                                    // put cards in movePile, set lastPile and lastStackDistance, clearRedoStack()
                                    view.setLastStackDistance(view.getStackDistance(clickPile));
                                    model.cardsToMove(clickPile, index);
                                    view.setLastPile(clickPile);
                                    redoStack.Clear();
                                }
                            }
                        }
                    }
                }
                pressedLastFrame = true;
            }
        }

        public void handleMouseRelease(object sender, MouseButtonEventArgs e)
        {
            // check for left button release
            RenderWindow window = (RenderWindow)sender;
            if(e.Button == Mouse.Button.Left)
            {
                Vector2f mousePos = new Vector2f(Mouse.GetPosition(window).X, Mouse.GetPosition(window).Y);
                int index = 0;
                List<Card> clickPile = view.getRelease(mousePos, out index);
                if(clickPile != null)
                {
                    if(model.getMovePile().Count != 0)
                    {
                        // continue with move, check lastPile
                        if (model.checkDrawPile(clickPile))
                        {
                            // draw pile clicked on: return to lastPile
                            moveToLastPile(window);
                        }
                        else if(clickPile == view.getLastPile())
                        {
                            // lastPile click-released on: make best move
                            makeBestMove(window);
                        }
                        else
                        {
                            // new pile click-released on: make move or return to lastPile
                            if (checkDrop(clickPile))
                            {
                                // legal move: create Move command
                                Command move = new Move(model, view, this, window, view.getLastPile(), view.getLastPile().Count, clickPile, clickPile.Count);
                                undoStack.Add(move);
                                move.execute();
                            }
                            else
                            {
                                // move is illegal: return to lastPile
                                // create and execute Move, but do not add to stack
                                moveToLastPile(window);
                            }
                        }
                    }
                }
                else
                {
                    // null click-released: if movePile not empty, move to lastPile
                    if(model.getMovePile().Count != 0)
                    {
                        moveToLastPile(window);
                    }
                }
                once = true;
                pressedLastFrame = false;
            }
            
        }

        public void handleMouseMove(object sender, MouseMoveEventArgs e)
        {
            // check if left button is still pressed
            RenderWindow window = (RenderWindow)sender;
            if (pressedLastFrame)
            {
                // drag movePile
                if(model.getMovePile().Count != 0)
                {
                    Vector2f mousePos = new Vector2f(Mouse.GetPosition().X, Mouse.GetPosition().Y);
                    List<Card> movePile = model.getMovePile();
                    Vector2f cardPos = new Vector2f(movePile[movePile.Count - 1].getSprite().Position.X, movePile[movePile.Count - 1].getSprite().Position.Y);
                    if(mousePos.X > xPos && mousePos.Y > yPos && mousePos.X - view.getBoundsWidth() < xPos && mousePos.Y - view.getBoundsHeight() < yPos)
                    {
                        if (once)
                        {
                            xDiff = (int)(mousePos.X - cardPos.X);
                            yDiff = (int)(mousePos.Y - cardPos.Y);
                            once = false;
                        }
                        xPos = (int)(mousePos.X - xDiff);
                        yPos = (int)(mousePos.Y - yDiff);
                        for(int i = movePile.Count - 1; i >= 0; i--)
                        {
                            movePile[i].getSprite().Position = new Vector2f(xPos, yPos + view.getStackDistance(movePile) * (movePile.Count - 1 - i));
                        }
                    }
                    pressedLastFrame = true;
                }
                // draw all cards
                // TODO: check if this is correct place to draw....
                view.draw(window);
            }
        }

        public void dealTableau(RenderWindow window)
        {
            // deal 10 cards 5 times
            for(int h = 0; h < 5; h++)
            {
                // put 10 cards into movePile
                for(int i = 0; i < 10; i++)
                {
                    model.dealOneCardFromPile(model.getDealPile(), model.getMovePile());
                }
                // create animation wrappers for each card
                List<AnimationWrapperST> aniList1 = new List<AnimationWrapperST>();
                int staggerAmt1 = 10;
                int numFrames1 = 40;
                int j = 1;
                int k = 0;
                for(int i = 0; i < 10 * staggerAmt1; i += staggerAmt1)
                {
                    AnimationWrapperST animation = new AnimationWrapperST(model.getMovePile()[k].getSprite(), view.getNextXPos(model.getDrawPile(1)), view.getNextYPos(model.getDrawPile(1)),
                        view.getNextXPos(model.getMainPile(j)), view.getNextYPos(model.getMainPile(j)), numFrames1);
                    animation.setID(i);
                    aniList1.Add(animation);
                    j++;
                    k++;
                }
                // iterate through aniList staggering cards according to ID
                for(int i = 0; i < numFrames1 + 10 * staggerAmt1; i++)
                {
                    foreach(AnimationWrapperST item in aniList1)
                    {
                        if(item.getID() <= i)
                        {
                            item.advance(i - item.getID());
                        }
                    }
                    view.draw(window);
                }
                // put cards into correct piles
                for(int i = 0; i < 10; i++)
                {
                    model.getMainPile(i + 1).Add(model.getMovePile()[i]);
                }
                model.getMovePile().Clear();
            }
            // deal last 4 cards
            for(int i = 0; i < 4; i++)
            {
                model.dealOneCardFromPile(model.getDealPile(), model.getMovePile());
            }
            // create animation wrappers for each card
            List<AnimationWrapperST> aniList = new List<AnimationWrapperST>();
            int staggerAmt = 10;
            int numFrames = 40;
            int m = 1;
            int n = 0;
            for(int i = 0; i < 4 * staggerAmt; i += staggerAmt)
            {
                AnimationWrapperST animation = new AnimationWrapperST(model.getMovePile()[n].getSprite(), view.getNextXPos(model.getDrawPile(i)), view.getNextYPos(model.getDrawPile(1)),
                    view.getNextXPos(model.getMainPile(m)), view.getNextYPos(model.getMainPile(m)), numFrames);
                animation.setID(i);
                aniList.Add(animation);
                m++;
                n++;
            }
            // iterate through aniList staggering cards according to ID
            for(int i = 0; i < numFrames + 4 * staggerAmt; i++)
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
            // put cards into correct piles
            for(int i = 0; i < 4; i++)
            {
                model.getMainPile(i + 1).Add(model.getMovePile()[i]);
            }
            model.getMovePile().Clear();
            // reveal cards in each pile
            for(int i = 1; i < 11; i++)
            {
                model.revealCard(model.getMainPile(i));
            }
            // put cards into draw piles
            for(int i = 1; i < 6; i++)
            {
                for(int j = 0; j < 10; j++)
                {
                    model.dealOneCardFromPile(model.getDealPile(), model.getDrawPile(i));
                }
            }
        }

        public void redeal(RenderWindow window)
        {
            undoStack.Clear();
            redoStack.Clear();

            model.allCardsReturnToDeck(model.getDeck());
            model.shuffleCards();
            model.allDeckCardsToDealPile(model.getDeck());
            dealTableau(window);
        }

        public bool checkLegalDeal()
        {
            for(int i = 1; i < 11; i++)
            {
                if(model.getMainPile(i).Count == 0)
                {
                    return false;
                }
            }
            return true;
        }

        public bool checkMove(List<Card> pile, int index)
        {
            int val = pile[pile.Count - 1].Value;
            for(int i = pile.Count - 1; i >= index; i--)
            {
                if(pile[i].Value != val)
                {
                    return false;
                }
                val++;
            }
            return true;
        }

        public bool checkDrop(List<Card> pile)
        {
            if(pile.Count == 0)
            {
                return true;
            }
            // movePile already checked for empty
            if(model.getMovePile()[model.getMovePile().Count - 1].Value == pile[pile.Count - 1].Value - 1)
            {
                return true;
            }
            return false;
        }

        public bool checkFullStack(List<Card> pile)
        {
            if(pile.Count != 0)
            {
                if(pile.Count >= 13)
                {
                    int j = 1;
                    for(int i = pile.Count - 1; i >= pile.Count - 13; i--)
                    {
                        if(pile[i].Value != j)
                        {
                            return false;
                        }
                        j++;
                    }
                    return true;
                }
            }
            return false;
        }

        public bool checkWin()
        {
            for(int i = 1; i < 9; i++)
            {
                if(model.getFoundationPile(i).Count == 0)
                {
                    return false;
                }
            }
            return true;
        }

        public void makeBestMove(RenderWindow window)
        {
            // no need to check if movePile is empty or not (already checked)
            // first: get vector of posible piles to move to
            List<List<Card>> possPiles = new List<List<Card>>();
            for(int i = 10; i >= 1; i--)
            {
                if(model.getMainPile(i).Count != 0)
                {
                    // check back of mainPile: compare to back of movePile
                    if(model.getMovePile()[model.getMovePile().Count - 1].Value + 1 == model.getMainPile(i)[model.getMainPile(i).Count - 1].Value)
                    {
                        possPiles.Add(model.getMainPile(i));
                    }
                }
                else
                {
                    // mainPile is empty: it is possible move
                    possPiles.Add(model.getMainPile(i));
                }
            }
            if(possPiles.Count == 0)
            {
                // no possible move: return to lastPile
                moveToLastPile(window);
            }
            else if(possPiles.Count == 1)
            {
                if(possPiles[possPiles.Count - 1] == view.getLastPile())
                {
                    // only possible move is lastPile: return to lastPile
                    moveToLastPile(window);
                }
                else
                {
                    // only possible move is NOT lastPIle: make move
                    // create move command
                    Command move = new Move(model, view, this, window, view.getLastPile(), view.getLastPile().Count, possPiles[possPiles.Count - 1], possPiles[possPiles.Count - 1].Count);
                    undoStack.Add(move);
                    move.execute();
                }
            }
            else
            {
                // possPiles size > 1: choose largest stack to move to , this is best move
                // if piles are equal: the first is fine
                int maxCount = -1;
                List<Card> bestPile = null;
                foreach(List<Card> pile in possPiles)
                {
                    if(getSizeOfStack(pile) == maxCount && pile != view.getLastPile())
                    {
                        bestPile = pile;
                        maxCount = getSizeOfStack(pile);
                    }
                    else if (getSizeOfStack(pile) > maxCount)
                    {
                        bestPile = pile;
                        maxCount = getSizeOfStack(pile);
                    }
                }
                if(bestPile != view.getLastPile())
                {
                    // make move into bestPile, execute and add to undoStack
                    Command move = new Move(model, view, this, window, view.getLastPile(), view.getLastPile().Count, bestPile, bestPile.Count);
                    undoStack.Add(move);
                    move.execute();
                }
                else
                {
                    // do not add to undoStack
                    model.cardsFromMove(bestPile);
                }
            }
        }

        public int getNumUpturnedCards(List<Card> pile)
        {
            int count = 0;
            foreach(Card card in pile)
            {
                if (card.FaceUp)
                {
                    count++;
                }
            }
            return count;
        }

        public int getSizeOfStack(List<Card> pile)
        {
            int pileSize = getNumUpturnedCards(pile);
            if(pileSize <= 1)
            {
                return pileSize;
            }
            else
            {
                // pile size is greater than 1
                int count = 1;
                for(int i = pile.Count - 1; i > 0; i--)
                {
                    // if card at i - 1 is face down: return count
                    if(!pile[i - 1].FaceUp)
                    {
                        return count;
                    }
                    // cards must be increasing in value by 1
                    if(pile[i].Value + 1 != pile[i - 1].Value)
                    {
                        return count;
                    }
                    count++;
                }
                return count;
            }
/*            return 0;
*/        }

        public void moveToLastPile(RenderWindow window)
        {
            // create and execute Move, but do not add to stack
            Command move = new Move(model, view, this, window, view.getLastPile(), view.getLastPile().Count, view.getLastPile(), view.getLastPile().Count);
            move.execute();
        }

        public void Undo()
        {
            if(undoStack.Count != 0)
            {
                if(model.getMovePile().Count == 0)
                {
                    redoStack.Add(undoStack[undoStack.Count - 1]);
                    undoStack.RemoveAt(undoStack.Count - 1);
                    redoStack[redoStack.Count - 1].undo();
                }
            }
        }

        public void Redo()
        {
            if(redoStack.Count != 0)
            {
                if(model.getMovePile().Count == 0)
                {
                    undoStack.Add(redoStack[redoStack.Count - 1]);
                    redoStack.RemoveAt(redoStack.Count - 1);
                    undoStack[undoStack.Count - 1].execute();
                }
            }
        }

        public void clearRedoStack()
        {
            redoStack.Clear();
        }

        public List<Command> getUndoStack()
        {
            return undoStack;
        }

        public List<Command> getRedoStack()
        {
            return redoStack;
        }
    }
}
