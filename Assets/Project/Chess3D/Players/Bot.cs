using Assets.Project.ChessEngine;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Project.Chess3D
{
    public class Bot : IPlayer
    {
        public string Id { get; set; }
        public int Depth { get; set; }

        public string LastSearchResult { get; set; }
        private readonly GameController GameController;

        public Bot(GameController gc, string id, int depth)
        {
            GameController = gc;
            Id = id;
            Depth = depth;
        }

        public Task<Move> CalculateNextMove()
        {
            var task = Task.Run(() =>
            {
                SearchInfo searchInfo = new SearchInfo
                {
                    DepthLimit = Depth
                };
                LastSearchResult = GameController.Board.SearchPosition(searchInfo);
                return GameController.Board.PvMoves[0];
            });
            GameController.UiController.ShowInputInfoText(Id + " (Bot) is calculating.");
            return task;
        }

        public Task SelectPiece()
        {
            var task = Task.Run(() => {} );
            return task;
        }

        public Task DoMove()
        {
            var task = Task.Run(() => {} );
            return task;
        }
    }
}