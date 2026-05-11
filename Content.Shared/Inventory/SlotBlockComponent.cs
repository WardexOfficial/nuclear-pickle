using Robust.Shared.GameStates;

namespace Content.Shared.Inventory;

/// <summary>
/// Used to prevent items from being unequipped and equipped from slots that are listed in <see cref="Slots"/>.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, Access(typeof(SlotBlockSystem))]
public sealed partial class SlotBlockComponent : Component
{
    // Nuclear-Start
    /// <summary>
    /// Slots that this entity should block from both equipping/unequipping
    /// AND hide from the stripping menu.
    /// </summary>
    [DataField, AutoNetworkedField]
    public HashSet<SlotFlags> BlockList = new();

    /// <summary>
    /// Slots that this entity should only hide from the stripping menu,
    /// but still allow equipping/unequipping.
    /// </summary>
    [DataField, AutoNetworkedField]
    public HashSet<SlotFlags> HideList = new();
    // Nuclear-End
    
    // Nuclear-Edit: Original Slots field is retained for backward compatibility,
    // but marked as deprecated. Use BlockList instead.
    /// <summary>
    /// Slots that this entity should block. Deprecated: use BlockList instead.
    /// </summary>
    [DataField, AutoNetworkedField]
    public SlotFlags Slots = SlotFlags.NONE;
}
