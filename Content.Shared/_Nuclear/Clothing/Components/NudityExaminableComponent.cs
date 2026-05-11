using Robust.Shared.GameStates;

namespace Content.Shared._Nuclear.Clothing.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class NudityExaminableComponent : Component
{
    /// <summary>If true, nudity checks are skipped (for species without nudity concept).</summary>
    [DataField, AutoNetworkedField]
    public bool IgnoreNudity = false;
    
    /// <summary>Whether the chest is currently covered.</summary>
    [DataField, AutoNetworkedField]
    public bool ChestCovered = true;

    /// <summary>Whether the groin is currently covered.</summary>
    [DataField, AutoNetworkedField]
    public bool GroinCovered = true;

    /// <summary>If true, only socks are worn and nothing else.</summary>
    [DataField, AutoNetworkedField]
    public bool HasOnlySocks;

    /// <summary>Name of the socks item, if HasOnlySocks is true.</summary>
    [DataField, AutoNetworkedField]
    public string? SocksItemName;

    /// <summary>If true and female, chest exposure warning should be shown.</summary>
    [DataField, AutoNetworkedField]
    public bool WarnChestExposure;

    /// <summary>ID of the main nudity message to display (empty if none).</summary>
    [DataField, AutoNetworkedField]
    public string NudityMessageId = string.Empty;

    /// <summary>Optional item name to pass to the nudity message.</summary>
    [DataField, AutoNetworkedField]
    public string? NudityMessageItem;
}