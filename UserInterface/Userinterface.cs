using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemoryGame;
using Ex02.ConsoleUtils;
using System.Diagnostics;

namespace UserInterface
{

    public enum ePlayersSign
    {
        Computer,
        Human
    }

    public class MemoryGameUserinterface
    {
        private MemoryGameLogic m_MemoryGame;
        private bool v_IsAgainstComputer;
        private const string k_QuitSign = "Q";
        public const uint k_MaxBoardSize = 6;
        public const uint k_MinBoardSize = 4;

        public void Activate()
        {
            startScreenAndScanUserName();
            chooseIfPlayAgainstUmanOrComputer();
        }

        private void startScreenAndScanUserName()
        {
            Screen.Clear();

            StringBuilder openingMessage = new StringBuilder();

            openingMessage.Append("Wellcome to Memory game!");
            openingMessage.AppendLine();
            openingMessage.Append("==============================");
            openingMessage.AppendLine();
            openingMessage.Append($"For quit the game you can enter {k_QuitSign} any time:)");
            openingMessage.AppendLine();

            Console.WriteLine(openingMessage.ToString());

            Console.WriteLine("Enter your name: ");
            string username = Console.ReadLine();
            // Set user name in game
        }

        private void chooseIfPlayAgainstUmanOrComputer()
        {
            Console.WriteLine("Against who you want to play? ({0} / {1}):", ePlayersSign.Computer.ToString(), ePlayersSign.Human.ToString());
            string playerSignStr = Console.ReadLine();
            ePlayersSign playerSign;
            while (!(Enum.TryParse<ePlayersSign>(playerSignStr, out playerSign)))
            {
                Console.WriteLine("Invalid enemy sign input! Try Again.");
                Console.Write($"{ePlayersSign.Computer} / {ePlayersSign.Human} : ");
                playerSignStr = Console.ReadLine();
            }

            if (playerSign == ePlayersSign.Human)
            {
                v_IsAgainstComputer = false;
                Console.WriteLine("Enter other player name: ");
                string username = Console.ReadLine();
                // Set user name in game

            }
            else
            {
                v_IsAgainstComputer = true;
                // Set user name in game
            }
        }

        private void scanBoardSizeFromUser()
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
                    Console.WriteLine("Rows size input is invalid! Try again.");
                }

                rowsStr = Console.ReadLine();
                isInputValid = false;
            }
            while (!(uint.TryParse(rowsStr, out rowsSize)) || rowsSize < k_MinBoardSize || rowsSize > k_MaxBoardSize);

            m_MemoryGame.Rows = rowsSize;
            isInputValid = true;

            Console.WriteLine("Enter cols size (choose between {0} - {1}): ", k_MinBoardSize, k_MaxBoardSize);
            do
            {
                if (!isInputValid)
                {
                    Console.WriteLine("Cols size input is invalid! Try again.");
                }

                colsStr = Console.ReadLine();
                isInputValid = false;

            }
            while (!(uint.TryParse(colsStr, out colsSize)) || colsSize < k_MinBoardSize || colsSize > k_MaxBoardSize);

            m_MemoryGame.Cols = colsSize;
        }

        public void PlayTurn()
        {
            
        }

        public void scanCellsLocations()
        {
            Console.WriteLine("Enter first cell row and col (e.g. 1B): ");
            string firstCellLocationInput = Console.ReadLine();
        }

        private void isCellLocationValid(string i_CellLocation)
        {
            bool isCellValid = true;
            int row;
            char col;

            do
            {
                if (string.IsNullOrEmpty(i_CellLocation) || i_CellLocation.Length != 2)
                {
                    Console.WriteLine("Invalid input! It should contain a number and a letter only. Try again.");
                    isCellValid = false;
                }
                else
                {
                    // Seperate the input to row and col
                    if (!TryParseInput(i_CellLocation, out row, out col))
                    {
                        Console.WriteLine("Invalid input! It should first contain a number (row) and then a letter (col). Try again.");
                        isCellValid = false;
                    }
                    else
                    {
                        // Check if its in range
                        int rowInBoardMatrix = row - 1;
                        int colInBoardMatrix = convertColLetterToColInMatrix(col);

                        if (!(rowInBoardMatrix >= 0 && rowInBoardMatrix < m_MemoryGame.Rows && colInBoardMatrix >= 0 && colInBoardMatrix < m_MemoryGame.Cols))
                        {
                            Console.WriteLine("Invalid input! The cell is not in the board. Try again.");
                            isCellValid = false;
                        }
                        else
                        {
                            // Check if the cell is not exposed
                            if (m_MemoryGame.IsCellExposed(rowInBoardMatrix, colInBoardMatrix))
                            {
                                Console.WriteLine("Invalid input! The cell is already expose. Try again.");
                                isCellValid = false;
                            }
                        }
                    }

                }
            }
            while (!isCellValid);


        }

        static bool TryParseInput(string i_CellLocationInput, out int o_Row, out char o_Col)
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
    }

    
}
