using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CardgameDungeon.Unity.Core;
using CardgameDungeon.Unity.Network;

namespace CardgameDungeon.Unity.UI
{
    public class LoginUI : MonoBehaviour
    {
        [Header("Inputs")]
        [SerializeField] private TMP_InputField usernameInput;
        [SerializeField] private TMP_InputField emailInput;
        [SerializeField] private TMP_InputField passwordInput;

        [Header("Actions")]
        [SerializeField] private Button loginButton;
        [SerializeField] private Button registerButton;
        [SerializeField] private Button continueButton;

        [Header("Navigation")]
        [SerializeField] private string mainMenuSceneName = "MainMenu";

        [Header("Status")]
        [SerializeField] private TMP_Text statusText;

        private void OnEnable()
        {
            if (loginButton != null) loginButton.onClick.AddListener(OnLoginClicked);
            if (registerButton != null) registerButton.onClick.AddListener(OnRegisterClicked);
            if (continueButton != null) continueButton.onClick.AddListener(OnContinueClicked);

            if (continueButton != null)
                continueButton.gameObject.SetActive(GameManager.Instance.IsAuthenticated);

            SetStatus("Enter credentials to continue.");
        }

        private void OnDisable()
        {
            if (loginButton != null) loginButton.onClick.RemoveListener(OnLoginClicked);
            if (registerButton != null) registerButton.onClick.RemoveListener(OnRegisterClicked);
            if (continueButton != null) continueButton.onClick.RemoveListener(OnContinueClicked);
        }

        private void OnContinueClicked()
        {
            GameManager.Instance.GoToSceneWithLoading(mainMenuSceneName);
        }

        private void OnLoginClicked()
        {
            var username = usernameInput?.text?.Trim() ?? string.Empty;
            var password = passwordInput?.text ?? string.Empty;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                SetStatus("Username and password are required.");
                return;
            }

            SetBusy(true);
            SetStatus("Signing in...");

            var request = new LoginRequest
            {
                username = username,
                password = password
            };

            GameManager.Instance.Api.Login(this, request, auth =>
            {
                HandleAuthSuccess(auth, "Login successful.");
            }, error =>
            {
                SetBusy(false);
                SetStatus($"Login failed: {error}");
            });
        }

        private void OnRegisterClicked()
        {
            var username = usernameInput?.text?.Trim() ?? string.Empty;
            var email = emailInput?.text?.Trim() ?? string.Empty;
            var password = passwordInput?.text ?? string.Empty;

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                SetStatus("Username, email and password are required for registration.");
                return;
            }

            SetBusy(true);
            SetStatus("Creating account...");

            var request = new RegisterRequest
            {
                username = username,
                email = email,
                password = password
            };

            GameManager.Instance.Api.Register(this, request, auth =>
            {
                HandleAuthSuccess(auth, "Account created.");
            }, error =>
            {
                SetBusy(false);
                SetStatus($"Registration failed: {error}");
            });
        }

        private void HandleAuthSuccess(AuthResponse auth, string successMessage)
        {
            try
            {
                GameManager.Instance.ApplyAuth(auth);
                SetStatus(successMessage);
                GameManager.Instance.GoToSceneWithLoading(mainMenuSceneName);
            }
            catch (Exception ex)
            {
                SetBusy(false);
                SetStatus($"Auth response invalid: {ex.Message}");
            }
        }

        private void SetBusy(bool busy)
        {
            if (loginButton != null) loginButton.interactable = !busy;
            if (registerButton != null) registerButton.interactable = !busy;
            if (continueButton != null) continueButton.interactable = !busy;
        }

        private void SetStatus(string message)
        {
            if (statusText != null)
                statusText.text = message;
            Debug.Log($"[LoginUI] {message}");
        }
    }
}
