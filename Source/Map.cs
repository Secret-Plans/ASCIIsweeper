using System;

namespace SweepMiner
{
    class Map
    {
        public Tile[,] tiles;
        public int width;
        public int height;
        public int mines;
        public bool AllMinesCleared
        {
            get
            {
                int coveredNonMines = 0;
                int flaggedMines = 0;
                for(int x = 0; x != width; x++)
                {
                    for(int y = 0; y != height; y++)
                    {
                        if (!tiles[x, y].mine && !tiles[x,y].uncovered)
                        {
                            coveredNonMines++;
                        }
                        else if(tiles[x,y].mine && tiles[x, y].flagged)
                        {
                            flaggedMines++;
                        }
                    }
                }
                if(coveredNonMines == 0 || flaggedMines >= mines) { return true; }
                return false;
            }
        }

        public Map(int width, int height, int mines)
        {
            this.width = width;
            this.height = height;
            this.mines = mines;

            tiles = new Tile[width, height];

            for(int x = 0; x != width; x++)
            {
                for(int y = 0; y != height; y++)
                {
                    tiles[x, y] = new Tile();
                }
            }
        }

        public void Generate(int clickX, int clickY)
        {
            Random rn = new Random(DateTime.Now.GetHashCode());
            int mineCount = 0;
            while (mineCount < mines)
            {
                int x = rn.Next(0, width);
                int y = rn.Next(0, height);
                
                if (!tiles[x, y].mine && !(x == clickX && y == clickY))
                {
                    if(width * height > 100 && InRange(clickX-1, clickX+1, x) && InRange(clickY-1, clickY+1, y))
                    {
                        continue;
                    }
                    if (x == clickX && y == clickY)
                    {
                        continue;
                    }
                    tiles[x, y].mine = true;
                    mineCount += 1;
                }
            }
        }

        public bool InRange(int lower, int upper, int value)
        {
            return value >= lower && value <= upper;
        }

        public void SetNumbers()
        {
            for(int x = 0; x != width; x++)
            {
                for(int y = 0; y != height; y++)
                {
                    if (!tiles[x, y].mine)
                    {
                        int num = CheckSurroundingTiles(x, y);
                        tiles[x, y].surroundingMines = num;
                    }
                }
            }
        }

        public int CheckSurroundingTiles(int x, int y)
        {
            int count = 0;

            for(int relativeX = -1; relativeX != 2; relativeX++)
            {
                for(int relativeY = -1; relativeY != 2; relativeY++)
                {
                    int nearX = x + relativeX;
                    int nearY = y + relativeY;

                    if(nearX < 0 || nearX > width - 1 || nearY < 0 || nearY > height - 1)
                    {
                        continue;
                    }
                    if (tiles[nearX, nearY].mine)
                    {
                        count++;
                    }
                }
            }

            return count;
        }
    }
}
