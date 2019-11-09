using System;
using System.ComponentModel.DataAnnotations;

namespace SweepMiner
{
    class Tile
    {
        [Range(0,8)]
        public int surroundingMines = -1;
        public bool mine = false;
        public bool uncovered = false;
        public bool flagged = false;
    }
}
