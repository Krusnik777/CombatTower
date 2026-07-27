using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Loading
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private Slider m_progressBar;
        [SerializeField] private TMP_Text m_statusText;

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetProgress(float value) => m_progressBar.value = value;
        public void SetStatus(string status) => m_statusText.text = status;
    }
}
