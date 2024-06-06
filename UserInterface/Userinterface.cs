using System;
using System.Text;
using MemoryGame;
using Ex02.ConsoleUtils;

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
                
                while (playingCurrentGame)
                {
                    playingCurrentGame = PlayTurn(m_MemoryGame.IsComputerPlaying(),out isPressQ);
                    if(isPressQ)
                    {
                        playingCurrentGame = false;
                        isPlayingGame = false;
                    }
                }

                Console.WriteLine($"Do you want to play another game? ({k_QuitSign} - no, any key - yes)");
                string anotherGameOrQuit = Console.ReadLine();
                if ( anotherGameOrQuit.Equals(k_QuitSign))
                {
                    isPlayingGame = false;
                }
            }

            Console.WriteLine("Bye Bye! :)");
        }

        private void startScreenAndScanUserName(out string o_Username)
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
            o_Username =  Console.ReadLine();
        }

        private void chooseIfPlayAgainstHumanOrComputer(out string o_SecondPlayerName)
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
                m_IsAgainstComputer = false;
                Console.WriteLine("Enter other player name: ");
                o_SecondPlayerName = Console.ReadLine();
            }
            else
            {
                m_IsAgainstComputer = true;
                o_SecondPlayerName = "Computer";
            }
        }

        private void scanBoardFromUser(out uint o_RowsSize, out uint o_ColsSize)
        {
            bool isInputValid = true;

            do
            {
                scanRowsAndCols(out o_RowsSize, out o_ColsSize);
                if (!MemoryGameLogic.IsBoardSizeEven(o_ColsSize, o_RowsSize))
                {
                    Console.WriteLine("Input is invalid! The board size shoul be even. Try again.");
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
                    Console.WriteLine("Rows size input is invalid! Try again.");
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
                    Console.WriteLine("Cols size input is invalid! Try again.");
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

            if (i_IsComputer)
            {
                m_MemoryGame.RandomCellsLocation(out firstLocationCell, out secondLocationCell);
            }
            else
            {
                ScanCellsLocations(out firstLocationCell, out secondLocationCell, out o_IsPressQ);
            }

            m_MemoryGame.NextTurn(firstLocationCell, secondLocationCell);

            // Check if the game is over
            return m_MemoryGame.IsBoardFull();
        }

        
        public void ScanCellsLocations(out LocationOfCell o_FirstLocationCell, out LocationOfCell o_SecondLocationCell, out bool o_IsPressQ)
        {
            string firstCellLocationInput;
            string secondCellLocationInput;
            o_IsPressQ = false;

            // Initialize default values in case of pressing 'Q'
            o_FirstLocationCell = new LocationOfCell(-1, -1);
            o_SecondLocationCell = new LocationOfCell(-1, -1);

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
                do
                {
                    Console.WriteLine("Enter second cell row and col (e.g. 1B): ");
                    secondCellLocationInput = Console.ReadLine();
                    if  (firstCellLocationInput.Equals(k_QuitSign))
                    {
                        o_IsPressQ = true;
                        break;
                    }
                }
                while (!isCellLocationValid(secondCellLocationInput, out o_SecondLocationCell));
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
                Console.WriteLine("Invalid input! It should contain a number and a letter only. Try again.");
            }
            else
            {
                // Seperate the input to row and col
                if (!TryParseInput(i_CellLocationInput, out row, out col))
                {
                    Console.WriteLine("Invalid input! It should first contain a number (row) and then a letter (col). Try again.");
                }
                else
                {
                    // The input is in the right way, need to continue the validation from the logic
                    int rowInBoardMatrix = row - 1;
                    int colInBoardMatrix = convertColLetterToColInMatrix(col);
                    string errorMsg = null;

                    if (!m_MemoryGame.isCellLocationValid(rowInBoardMatrix, colInBoardMatrix, out errorMsg))
                    {
                        Console.WriteLine(errorMsg);
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

        private void printBoard()
        {
            Screen.Clear();

            StringBuilder boardSb = new StringBuilder();

            for (int i = 0; i < m_MemoryGame.Cols; i ++)
            {
                Console.Write("\t");

                Console.Write((char)('A' + i));
            }
            Console.Write("\n");

            for (int i = 0; i < m_MemoryGame.Cols; i++)
            {
                Console.Write("=====");
            }
            Console.Write("\n");

            for (int i = 0; i < m_MemoryGame.Rows; i++)
            {
                Console.Write($"{i + 1} ");

                for (int j = 0; j < m_MemoryGame.Cols; j++)
                {
                    Console.Write($"|");
                    if(m_MemoryGame.MatrixBoard[i,j].IsExposed())
                    {
                        Console.Write($"{m_MemoryGame.MatrixBoard[i, j].Value} ");
                    }
                    else
                    {
                        Console.Write("\t");

                    }
                    Console.Write($"|");
                }
            }
        }
    }
}
