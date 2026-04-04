using UnityEngine;
using CardgameDungeon.Unity.Core;

namespace CardgameDungeon.Unity.UI
{
    public class AuthenticatedScreenGuard : MonoBehaviour
    {
        [SerializeField] private string loginSceneName = "Login";
        [SerializeField] private bool useLoadingTransition = true;

        private void Start()
        {
            if (GameManager.Instance.IsAuthenticated)
                return;

            Debug.LogWarning("[AuthenticatedScreenGuard] No auth session. Redirecting to login.");
            if (useLoadingTransition)
                GameManager.Instance.GoToSceneWithLoading(loginSceneName);
            else
                GameManager.Instance.GoToScene(loginSceneName);
        }
    }
}
