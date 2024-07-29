using System;
using System.IO;
using System.Text;

namespace LevelDesigner
{
    public class Level : ILevel
    {
        public string levelName;
        public int LevelWidth { get; set; }
        public int LevelHeight { get; set; }
        private bool validLevel;
        public Part[,] levelGrid;
        public int levelCount;
        public int pieceCount = 0;
        public int playerCount { get; set; }
        public int gridX;
        public int gridY;
        public int startXPos = 0;
        public int startYPos = 0;
        public int XPlayerPos { get; set; }
        public int YPlayerPos { get; set; }
        private Filer filer = new Filer();
        public Level()
        {}

        public void SetStartX(int startX)
        {
            startXPos = startX;
        }

        public void SetStartY(int startY)
        {
            startYPos = startY;
        }

        public static string LoadFromFile(string file)
        {
            StreamReader fileReader;
            fileReader = new StreamReader(file);
            string fileContent = File.ReadAllText(file);
            fileReader.Close();
            Console.WriteLine(fileContent);
            return fileContent;
        }

        public void LoadLevel(string level)
        {
            string[] lines = level.Split(
                new string[] { "\r\n" },
                StringSplitOptions.None);

            var colCount = lines[0].Length;
            var rowCount = colCount;

            LevelHeight = rowCount;
            LevelWidth = colCount;

            CreateLevel(rowCount, colCount);

            for (int row = 0; row < rowCount; row++)
            {
                for (int column = 0; column < colCount; column++)
                {
                    var letter = lines[column][row];
                    switch (letter)
                    {
                        case 'E':
                            AddEmpty(row, column);
                            break;
                        case 'K':
                            AddKing(row, column);
                            break;
                        case 'R':
                            AddRook(row, column);
                            break;
                        case 'B':
                            AddBishop(row, column);
                            break;
                        case 'N':
                            AddKnight(row, column);
                            break;
                        case 'Q':
                            AddQueen(row, column);
                            break;
                        case 'P':
                            AddPawn(row, column);
                            break;
                        case 'e':
                            AddPlayerOnEmpty(row, column);
                            break;
                        case 'k':
                            AddPlayerOnKing(row, column);
                            break;
                        case 'r':
                            AddPlayerOnRook(row, column);
                            break;
                        case 'b':
                            AddPlayerOnBishop(row, column);
                            break;
                        case 'n':
                            AddPlayerOnKnight(row, column);
                            break;
                        case 'q':
                            AddPlayerOnQueen(row, column);
                            break;
                        case 'p':
                            AddPlayerOnPawn(row, column);
                            break;
                        default:
                            Console.WriteLine($"Piece is Invalid: {letter}");
                            break;
                    }
                }
            }
            CheckValid();
        }

        public void CreateLevel(int x, int y)
        {
            LevelHeight = x;
            LevelWidth = y;
            levelGrid = new Part[LevelHeight, LevelWidth];
            levelCount++;

            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    AddEmpty(i, j);
                    Console.Write((char)Part.Empty);
                }
            }
            pieceCount = 0;
        }

        public void SetLevelName(string name)
        {
            levelName = name;
        }

        public string GetLevelName() => levelName;

        public bool CheckValid()
        {
            validLevel = true; 

            if (LevelHeight == 0 && LevelWidth == 0)
            {
                validLevel = false;
                throw new Exception("Generation failed");
            }
            else if (LevelHeight < 3 || LevelWidth < 3) 
            {
                validLevel = false;
                throw new Exception("Level size too small");
            }
            else if (pieceCount < 3)
            {
                validLevel = false;
                throw new Exception("Incorrect amount of pieces");
            }
            else if (startXPos != 0 || startYPos != 0) 
            {
                validLevel = false;
                throw new Exception("Wrong starting position");
            }
            else if (playerCount == 0 || playerCount > 1) 
            {
                validLevel = false;
                if (playerCount == 0)
                    throw new Exception("Must have a player");
                else
                    throw new Exception("Too many players");
            }

            return validLevel;
        }

        public void PlayerCount() { playerCount++; }

        public void UpdatePlayerPos(int x, int y)
        {
            XPlayerPos = x;
            YPlayerPos = y;
        }

        public bool IsFinished()
        {
            if (XPlayerPos == LevelHeight && YPlayerPos == LevelWidth)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public void SaveTheLevel(string level)
        {
            filer.Save("savelevel.txt", level, levelName);
        }


        public int GetLevelHeight() { return LevelHeight; }

        public int GetLevelWidth() { return LevelWidth; }

        public int GetPlayerX() { return XPlayerPos; }

        public int GetPlayerY() { return YPlayerPos; }

        private void Move(Part piece, int gridX, int gridY)
        {
            if ((gridX >= 0 && gridX < LevelHeight) && (gridY >= 0 && gridY < LevelWidth))
            {
                levelGrid[gridX, gridY] = piece;
            }
            else
            {
                throw new NotImplementedException("Can only add piece in your selected parameters");
            }
        }

        public void RemovePiece(int gridX, int gridY)
        {
            if ((gridX >= 0 && gridX < LevelHeight) && (gridY >= 0 && gridY < LevelWidth))
            {
                levelGrid[gridX, gridY] = Part.Empty;
                pieceCount--;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public void AddEmpty(int gridX, int gridY)
        {
            if (levelGrid[gridX, gridY] != Part.Empty) pieceCount--;

            if (levelGrid[gridX, gridY] == Part.PlayerOnRook ||
                levelGrid[gridX, gridY] == Part.PlayerOnBishop ||
                levelGrid[gridX, gridY] == Part.PlayerOnKnight ||
                levelGrid[gridX, gridY] == Part.PlayerOnQueen ||
                levelGrid[gridX, gridY] == Part.PlayerOnPawn ||
                levelGrid[gridX, gridY] == Part.PlayerOnKing)
            {
                playerCount--;
            }

            Move(Part.Empty, gridX, gridY);
        }

        public void AddKing(int gridX, int gridY)
        {
            if (levelGrid[gridX, gridY] == Part.Empty) pieceCount++;
            Move(Part.King, gridX, gridY);
        }

        public void AddPawn(int gridX, int gridY)
        {
            if (levelGrid[gridX, gridY] == Part.Empty) pieceCount++;
            Move(Part.Pawn, gridX, gridY);
        }

        public void AddRook(int gridX, int gridY)
        {
            if (levelGrid[gridX, gridY] == Part.Empty) pieceCount++;
            Move(Part.Rook, gridX, gridY);
        }

        public void AddBishop(int gridX, int gridY)
        {
            if (levelGrid[gridX, gridY] == Part.Empty) pieceCount++;
            Move(Part.Bishop, gridX, gridY);
        }

        public void AddKnight(int gridX, int gridY)
        {
            if (levelGrid[gridX, gridY] == Part.Empty) pieceCount++;
            Move(Part.Knight, gridX, gridY);
        }

        public void AddQueen(int gridX, int gridY)
        {
            if (levelGrid[gridX, gridY] == Part.Empty) pieceCount++;
            Move(Part.Queen, gridX, gridY);
        }

        public void AddPlayerOnQueen(int gridX, int gridY)
        {
            Move(Part.PlayerOnQueen, gridX, gridY);
            UpdatePlayerPos(gridX, gridY);
            pieceCount++;
            PlayerCount();
        }

        public void AddPlayerOnPawn(int gridX, int gridY)
        {
            Move(Part.PlayerOnPawn, gridX, gridY);
            UpdatePlayerPos(gridX, gridY);
            pieceCount++;
            PlayerCount();
        }

        public void AddPlayerOnKing(int gridX, int gridY)
        {
            Move(Part.PlayerOnKing, gridX, gridY);
            UpdatePlayerPos(gridX, gridY);
            pieceCount++;
            PlayerCount();
        }

        public void AddPlayerOnBishop(int gridX, int gridY)
        {
            Move(Part.PlayerOnBishop, gridX, gridY);
            UpdatePlayerPos(gridX, gridY);
            pieceCount++;
            PlayerCount();
        }

        public void AddPlayerOnRook(int gridX, int gridY)
        {
            Move(Part.PlayerOnRook, gridX, gridY);
            UpdatePlayerPos(gridX, gridY);
            pieceCount++;
            PlayerCount();
        }

        public void AddPlayerOnKnight(int gridX, int gridY)
        {
            Move(Part.PlayerOnKnight, gridX, gridY);
            UpdatePlayerPos(gridX, gridY);
            pieceCount++;
            PlayerCount();
        }

        public void AddPlayerOnEmpty(int gridX, int gridY)
        {
            Move(Part.PlayerOnEmpty, gridX, gridY);
            UpdatePlayerPos(gridX, gridY);
        }

        public Part GetPartAtIndex(int gridX, int gridY)
        {
            return levelGrid[gridX, gridY];
        }
    }
}
