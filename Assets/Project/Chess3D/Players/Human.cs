using Assets.Project.ChessEngine;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Project.Chess3D
{
    public class Human : IPlayer
    {
        public string Id { get; set; }
        public GameController GameController;

        public SemaphoreSlim Semaphore { get; private set; }

        public Human(GameController gc, string id)
        {
            Id = id;
            GameController = gc;
            Semaphore = new SemaphoreSlim(0, 1);
        }

        public Task<Move> CalculateNextMove()
        {
            var task = Task.Run(() => { return new Move(); });
            return task;
        }

        public Task SelectPiece()
        {
            var task = Task.Run(() => { Semaphore.Wait(); });
            GameController.UiController.ShowInputInfoText(Id + " (Human), select piece to move.");
            return task;
        }

        public Task DoMove()
        {
            var task = Task.Run(() => { Semaphore.Wait(); });
            GameController.UiController.ShowInputInfoText(Id + " (Human), select square to move.");
            return task;
        }
    }
}