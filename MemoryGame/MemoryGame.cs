﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;



namespace MemoryGame
{
    ///////////////////////////////////////////// LocationOfCell /////////////////////////////////////////////
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

    ///////////////////////////////////////////// Cell /////////////////////////////////////////////

    public class Cell
    {
        private char m_Value;
        private LocationOfCell m_CellLocation;
        private bool m_IsExposed;
        
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

        public Cell(int i_Row, int i_Col, char i_Value)
        {
            m_CellLocation = new LocationOfCell(i_Row, i_Col);
            m_Value = i_Value;
            m_IsExposed = false;
        }
 
        public bool IsExposed()
        {
            return this.m_IsExposed;
        }

        public void SetExposed()
        {
            this.m_IsExposed = true;
        }

        public void UndoExposed()
        {
            this.m_IsExposed = false;   
        }
    }

    ///////////////////////////////////////////// User /////////////////////////////////////////////
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

    ///////////////////////////////////////////// MemoryGameLogic /////////////////////////////////////////////

    public class MemoryGameLogic
    {
        private uint m_BoardRows = 0;
        private uint m_BoardCols = 0;
        private uint m_Turn = 1;
        private uint m_TakenCells = 0;
        private Cell[,] m_MatrixBoard = null;
        private User m_FirstUser;
        private User m_SecondUser;
        private List<LocationOfCell> m_UnexposedCellsList = null; 
      
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

        public string FirstUserName
        {
            get
            {
                return m_FirstUser.Name;
            }
        }

        public string SecondUserName
        {
            get
            {
                return m_SecondUser.Name;
            }
        }

        public User GetUser(string i_numberOfUser)
        {
            if(i_numberOfUser.Equals("first"))
            {
                return m_FirstUser;
            }
            else
            {
                return m_SecondUser;
            }
        }

        public Cell[,] MatrixBoard
        {
            get
            {
                return m_MatrixBoard;
            }
            
        }

        public string GetCurrentPlayerName()
        {
            string currentPlayerName;

            if(Turn == 1)
            {
                currentPlayerName = m_FirstUser.Name;
            }
            else
            {
                currentPlayerName = m_SecondUser.Name;
            }
            return currentPlayerName;
        }

        public MemoryGameLogic(string i_FirstUserName, string i_SecondUserName, bool i_IsSecondPlayerComputer,uint i_Rows, uint i_Cols)
        {
            m_FirstUser = new User(i_FirstUserName, false);
            m_SecondUser = new User(i_SecondUserName, i_IsSecondPlayerComputer);
            m_MatrixBoard = new Cell[i_Rows, i_Cols];
            m_UnexposedCellsList = new List<LocationOfCell>();
            m_BoardCols = i_Cols;
            m_BoardRows = i_Rows;
            for (int i = 0; i < i_Rows; i++) 
            {
                for (int j = 0; j < i_Cols; j++)
                {
                    m_MatrixBoard[i, j] = new Cell(i, j, '\0');
                    m_UnexposedCellsList.Add(new LocationOfCell(i,  j));
                }
            }

            initializeBoardBySettingValues();
        }

        public void GetPointOfPlayers(out int o_FirstPlayerPoints, out int o_SecondPlayerPoints)
        {
            o_FirstPlayerPoints = m_FirstUser.NumberOfPoints;
            o_SecondPlayerPoints = m_SecondUser.NumberOfPoints;
        }

        private void initializeBoardBySettingValues()
        {
            // Set couples and their value. 

            char charToInsert = 'A';
            List<LocationOfCell> alreadyRaffledCells = new List<LocationOfCell>(); // List of full cells
            Random random = new Random();
            uint numberOfLetters = m_BoardCols * m_BoardRows / 2;

            for (int i = 0; i < numberOfLetters; i++)
            {
                // Raffled two cell's locations
                int firstCellRow = random.Next(0, (int)m_BoardRows);
                int firstCellCol = random.Next(0, (int)m_BoardCols);

                // Check if this cell already Raffled
                while (isCellAlreadyRaffled(alreadyRaffledCells, firstCellRow, firstCellCol))
                {
                    firstCellRow = random.Next(0, (int)m_BoardRows);
                    firstCellCol = random.Next(0, (int)m_BoardCols);
                }

                alreadyRaffledCells.Add(new LocationOfCell(firstCellRow, firstCellCol));
                int secondCellRow = random.Next(0, (int)m_BoardRows);
                int secondCellCol = random.Next(0, (int)m_BoardCols);

                while (isCellAlreadyRaffled(alreadyRaffledCells, secondCellRow, secondCellCol))
                {
                    secondCellRow = random.Next(0, (int)m_BoardRows);
                    secondCellCol = random.Next(0, (int)m_BoardCols);
                }

                alreadyRaffledCells.Add(new LocationOfCell(secondCellRow, secondCellCol));
                // Set the value
                m_MatrixBoard[firstCellRow, firstCellCol].Value = charToInsert;
                m_MatrixBoard[secondCellRow, secondCellCol].Value = charToInsert;
                // Move to next char
                charToInsert++ ;
            }
        }

        private bool isCellAlreadyRaffled(List<LocationOfCell> i_RaffledCellsList, int i_Rows, int i_Cols)
        {
            bool isInList = false;

            for (int i = 0; i < i_RaffledCellsList.Count; i++)
            {
                if (i_RaffledCellsList[i].Row == i_Rows && i_RaffledCellsList[i].Col == i_Cols)
                {
                    isInList = true;
                    break;
                }
            }

            return isInList;
        }

        public static bool IsBoardSizeEven(uint i_Rows,  uint i_Cols)
        {
            return (i_Rows * i_Cols) % 2 == 0;
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

        public void SetExpose(int i_Row, int i_Col)
        {
            m_MatrixBoard[i_Row, i_Col].SetExposed();
            m_UnexposedCellsList.Remove(new LocationOfCell(i_Row,  i_Col));
        }

        public void UndoExposed(int i_Row, int i_Col)
        {
            m_MatrixBoard[i_Row, i_Col].UndoExposed();
            m_UnexposedCellsList.Add(new LocationOfCell(i_Row, i_Col));
        }

        public bool CheckIfCuple(LocationOfCell i_FirstLocation , LocationOfCell i_SecondLocation)
        {
            bool isCuple = true;
            if (MatrixBoard[i_FirstLocation.Row, i_FirstLocation.Col].Value != MatrixBoard[i_SecondLocation.Row, i_SecondLocation.Col].Value)
            {
                UndoExposed(i_FirstLocation.Row,i_FirstLocation.Col);
                UndoExposed(i_SecondLocation.Row,i_SecondLocation.Col);
                isCuple = false;
            }
            else
            {
                m_TakenCells += 2;
            }

            return isCuple;
        }
         
        public bool IsCellLocationValid(int i_CellRow, int i_CellCol, out string o_ErrorMsg)
        {
            bool isCellValid = false;

            if (!(i_CellRow >= 0 && i_CellRow < m_BoardRows && i_CellCol >= 0 && i_CellCol < m_BoardCols))
            {
                o_ErrorMsg = "The cell is out of range. Try again.";
            }
            else
            {
                if (IsCellExposed(i_CellRow, i_CellCol))
                {
                    o_ErrorMsg = "This cell is already exposed. Try again.";
                    isCellValid = false;
                }
                else
                {
                    o_ErrorMsg = null;
                    isCellValid = true;
                }
            }

            return isCellValid;
        }

        public void RandomOneCellLocation(out LocationOfCell o_LocationCell)
        {
            int cellRow = 0;
            int cellCol = 0;
            int indexOfList;
            o_LocationCell = new LocationOfCell();
            Random random = new Random();

            do
            {
                indexOfList = random.Next(m_UnexposedCellsList.Count);
                cellRow  = m_UnexposedCellsList[indexOfList].Row;
                cellCol = m_UnexposedCellsList[indexOfList].Col;
            }
            while (IsCellExposed(cellRow, cellCol));

            o_LocationCell.Row = cellRow;
            o_LocationCell.Col = cellCol;
            SetExpose(cellRow, cellCol);
        }

        public void NextTurn(LocationOfCell i_FirstLocation, LocationOfCell i_SecondLocation)
        {
            if(CheckIfCuple(i_FirstLocation, i_SecondLocation))
            {
                if  (Turn == 1)
                {
                    m_FirstUser.NumberOfPoints += 1;
                }
                else
                {
                    m_SecondUser.NumberOfPoints += 1;
                }
            }

           if(Turn == 1)
            {
                Turn = 2;
            }
            else
            {
                Turn =  1;
            }
        }

        public bool IsComputerPlaying()
        {
            bool isComputerPlaying = false;

            if (m_Turn == 1)
            {
                // Player 1 is playing
                isComputerPlaying = m_FirstUser.IsComputer;
            }
            else
            {
                // Player 2 is playing
                isComputerPlaying = m_SecondUser.IsComputer;
            }

            return isComputerPlaying;
        }

    }
}
