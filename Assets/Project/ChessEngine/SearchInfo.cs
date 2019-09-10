using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Assets.Project.ChessEngine
{
    public class SearchInfo
    {
        public Stopwatch Stopwatch { get; set; }
        public int DepthLimit { get; set; }
        public int TimeLimit { get; set; }
        public bool Infinite { get; set; }
        public long NodesVisited { get; set; }
        public decimal Fh { get; set; }
        public decimal Fhf { get; set; }

        public bool Quit { get; set; }
        public bool Stopped { get; set; }

        public SearchInfo()
        {
            Stopwatch = new Stopwatch();
        }
    }
}
