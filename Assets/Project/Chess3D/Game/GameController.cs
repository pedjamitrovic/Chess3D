using Assets.Project.Chess3D.Pieces;
using Assets.Project.ChessEngine;
using Assets.Project.ChessEngine.Pieces;
using UnityEngine;
using Chess3D.Dependency;
using Assets.Project.ChessEngine.Exceptions;
using UnityEngine.SceneManagement;
using System.Threading;

namespace Assets.Project.Chess3D
{
    public class GameController : MonoBehaviour
    {
        public EventManager eventManager;
        public Spawner spawner;
        public Visualizer visualizer;
        public GameUiController uiController;

        public Board Board { get; private set; }
        public Piece SelectedPiece { get; private set; }

        public SemaphoreSlim semaphore;
        public IPlayer[] players;

        void Start()
        {
            var wc = SceneLoading.Context.Resolve<string>(SceneLoading.Parameters.WhiteChoice);
            var bc = SceneLoading.Context.Resolve<string>(SceneLoading.Parameters.BlackChoice);
            var wd = SceneLoading.Context.Resolve<int>(SceneLoading.Parameters.WhiteDepth);
            var bd = SceneLoading.Context.Resolve<int>(SceneLoading.Parameters.BlackDepth);

            Board = SceneLoading.Context.Resolve<Board>(SceneLoading.Parameters.Board);
            
            SetupPieces();
            StartGame();
        }

        public void SetupPieces()
        {
            foreach (Piece piece in Board.Pieces)
            {
                if (piece == null || piece is OffLimits) continue;
                spawner.SpawnPiece(piece);
            }
        }

        public void StartGame()
        {

        }

        public void SelectPiece(int sq120)
        {
            if (Board.Pieces[sq120].Color != Board.OnTurn) return;
            if (SelectedPiece != null) visualizer.RemoveHighlightFromPiece(SelectedPiece);
            if (Board.Pieces[sq120] == null) return;
            SelectedPiece = Board.Pieces[sq120];
            visualizer.HighlightPiece(SelectedPiece);
        }

        public void DoMove(int sq120)
        {
            if (SelectedPiece.Square == (Square)sq120)
            {
                visualizer.RemoveHighlightFromPiece(SelectedPiece);
                SelectedPiece = null;
                return;
            }

            Move legalMove = null;
            MoveList moveList = Board.GenerateAllMoves();
            foreach (Move move in moveList)
            {
                if (move.FromSq == SelectedPiece.Square && move.ToSq == (Square)sq120)
                {
                    legalMove = move;
                    break;
                }
            }

            visualizer.RemoveHighlightFromPiece(SelectedPiece);
            SelectedPiece = null;

            if (Board.MoveExists(legalMove))
            {
                spawner.DoMove(legalMove);
                Board.DoMove(legalMove);
                if (legalMove.PromotedPiece.HasValue)
                {
                    spawner.SpawnPiece(Board.Pieces[(int)legalMove.ToSq]);
                }
            }
        }

        public string GetCellString(int cellNumber)
        {
            int j = cellNumber % 8;
            int i = cellNumber / 8;
            return char.ConvertFromUtf32(j + 65) + "" + (i + 1);
        }

        public Vector3 ToWorldPoint(int cellNumber)
        {
            int j = cellNumber % 8;
            int i = cellNumber / 8;
            return new Vector3(i * -4 + 14, 1, j * 4 - 14);
        }

        public bool IsValidCell(int cellNumber)
        {
            return cellNumber >= 0 && cellNumber <= 63;
        }
    }
}
