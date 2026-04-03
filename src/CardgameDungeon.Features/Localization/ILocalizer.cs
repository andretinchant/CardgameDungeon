namespace CardgameDungeon.Features.Localization;

/// <summary>
/// Resolves localized strings by key for the current request locale.
/// </summary>
public interface ILocalizer
{
    string this[string key] { get; }
    string this[string key, params object[] args] { get; }
    string CurrentLocale { get; }
}
