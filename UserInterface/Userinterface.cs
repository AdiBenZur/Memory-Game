﻿using System;
using System.Text;
using MemoryGame;
using Ex02.ConsoleUtils;
using System.Threading;

namespace UserInterface
{
    ///////////////////////////////////////////// ePlayerSign /////////////////////////////////////////////
    public enum ePlayersSign
    {
        Computer,
        Human
    }

    ///////////////////////////////////////////// MemoryGameUserinterface /////////////////////////////////////////////
    public class MemoryGameUserinterface
    {
        private MemoryGameLogic m_MemoryGame;
        private bool m_IsAgainstComputer;
        private const string k_QuitSign = "Q";
        public const uint k_MaxBoardSize = 6;
        public const uint k_MinBoardSize = 4;

        public void Activate()
        {
            bool isPlayingGame = true;
            bool playingCurrentGame = true;
            string firstPlayerUsername, secondPlayerUsername;
            uint rowsSize, colsSize;
            bool isPressQ = false;

            startScreenAndScanUserName(out firstPlayerUsername);
            chooseIfPlayAgainstHumanOrComputer(out secondPlayerUsername);

            while (isPlayingGame)
            {
                // scan board size from user 
                scanBoardFromUser(out rowsSize, out colsSize);
                m_MemoryGame = new MemoryGameLogic(firstPlayerUsername, secondPlayerUsername, m_IsAgainstComputer, rowsSize, colsSize);
                printBoard();
                playingCurrentGame = true;

                while (playingCurrentGame)
                {
                    playingCurrentGame = PlayTurn(m_MemoryGame.IsComputerPlaying(), out isPressQ);
                    if(isPressQ)
                    {
                        playingCurrentGame = false;
                        isPlayingGame = false;
                    }
                }

                if (!isPressQ)
                {
                    // Need to contine
                    printBoard();
                    printMessageWhenGameIsFinish();
                    Console.WriteLine($"Do you want to play another game? ({k_QuitSign} - no, any key - yes)");
                    string anotherGameOrQuit = Console.ReadLine();
                    if (anotherGameOrQuit.Equals(k_QuitSign))
                    {
                        isPlayingGame = false;
                    }
                }
                    
            }

            Console.WriteLine("\nBye Bye! :)");
            Thread.Sleep(2000);
        }

        private void startScreenAndScanUserName(out string o_Username)
        {
            Screen.Clear();
            StringBuilder openingMessage = new StringBuilder();

            openingMessage.Append("Wellcome to Memory game!");
            openingMessage.AppendLine();
            openingMessage.Append("==============================");
            openingMessage.AppendLine();
            openingMessage.Append($"For quit the game you can enter {k_QuitSign} as long as the game started:)");
            openingMessage.AppendLine();
            Console.WriteLine(openingMessage.ToString());
            Console.WriteLine("Enter your name: ");
            o_Username =  Console.ReadLine();
        }

        private void chooseIfPlayAgainstHumanOrComputer(out string o_SecondPlayerName)
        {
            Console.WriteLine("Against who you want to play? press 0 to Computer and 1 for Human.");
            string playerSignStr = Console.ReadLine();
            ePlayersSign playerSign;
            while (!(Enum.TryParse(playerSignStr, out playerSign)) || !Enum.IsDefined(typeof(ePlayersSign), playerSign))
            {
                Console.WriteLine("Invalid player sign input! Try Again.");
                Console.Write($"{ePlayersSign.Computer} / {ePlayersSign.Human} : ");
                playerSignStr = Console.ReadLine();
            }

            if (playerSign == ePlayersSign.Human)
            {
                m_IsAgainstComputer = false;
                Console.WriteLine("Enter second player's name: ");
                o_SecondPlayerName = Console.ReadLine();
            }
            else
            {
                m_IsAgainstComputer = true;
                o_SecondPlayerName = "The Computer";
            }
        }

        private void scanBoardFromUser(out uint o_RowsSize, out uint o_ColsSize)
        {
            bool isInputValid;

            do
            {
                isInputValid = true;
                scanRowsAndCols(out o_RowsSize, out o_ColsSize);
                if (!MemoryGameLogic.IsBoardSizeEven(o_ColsSize, o_RowsSize))
                {
                    Console.WriteLine("Input is invalid! The board size should be even. Try again.\n");
                    isInputValid = false;
                }
            } 
            while (!isInputValid);
        }
     
        private void scanRowsAndCols(out uint o_RowsSize, out uint o_ColsSize)
        {
            string rowsStr;
            uint rowsSize;
            string colsStr;
            uint colsSize;
            bool isInputValid = true;

            Console.WriteLine("Enter rows size (choose between {0} - {1}): ", k_MinBoardSize, k_MaxBoardSize);
            do
            {
                if (!isInputValid)
                {
                    Console.WriteLine("Rows size input is invalid! Its should be a value between {0} - {1}. Try again.\n", k_MinBoardSize, k_MaxBoardSize);
                }

                rowsStr = Console.ReadLine();
                isInputValid = false;
            }
            while (!(uint.TryParse(rowsStr, out rowsSize)) || rowsSize < k_MinBoardSize || rowsSize > k_MaxBoardSize);

            isInputValid = true;
            o_RowsSize = rowsSize;
            Console.WriteLine("Enter cols size (choose between {0} - {1}): ", k_MinBoardSize, k_MaxBoardSize);
            do
            {
                if (!isInputValid)
                {
                    Console.WriteLine("Rows size input is invalid! Its should be a value between {0} - {1}. Try again.\n", k_MinBoardSize, k_MaxBoardSize);
                }

                colsStr = Console.ReadLine();
                isInputValid = false;

            }
            while (!(uint.TryParse(colsStr, out colsSize)) || colsSize < k_MinBoardSize || colsSize > k_MaxBoardSize);

            o_ColsSize = colsSize;
        }

        public bool PlayTurn(bool i_IsComputer, out bool o_IsPressQ)
        {
            LocationOfCell firstLocationCell ;
            LocationOfCell secondLocationCell;
            o_IsPressQ = false;

            printBoard();
            if (i_IsComputer)
            {
                m_MemoryGame.RandomOneCellLocation(out firstLocationCell);
                printScreenWhenComputerPlay();
                m_MemoryGame.RandomOneCellLocation(out secondLocationCell);
                printScreenWhenComputerPlay();
            }
            else
            {
                ScanCellsLocations(out firstLocationCell, out secondLocationCell, out o_IsPressQ);
            }

            if (!o_IsPressQ)
            {
                Thread.Sleep(2000);
                m_MemoryGame.NextTurn(firstLocationCell, secondLocationCell);
            }

            // Check if the game is over. If not, return that the game is not over yet.
            return !m_MemoryGame.IsBoardFull();
        }

        private void printScreenWhenComputerPlay()
        {
            printBoard();
            printCurrentPlayerName();
            Thread.Sleep(2000);
        }

        private void printCurrentPlayerName()
        {
            string currentPlayerName = m_MemoryGame.GetCurrentPlayerName();
            Console.WriteLine("Its " + currentPlayerName + "'s turn!\n");
        }

        public void ScanCellsLocations(out LocationOfCell o_FirstLocationCell, out LocationOfCell o_SecondLocationCell, out bool o_IsPressQ)
        {
            string firstCellLocationInput;
            string secondCellLocationInput;
            o_IsPressQ = false;

            // Initialize default values in case of pressing 'Q'
            o_FirstLocationCell = new LocationOfCell(-1, -1);
            o_SecondLocationCell = new LocationOfCell(-1, -1);
            printCurrentPlayerName();
            do
            {
                Console.WriteLine("Enter first cell row and col (e.g. 1B): ");
                firstCellLocationInput = Console.ReadLine();
                if(firstCellLocationInput.Equals(k_QuitSign))
                {
                   o_IsPressQ = true;
                   break;
                }
            }
            while  (!isCellLocationValid(firstCellLocationInput, out o_FirstLocationCell));

            if  (!o_IsPressQ)
            {
                printBoard();
                printCurrentPlayerName();
                do
                {
                    Console.WriteLine("Enter second cell row and col (e.g. 1B): ");
                    secondCellLocationInput = Console.ReadLine();
                    if  (secondCellLocationInput.Equals(k_QuitSign))
                    {
                        o_IsPressQ = true;
                        break;
                    }
                }
                while (!isCellLocationValid(secondCellLocationInput, out o_SecondLocationCell));

                if (!o_IsPressQ)
                {
                    printBoard();
                }
            }
        }

        private bool isCellLocationValid(string i_CellLocationInput, out  LocationOfCell o_UserCellLocationChoice)
        {
            bool isCellValid = false;
            int row;
            char col;
            o_UserCellLocationChoice = new LocationOfCell(-1,-1);
           
            if (string.IsNullOrEmpty(i_CellLocationInput) || i_CellLocationInput.Length != 2)
            {
                Console.WriteLine("Invalid input! It should contain a number and a letter only. Try again.\n");
            }
            else
            {
                // Seperate the input to row and col
                if (!tryParseInput(i_CellLocationInput, out row, out col))
                {
                    Console.WriteLine("Invalid input! It should first contain a number (row) and then a letter (col). Try again.\n");
                }
                else
                {
                    // The input is in the right way, need to continue the validation from the logic
                    int rowInBoardMatrix = row - 1;
                    int colInBoardMatrix = convertColLetterToColInMatrix(col);
                    string errorMsg = null;

                    if (!m_MemoryGame.IsCellLocationValid(rowInBoardMatrix, colInBoardMatrix, out errorMsg))
                    {
                        Console.WriteLine(errorMsg + "\n");
                    }
                    else
                    {
                        // The cell that the user choose in valid.
                        isCellValid = true;
                        m_MemoryGame.SetExpose(rowInBoardMatrix, colInBoardMatrix);
                        o_UserCellLocationChoice.Row = rowInBoardMatrix;
                        o_UserCellLocationChoice.Col = colInBoardMatrix;
                    }
                }
            }

            return isCellValid;
        }

        private bool tryParseInput(string i_CellLocationInput, out int o_Row, out char o_Col)
        {
            bool isParsingSucceed = true;
            o_Row = 0;
            o_Col = '\0';

            // Extract the numeric part
            string numberPart = i_CellLocationInput.Substring(0, i_CellLocationInput.Length - 1);
            // Extract the character part
            char letterPart = i_CellLocationInput[i_CellLocationInput.Length - 1];
            // Validate the numeric part
            if (int.TryParse(numberPart, out o_Row) && char.IsLetter(letterPart))
            {
                o_Col = letterPart;
            }
            else
            {
                isParsingSucceed = false;
            }

            return isParsingSucceed;
        }

        private int convertColLetterToColInMatrix(char i_CollLetter)
        {
            // 'A' is 0, 'B' is 1, 'C' is 2 ...
            i_CollLetter = char.ToUpper(i_CollLetter);

            return i_CollLetter - 'A';
        }

        private void printMessageWhenGameIsFinish()
        {
            int firstUserPoints = 0;
            int secondUserPoints = 0;

            m_MemoryGame.GetPointOfPlayers(out firstUserPoints, out secondUserPoints);
            StringBuilder usersScoresStr = new StringBuilder();
            usersScoresStr.Append(m_MemoryGame.FirstUserName + "'s score: ").Append(firstUserPoints).Append(".");
            usersScoresStr.AppendLine();
            usersScoresStr.Append(m_MemoryGame.SecondUserName + "'s score: ").Append(secondUserPoints).Append(".");

            if (firstUserPoints > secondUserPoints)
            {
                Console.WriteLine("The winner is " + m_MemoryGame.FirstUserName + "!!!");
            }
            else
            {
                if (secondUserPoints > firstUserPoints)
                {
                    Console.WriteLine("The winner is " + m_MemoryGame.SecondUserName + "!!!");
                }
                else
                {
                    // A tie
                    Console.WriteLine("It's a tie!");
                }
            }

            Console.WriteLine(usersScoresStr.ToString() + "\n");
        }

        private void printBoard()
        {
            Screen.Clear();
            StringBuilder boardSb = new StringBuilder();

            // Add cols letters
            boardSb.Append("   ");
            for (int i = 0; i < m_MemoryGame.Cols; i ++)
            {
                boardSb.Append("   " + (char)('A' + i) + "  ");
            }
            boardSb.AppendLine();
            boardSb.Append("   ");

            // Add seperator line
            for (int i = 0; i < m_MemoryGame.Cols; i++)
            {
                boardSb.Append("======");
            }
            boardSb.AppendLine("=");

            // Add cells
            for (int i = 0; i < m_MemoryGame.Rows; i++)
            {
                boardSb.Append((i + 1).ToString().PadLeft(2) + " |");
                for (int j = 0; j < m_MemoryGame.Cols; j++)
                {
                    if(m_MemoryGame.MatrixBoard[i,j].IsExposed())
                    {
                        // Need to add value
                        boardSb.Append("  " + m_MemoryGame.MatrixBoard[i, j].Value + "  |");
                    }
                    else
                    {
                        // Dont need to add value
                        boardSb.Append("     |");
                    }
                }

                boardSb.AppendLine();
                boardSb.Append("   ");
                for (int j = 0; j < m_MemoryGame.Cols; j++)
                {
                    boardSb.Append("======");
                }

                boardSb.AppendLine("=");
            }

            Console.WriteLine(boardSb.ToString());
        }
    }
}
