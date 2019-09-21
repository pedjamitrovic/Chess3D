using System;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Chess3D.Dependency;
using Assets.Project.ChessEngine;
using Assets.Project.ChessEngine.Exceptions;

public class MenuUiController : MonoBehaviour
{
    public Dropdown WhiteChoice;
    public Dropdown BlackChoice;
    public Dropdown WhiteDepth;
    public Dropdown BlackDepth;
    public InputField Fen;
    public Button StartButton;
    public Button ExitButton;
    public Text ErrorText;
    
    private readonly string StartFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

    void Start()
    {
        ErrorText.text = string.Empty;

        WhiteChoice.onValueChanged.AddListener(OnWhiteChoiceChanged);
        BlackChoice.onValueChanged.AddListener(OnBlackChoiceChanged);

        WhiteDepth.transform.localScale = new Vector3(1, 0, 1);
        BlackDepth.transform.localScale = new Vector3(1, 0, 1);

        StartButton.onClick.AddListener(OnStartClicked);
        ExitButton.onClick.AddListener(OnExitClicked);
    }

    private void OnWhiteChoiceChanged(int index)
    {
        WhiteDepth.transform.localScale = new Vector3(1, index, 1);
    }
    private void OnBlackChoiceChanged(int index)
    {
        BlackDepth.transform.localScale = new Vector3(1, index, 1);
    }

    private void OnStartClicked()
    {
        SceneLoading.Context.Clear();

        SceneLoading.Context.Inject(SceneLoading.Parameters.WhiteChoice, WhiteChoice.options[WhiteChoice.value].text);
        SceneLoading.Context.Inject(SceneLoading.Parameters.BlackChoice, BlackChoice.options[BlackChoice.value].text);
        SceneLoading.Context.Inject(SceneLoading.Parameters.WhiteDepth, Convert.ToInt32(WhiteDepth.options[WhiteDepth.value].text));
        SceneLoading.Context.Inject(SceneLoading.Parameters.BlackDepth, Convert.ToInt32(BlackDepth.options[BlackDepth.value].text));
        SceneLoading.Context.Inject(SceneLoading.Parameters.Fen, Fen.text);

        try
        {
            Board board;
            if (!string.IsNullOrEmpty(Fen.text)) board = new Board(Fen.text);
            else board = new Board(StartFen);
            SceneLoading.Context.Inject(SceneLoading.Parameters.Board, board);
            SceneManager.LoadScene("Game");
        }
        catch (Exception)
        {
            ErrorText.text = "Invalid FEN format. Try again.";
        }
    }
    private void OnExitClicked()
    {
        Application.Quit();
    }
}
