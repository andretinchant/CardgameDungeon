using System.Collections;
using TMPro;
using UnityEngine;
using CardgameDungeon.Unity.Core;

namespace CardgameDungeon.Unity.UI
{
    public class BootstrapUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text statusText;
        [SerializeField] private string loginSceneName = "Login";
        [SerializeField] private string mainMenuSceneName = "MainMenu";

        private void Start()
        {
            StartCoroutine(BootstrapRoutine());
        }

        private IEnumerator BootstrapRoutine()
        {
            SetStatus("Initializing session...");

            if (!GameManager.Instance.IsAuthenticated)
            {
                SetStatus("No auth session found.");
                GameManager.Instance.GoToScene(loginSceneName);
                yield break;
            }

            SetStatus("Refreshing credentials...");

            var completed = false;
            var success = false;
            string error = null;

            yield return GameManager.Instance.TryRefreshSession((ok, message) =>
            {
                success = ok;
                error = message;
                completed = true;
            });

            while (!completed)
                yield return null;

            if (success)
            {
                SetStatus("Session restored.");
                GameManager.Instance.GoToSceneWithLoading(mainMenuSceneName);
                yield break;
            }

            SetStatus($"Session expired: {error}");
            GameManager.Instance.ClearAuth();
            GameManager.Instance.GoToScene(loginSceneName);
        }

        private void SetStatus(string message)
        {
            if (statusText != null)
                statusText.text = message;
            Debug.Log($"[BootstrapUI] {message}");
        }
    }
}
