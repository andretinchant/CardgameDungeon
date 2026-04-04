using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CardgameDungeon.Unity.Core
{
    /// <summary>
    /// Static helper for scene loading, supporting synchronous and async loading with callbacks.
    /// </summary>
    public static class SceneLoader
    {
        public static bool IsLoading { get; private set; }
        public static float Progress { get; private set; }
        public static string TargetScene { get; private set; }

        /// <summary>
        /// Load a scene synchronously by name.
        /// </summary>
        public static void LoadScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("[SceneLoader] Scene name cannot be null or empty.");
                return;
            }

            SceneManager.LoadScene(sceneName);
            IsLoading = false;
            Progress = 1f;
            TargetScene = sceneName;
        }

        /// <summary>
        /// Load a scene asynchronously with an optional callback when complete.
        /// Requires a MonoBehaviour host to run the coroutine (uses GameManager.Instance).
        /// Optionally shows a loading screen scene during the load.
        /// </summary>
        public static void LoadSceneAsync(string sceneName, Action onComplete = null)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("[SceneLoader] Scene name cannot be null or empty.");
                return;
            }

            var host = GameManager.Instance;
            if (host == null)
            {
                Debug.LogError("[SceneLoader] GameManager.Instance is null. Cannot start async load coroutine.");
                return;
            }

            host.StartCoroutine(LoadSceneCoroutine(sceneName, null, onComplete));
        }

        /// <summary>
        /// Load a scene asynchronously, showing a loading screen scene during the transition.
        /// </summary>
        public static void LoadSceneWithLoadingScreen(string sceneName, string loadingScreenScene, Action onComplete = null)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("[SceneLoader] Scene name cannot be null or empty.");
                return;
            }

            var host = GameManager.Instance;
            if (host == null)
            {
                Debug.LogError("[SceneLoader] GameManager.Instance is null. Cannot start async load coroutine.");
                return;
            }

            host.StartCoroutine(LoadSceneCoroutine(sceneName, loadingScreenScene, onComplete));
        }

        private static IEnumerator LoadSceneCoroutine(string sceneName, string loadingScreenScene, Action onComplete)
        {
            IsLoading = true;
            Progress = 0f;
            TargetScene = sceneName;

            if (!string.IsNullOrEmpty(loadingScreenScene))
            {
                SceneManager.LoadScene(loadingScreenScene);
                yield return null;
            }

            AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName);
            if (asyncOp == null)
            {
                Debug.LogError($"[SceneLoader] Failed to start async load for scene: {sceneName}");
                yield break;
            }

            asyncOp.allowSceneActivation = true;

            while (!asyncOp.isDone)
            {
                Progress = Mathf.Clamp01(asyncOp.progress / 0.9f);
                yield return null;
            }

            Progress = 1f;
            IsLoading = false;
            onComplete?.Invoke();
        }
    }
}
