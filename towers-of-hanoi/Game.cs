using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Xml;

namespace towers_of_hanoi
{
    class Game
    {
        Stack<int>[] poles;

        public Stack<(bool, int, int)> moveHistory; // first arg: move done by bot. second arg: start pole. third arg: end pole

        int poleCount;
        int discCount;
        int startPole;
        int endPole;

        int botState;
        int botA;
        int botB;
        int botC;

        bool botMoving;
        bool undoingMove;

        private bool _gameWon;
        public bool GameWon
        {
            get { return _gameWon; }
            set { return; }
        }

        private int _movesTaken;
        public int MovesTaken
        {
            get { return _movesTaken; }
            set { return; }
        }

        public Game(int poleCount, int discCount, int startingPole, int endingPole)
        {
            _gameWon = false;
            _movesTaken = 0;
            // create the poles
            this.poleCount = poleCount;
            poles = new Stack<int>[poleCount];
            for (int poleIndex = 0; poleIndex < poleCount; poleIndex++)
            {
                poles[poleIndex] = new Stack<int>();
            }
            moveHistory = new Stack<(bool, int, int)>();
            this.discCount = discCount;

            // fill the starting pole with disks
            startPole = startingPole;
            endPole = endingPole;
            for (int disc = discCount; disc > 0; disc--)
            {
                poles[startPole].Push(disc);
            }

            undoingMove = false;

            // set up the bot
            int tempPole = startingPole + 1;
            botMoving = false;
            if (tempPole == endingPole)
            {
                tempPole++;
            }
            botA = startingPole;
            botB = tempPole;
            botC = endingPole;
            if (discCount % 2 != 0)
            {
                botB = endingPole;
                botC = tempPole;
            }
            botState = 0;
        }

        public override string ToString()
        {
            string output = "";

            // convert the poles into arrays for printing
            int[,] poleArrays = new int[poleCount, discCount];
            for (int poleIndex = 0; poleIndex < poleCount; poleIndex++)
            {
                int discIndex = poles[poleIndex].Count - 1;
                foreach (int disc in poles[poleIndex])
                {
                    poleArrays[poleIndex, discIndex] = disc;
                    discIndex--;
                }
            }

            // print the contents of the arrays
            for (int disc = -1; disc < discCount; disc++)
            {
                for (int pole = 0; pole < poleCount; pole++)
                {
                    for (int side = 0; side < 2; side++)
                    {
                        bool discExists = false;
                        for (int discOffset = discCount; discOffset > 0; discOffset--)
                        {
                            if (disc >= 0 && (
                                side == 0 && poleArrays[pole, discCount - disc - 1] >= discOffset ||
                                side == 1 && poleArrays[pole, discCount - disc - 1] > discCount - discOffset))
                            {
                                output += "#";
                                discExists = true;
                            }
                            else
                            {
                                output += " ";
                            }
                        }
                        if (side == 0 && discExists)
                        {
                            output += "#";
                        }
                        else if (side == 0)
                        {
                            output += "|";
                        }
                    }
                    output += " ";
                }
                output += "\n";
            }
            for (int pole = 0; pole < poleCount; pole++)
            {
                for (int side = 0; side < 2; side++)
                {
                    for (int discOffset = 0; discOffset < discCount; discOffset++)
                    {
                        output += "=";
                    }
                    if (side == 0)
                    {
                        output += "+";
                    }
                }
                output += " ";
            }
            return output;
        }

        public bool MoveDisc(int startIndex, int endIndex)
        {
            bool status = true;
            if (startIndex < 0 || startIndex >= poleCount || endIndex < 0 || endIndex >= poleCount)
            {
                status = false;
            }
            if (poles[startIndex].Count == 0 && status)
            {
                status = false;
            }
            else if (status)
            {
                int disc = poles[startIndex].Pop();
                if (poles[endIndex].Count == 0 || poles[endIndex].Peek() > disc)
                {
                    poles[endIndex].Push(disc);
                    if (startIndex != endIndex)
                    {
                        if (!undoingMove)
                        {
                            moveHistory.Push((botMoving, startIndex, endIndex));
                            _movesTaken++;
                        }
                        else
                        {
                            MovesTaken--;
                        }
                    }
                    // check if the game is won
                    if (endIndex == endPole && poles[endPole].Count == discCount)
                    {
                        _gameWon = true;
                    }
                }
                else
                {
                    poles[startIndex].Push(disc);
                    status = false;
                }
            }
            return status;
        }

        public int PeekPole(int poleIndex)
        {
            return poles[poleIndex].Peek();
        }

        public int NumberOnPole(int poleIndex)
        {
            return poles[poleIndex].Count;
        }

        public (int,int) BotMove()
        {
            botMoving = true;
            int moveFrom = 0;
            int moveTo = 0;
            switch (botState)
            {
                case 0:
                    moveFrom = botA;
                    moveTo = botB;
                    if (!MoveDisc(botA, botB))
                    {
                        MoveDisc(botB, botA);
                        moveFrom = botB;
                        moveTo = botA;
                    }
                    break;
                case 1:
                    moveFrom = botA;
                    moveTo = botC;
                    if (!MoveDisc(botA, botC))
                    {
                        MoveDisc(botC, botA);
                        moveFrom = botC;
                        moveTo = botA;
                    }
                    break;
                case 2:
                    moveFrom = botB;
                    moveTo = botC;
                    if (!MoveDisc(botB, botC))
                    {
                        MoveDisc(botC, botB);
                        moveFrom = botC;
                        moveTo = botB;
                    }
                    break;
            }
            // check if the game was won
            if (moveTo == endPole && poles[endPole].Count == discCount)
            {
                _gameWon = true;
            }
            botState = (botState + 1) % 3;
            botMoving = false;
            return (moveFrom, moveTo);
        }

        public (int,int) UndoMove()
        {
            undoingMove = true;
            (bool, int, int) lastMove = moveHistory.Pop();
            MoveDisc(lastMove.Item3, lastMove.Item2);
            if (lastMove.Item1)
            {
                // bot move, revert the state
                botState--;
                if (botState == -1)
                {
                    botState = 2;
                }
            }
            undoingMove = false;
            return (lastMove.Item2, lastMove.Item3);
        }
    }
}
