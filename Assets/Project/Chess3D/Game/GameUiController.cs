using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Project.Chess3D
{
    public class GameUiController : MonoBehaviour
    {
        public Text ErrorText;
        public Text InputInfoText;
        public Text SearchInfoText;
        public Button EndButton;

        private void Start()
        {
            EndButton.onClick.AddListener(OnEndClicked);
        }

        public void ShowErrorText(string text)
        {
            ErrorText.text = text;
        }

        public void HideErrorText()
        {
            ErrorText.text = string.Empty;
        }

        public void ShowInputInfoText(string text)
        {
            InputInfoText.text = text;
        }

        public void HideInputInfoText()
        {
            InputInfoText.text = string.Empty;
        }

        public void ShowSearchInfoText(string text)
        {
            SearchInfoText.text = text;
        }

        public void HideSearchInfoText()
        {
            SearchInfoText.text = string.Empty;
        }

        public void EndGame(string winnerText)
        {
            InputInfoText.text = winnerText;
            ErrorText.text = string.Empty;
            SearchInfoText.text = string.Empty;
        }

        public void ClearAll()
        {
            InputInfoText.text = string.Empty;
            ErrorText.text = string.Empty;
        }

        public void OnEndClicked()
        {
            SceneManager.LoadScene("Menu");
        }
    }
}
