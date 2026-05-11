using Robust.Shared.GameStates;

namespace Content.Shared._Nuclear.Clothing.Components;

[RegisterComponent, NetworkedComponent]
public sealed partial class NudityCheckComponent : Component
{
    /// <summary>Whether this clothing covers the chest.</summary>
    [DataField]
    public bool CoversChest = false;

    /// <summary>Whether this clothing covers the groin.</summary>
    [DataField]
    public bool CoversGroin = false;

    /// <summary>Whether this clothing hides socks visually.</summary>
    [DataField]
    public bool CoversSocks = false;
}