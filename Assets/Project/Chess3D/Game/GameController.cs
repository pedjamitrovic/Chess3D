using Assets.Project.Chess3D.Pieces;
using Assets.Project.ChessEngine;
using Assets.Project.ChessEngine.Pieces;
using UnityEngine;
using Chess3D.Dependency;
using Assets.Project.ChessEngine.Exceptions;
using UnityEngine.SceneManagement;
using System.Threading;
using System.Threading.Tasks;

namespace Assets.Project.Chess3D
{
    public class GameController : MonoBehaviour
    {
        public EventManager EventManager;
        public Spawner Spawner;
        public Visualizer Visualizer;
        public GameUiController UiController;

        public Board Board { get; private set; }
        public Piece SelectedPiece { get; set; }

        public IPlayer[] Players { get; private set; }
        public IPlayer OnTurn { get; private set; }

        void Start()
        {
            Board = SceneLoading.Context.Resolve<Board>(SceneLoading.Parameters.Board);

            Players = new IPlayer[2];

            var wc = SceneLoading.Context.Resolve<string>(SceneLoading.Parameters.WhiteChoice);
            var bc = SceneLoading.Context.Resolve<string>(SceneLoading.Parameters.BlackChoice);
            var wd = SceneLoading.Context.Resolve<int>(SceneLoading.Parameters.WhiteDepth);
            var bd = SceneLoading.Context.Resolve<int>(SceneLoading.Parameters.BlackDepth);

            if (wc.Equals("Human"))
            {
                OnTurn = Players[0] = new Human(this, "White player");
            }
            else
            {
                OnTurn = Players[0] = new Bot(this, "White player", wd);
            }

            if (bc.Equals("Human"))
            {
                Players[1] = new Human(this, "Black player");
            }
            else
            {
                Players[1] = new Bot(this, "Black player", bd);
            }

            SetupPieces();
            StartGame();
        }

        private void SetupPieces()
        {
            foreach (Piece piece in Board.Pieces)
            {
                if (piece == null || piece is OffLimits) continue;
                Spawner.SpawnPiece(piece);
            }
        }

        private async void StartGame()
        {
            while (true)
            {
                OnTurn = Players[(int)Board.OnTurn];
                if (NoPossibleMoves()) break;

                Move move = await OnTurn.CalculateNextMove();

                if (OnTurn is Bot)
                {
                    Bot bot = OnTurn as Bot;
                    UiController.ShowSearchInfoText(bot.LastSearchResult);
                    SelectPiece((int)move.FromSq);
                    DoMove((int)move.ToSq);
                }
                else
                {
                    await OnTurn.SelectPiece();
                    if (SelectedPiece == null) continue;
                    await OnTurn.DoMove();
                }
            }
            EndGame();
        }

        private void EndGame()
        {
            IPlayer winner = Players[(int)Board.OnTurn ^ 1];
            UiController.EndGame(winner.Id + " wins.");
            EventManager.BlockEvents();
        }

        private bool NoPossibleMoves()
        {
            MoveList moveList = Board.GenerateAllMoves();
            foreach (Move move in moveList)
            {
                if (Board.DoMove(move))
                {
                    Board.UndoMove();
                    return false;
                }
            }
            return true;
        }

        private void ReleaseHumanSemaphore()
        {
            Human human = OnTurn as Human;
            if (human != null)
            {
                if (human.Semaphore.CurrentCount == 0) human.Semaphore.Release();
            }
        }

        public void SelectPiece(int sq120)
        {
            if (Board.Pieces[sq120].Color != Board.OnTurn)
            {
                UiController.ShowErrorText("You must select piece of your color.");
                return;
            }
            if (SelectedPiece != null) Visualizer.RemoveHighlightFromPiece(SelectedPiece);
            if (Board.Pieces[sq120] == null) return;
            SelectedPiece = Board.Pieces[sq120];
            Visualizer.HighlightPiece(SelectedPiece);
            ReleaseHumanSemaphore();
            UiController.HideErrorText();
        }

        public void DoMove(int sq120)
        {
            if (SelectedPiece.Square == (Square)sq120)
            {
                Visualizer.RemoveHighlightFromPiece(SelectedPiece);
                SelectedPiece = null;
                ReleaseHumanSemaphore();
                return;
            }

            Move foundMove = null;
            MoveList moveList = Board.GenerateAllMoves();
            foreach (Move move in moveList)
            {
                if (move.FromSq == SelectedPiece.Square && move.ToSq == (Square)sq120)
                {
                    foundMove = move;
                    break;
                }
            }

            Visualizer.RemoveHighlightFromPiece(SelectedPiece);
            var IllegalMovePiece = SelectedPiece;
            SelectedPiece = null;

            if (Board.MoveExists(foundMove))
            {
                Spawner.DoMove(foundMove);
                Board.DoMove(foundMove);
                if (foundMove.PromotedPiece.HasValue)
                {
                    Spawner.SpawnPiece(Board.Pieces[(int)foundMove.ToSq]);
                }
            }
            else
            {
                UiController.ShowErrorText("Illegal move attempted: (" + IllegalMovePiece.Label + ") " + IllegalMovePiece.Square.GetLabel() + " -> " + ((Square)sq120).GetLabel());
            }

            ReleaseHumanSemaphore();
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
