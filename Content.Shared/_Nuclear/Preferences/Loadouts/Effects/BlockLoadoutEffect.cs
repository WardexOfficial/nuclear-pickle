using System.Diagnostics.CodeAnalysis;
using Content.Shared.Preferences;
using Content.Shared.Preferences.Loadouts;
using Content.Shared.Preferences.Loadouts.Effects;
using Robust.Shared.Player;
using Robust.Shared.Utility;

namespace Content.Shared._Nuclear.Preferences.Loadouts.Effects;

/// <summary>
/// Always blocks the loadout from being selected. Used for category headers.
/// </summary>
public sealed partial class BlockLoadoutEffect : LoadoutEffect
{
    public override bool Validate(
        HumanoidCharacterProfile profile,
        RoleLoadout loadout,
        LoadoutPrototype proto,
        ICommonSession? session,
        IDependencyCollection collection,
        [NotNullWhen(false)] out FormattedMessage? reason)
    {
        reason = FormattedMessage.FromUnformatted(Loc.GetString("loadout-group-blocked"));
        return false;
    }
}