using Assets.Project.ChessEngine;
using System.Threading;
using System.Threading.Tasks;

namespace Assets.Project.Chess3D
{
    public interface IPlayer
    {
        string Id { get; set; }
        Task<Move> CalculateNextMove();
        Task SelectPiece();
        Task DoMove();
    }
}
