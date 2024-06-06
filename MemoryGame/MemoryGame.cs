using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;



namespace MemoryGame
{
    public struct LocationOfCell
    {
        private int m_Row;
        private int m_Col;

        public LocationOfCell(int i_Row, int i_Col)
        {
            m_Row = i_Row;
            m_Col = i_Col;
        }

        public int Row
        {
            get
            {
                return m_Row;
            }
            set
            {
                m_Row = value;
            }
        }

        public int Col
        {
            get
            {
                return m_Col;
            }
            set
            {
                m_Col = value;
            }
        }
    }

    /// ///////////////////////////////////////////////////////////////////////

    public class Cell
    {
        private char m_Value;
        private LocationOfCell m_CellLocation;
        private LocationOfCell m_CoupleLocation;
        private bool v_IsExposed = false;
        
        public char Value
        {
            get
            {
                return m_Value;
            }
            set
            {
                m_Value = value;
            }
        }

        public Cell(int i_Row, int i_Col, char i_Value, LocationOfCell i_CoupleLocation)
        {
            m_CellLocation = new LocationOfCell(i_Row, i_Col);
            m_Value = i_Value;
            m_CoupleLocation = i_CoupleLocation;
        }

        public bool IsExposed()
        {
            return this.v_IsExposed;
        }
    }

    /// ///////////////////////////////////////////////////////////////////////
    public class User
    {
        private string m_Name = null;
        private int m_NumberOfPoints = 0;
        private bool v_IsComputer;


        public int NumberOfPoints
        {
            get
            {
                return m_NumberOfPoints;
            }
            set
            {
                m_NumberOfPoints = value;
            }
        }
        public string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                m_Name = value;
            }
        }
        public bool IsComputer
        {
            get
            {
                return v_IsComputer;
            }
            set
            {
                v_IsComputer = value;
            }
        }

        public User(string i_Name, bool i_IsComputer)
        {
            m_Name =  i_Name;
            v_IsComputer = i_IsComputer;
        }

    }

    //////////////////////////////////////////////////////////////////////////

    public class MemoryGameLogic
    {
        private uint m_BoardRows = 0;
        private uint m_BoardCols = 0;
        private uint m_Turn = 0;
        private uint m_TakenCells = 0;
        private Cell[,] m_MatrixBoard = null;
        private User m_FirstUser;
        private User m_SecondUser;

        public uint Rows
        {
            get
            {
                return m_BoardRows;
            }
            set
            {
                m_BoardRows = value;
            }
        }

        public uint Cols
        {
            get
            {
                return m_BoardCols;
            }
            set
            {
                m_BoardCols = value;
            }
        }

        public uint Turn
        {
            get
            {
                return m_Turn;
            }
            set
            {
                m_Turn = value;
            }
        }

        public uint TakenCells
        {
            get
            {
                return m_TakenCells;
            }
            set
            {
                m_TakenCells = value;
            }
        }

        public Cell[,] getMatrixBoard()
        {
            return m_MatrixBoard;   
        }

        public MemoryGameLogic(string i_FirstUserName, string i_SecondUserName, bool i_IsSecondPlayerComputer, uint i_Rows, uint i_Cols)
        {
            m_FirstUser = new User(i_FirstUserName, false);
            m_SecondUser = new User(i_SecondUserName, i_IsSecondPlayerComputer);
            m_MatrixBoard = new Cell[i_Rows,  i_Cols];
            m_BoardCols = i_Cols;
            m_BoardRows = i_Rows;
        }

        public void InitializeBoard()
        {
            char charToInsert = 'A';
            List<LocationOfCell> alreadyRaffledCells = new List<LocationOfCell>(); // List of full cells
            Random random = new Random();
            uint numberOfLetters = m_BoardCols * m_BoardRows / 2;

            for (int i = 0; i < numberOfLetters; i++)
            {
                // Raffled two cell's locations
                int firstCellRow = random.Next(0, (int)m_BoardRows - 1);
                int firstCellCol = random.Next(0, (int)m_BoardRows - 1);

                // Check if this cell already Raffled
                while (isCellAlreadyRaffled(alreadyRaffledCells, firstCellRow, firstCellCol))
                {
                    firstCellRow = random.Next(0, (int)m_BoardRows - 1);
                    firstCellCol = random.Next(0, (int)m_BoardRows - 1);
                }

                alreadyRaffledCells.Add(new LocationOfCell(firstCellRow, firstCellCol));

                int secondCellRow = random.Next(0, (int)m_BoardRows - 1);
                int secondCellCol = random.Next(0, (int)m_BoardRows - 1);

                while (isCellAlreadyRaffled(alreadyRaffledCells, firstCellRow, firstCellCol))
                {
                    firstCellRow = random.Next(0, (int)m_BoardRows - 1);
                    firstCellCol = random.Next(0, (int)m_BoardRows - 1);
                }

                alreadyRaffledCells.Add(new LocationOfCell(secondCellRow, secondCellCol));

                // Set the value
                m_MatrixBoard[firstCellRow, firstCellCol].Value = charToInsert;
                m_MatrixBoard[secondCellRow, secondCellCol].Value = charToInsert;

                // Move to next char
                charToInsert += '1';
            }
        }

        private bool isCellAlreadyRaffled(List<LocationOfCell> i_RaffledCellsList, int i_Rows, int i_Cols)
        {
            bool isInList = true;

            for (int i = 0; i < i_RaffledCellsList.Count; i++)
            {
                if (i_RaffledCellsList[i].Row == i_Rows && i_RaffledCellsList[i].Col == i_Cols)
                {
                    isInList = false;
                }
            }

            return !isInList;
        }

        public bool IsBoardSizeEven()
        {
            return (m_BoardRows * m_BoardCols) % 2 == 0 ;
        }

        public void CurrentTurn()
        {

        }
        public void SwitchToNextPlayer()
        {
            if(m_Turn == 0)
            {
                m_Turn = 1;
            }
            else
            {
                m_Turn = 0;
            }
        }
        public string WhichPlayerWon()
        {
            if(m_FirstUser.NumberOfPoints > m_SecondUser.NumberOfPoints)
            {
                return m_FirstUser.Name + " won with " + m_FirstUser.NumberOfPoints + " points.";
            }
            else if(m_FirstUser.NumberOfPoints < m_SecondUser.NumberOfPoints)
            {
                return m_SecondUser.Name + " won with " + m_SecondUser.NumberOfPoints + " points.";
            }
            else
            {
                return "Its a Tie.";
            }
        }
        public bool IsBoardFull()
        {
            return m_TakenCells == m_BoardCols * m_BoardRows;
        }

        public bool IsCellExposed(int i_Row, int i_Col)
        {
            return m_MatrixBoard[i_Row, i_Col].IsExposed();
        }
    }
}
