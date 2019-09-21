using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Project.Chess3D
{
    public class Human : IPlayer
    {
        public string Id { get; set; }
        private readonly GameUiController uiController;

        public Human(string id)
        {
            Id = id;
            uiController = GameObject.FindGameObjectWithTag("UiController").GetComponent<GameUiController>();
        }

        public Task CalculateNextMove()
        {
            var task = Task.Run(() => { });
            return task;
        }

        public Task SelectFigure(SemaphoreSlim semaphore)
        {
            var task = Task.Run(() => { semaphore.Wait(); });
            return task;
        }

        public Task SelectSquare(SemaphoreSlim semaphore)
        {
            var task = Task.Run(() => { semaphore.Wait(); });
            return task;
        }
    }
}