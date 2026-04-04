# UI Support Screens Setup

This update adds a complete support-flow foundation:

- `Bootstrap` scene: `BootstrapUI`
- `Login` scene: `LoginUI`
- `Loading` scene: `LoadingScreenUI`
- `Profile` scene: `ProfileUI`
- `Settings` scene: `SettingsUI`
- Protected scenes: `AuthenticatedScreenGuard`

## Expected Scene Names

- `Bootstrap`
- `Login`
- `Loading`
- `MainMenu`
- `Collection`
- `DeckBuilder`
- `Marketplace`
- `BoosterShop`
- `Profile`
- `Settings`
- `Match`

## Required Bindings

### `BootstrapUI`
- `statusText`

### `LoginUI`
- `usernameInput`
- `emailInput`
- `passwordInput`
- `loginButton`
- `registerButton`
- `continueButton` (optional)
- `statusText`

### `LoadingScreenUI`
- `progressBar` (optional)
- `progressText` (optional)
- `targetSceneText` (optional)

### `MainMenuUI`
- Existing buttons plus optional:
  - `boosterShopButton`
  - `profileButton`
  - `settingsButton`
  - `logoutButton`

### `ProfileUI`
- `usernameText`
- `playerIdText`
- `statusText`
- `logoutButton`
- `backButton`

### `SettingsUI`
- `backButton`

### `AuthenticatedScreenGuard`
Attach to all scenes that require auth:
- `MainMenu`, `Collection`, `DeckBuilder`, `Marketplace`, `BoosterShop`, `Profile`, `Settings`, `Match`

## Runtime Flow

1. Start in `Bootstrap`.
2. If saved tokens exist, `BootstrapUI` calls refresh.
3. Success => goes to `MainMenu` via `Loading`.
4. Failure or no tokens => goes to `Login`.
5. `LoginUI` login/register stores tokens in `GameManager` and continues to `MainMenu` via `Loading`.
