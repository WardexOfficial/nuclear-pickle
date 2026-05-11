
namespace Content.Client._Nuclear.Lobby;

/// <summary>
/// Clothing display mode in the character editor.
/// </summary>
public enum ClothingDisplayMode : byte
{
    /// <summary>Show all clothing.</summary>
    ShowAll = 0,

    /// <summary>Show underwear only (undershirt, underpants, socks).</summary>
    ShowUnderwearOnly = 1,

    /// <summary>Hide all.</summary>
    HideAll = 2,
}