using Robust.Shared.GameStates;

namespace Content.Shared._Nuclear.Ghosts;

/// <summary>
/// Allows an entity (typically an admin ghost) to ignore inventory block/hide lists.
/// When IgnoreBlock is true, blocked slots can still be interacted with.
/// When ShowAllItems is true, hidden slots are revealed in the stripping menu.
/// </summary>
[RegisterComponent, AutoGenerateComponentState, NetworkedComponent]
public sealed partial class IgnoreInventoryBlockComponent : Component
{
    /// <summary>
    /// If true, blocks from BlockList are ignored — items can be equipped/unequipped.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool IgnoreBlock = true;

    /// <summary>
    /// If true, hidden slots are shown in the stripping menu.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool ShowAllItems = true;
}