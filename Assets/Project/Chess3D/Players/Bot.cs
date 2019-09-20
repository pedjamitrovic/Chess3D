using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Project.Chess3D
{
    public class Bot : IPlayer
    {
        public string Id { get; set; }
        private readonly UiController uiController;

        public Bot()
        {
            Id = "Bot";
            uiController = GameObject.FindGameObjectWithTag("UiController").GetComponent<UiController>();
        }

        public Task CalculateNextMove()
        {
            var task = Task.Run(() => { });
            return task;
        }

        public Task SelectFigure(SemaphoreSlim semaphore)
        {
            var task = Task.Run(() => { semaphore.Wait(); });
            uiController.ShowSelectFigure(this);
            return task;
        }

        public Task SelectSquare(SemaphoreSlim semaphore)
        {
            var task = Task.Run(() => { semaphore.Wait(); });
            uiController.ShowSelectSquare(this);
            return task;
        }
    }
}