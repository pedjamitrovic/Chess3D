using System.Threading;
using System.Threading.Tasks;

namespace Assets.Project.Chess3D
{
    public interface IPlayer
    {
        string Id { get; set; }
        Task CalculateNextMove();
        Task SelectFigure(SemaphoreSlim semaphore);
        Task SelectSquare(SemaphoreSlim semaphore);
    }
}
