using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CardgameDungeon.Unity.Core;

namespace CardgameDungeon.Unity.UI
{
    public class LoadingScreenUI : MonoBehaviour
    {
        [SerializeField] private Slider progressBar;
        [SerializeField] private TMP_Text progressText;
        [SerializeField] private TMP_Text targetSceneText;

        private void Update()
        {
            var progress = SceneLoader.Progress;

            if (progressBar != null)
                progressBar.value = progress;

            if (progressText != null)
                progressText.text = $"Loading {(progress * 100f):0}%";

            if (targetSceneText != null)
                targetSceneText.text = string.IsNullOrWhiteSpace(SceneLoader.TargetScene)
                    ? "Preparing..."
                    : $"Entering {SceneLoader.TargetScene}";
        }
    }
}
