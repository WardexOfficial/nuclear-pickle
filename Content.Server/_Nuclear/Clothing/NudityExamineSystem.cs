using System.Linq;
using Content.Shared._Nuclear.Clothing.Components;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Clothing.Components;
using Content.Shared.Humanoid;
using Robust.Shared.GameObjects;

namespace Content.Server._Nuclear.Clothing;

public sealed class NudityExamineSystem : EntitySystem
{
    [Dependency] private readonly InventorySystem _inventory = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<NudityExaminableComponent, DidEquipEvent>(OnClothingChanged);
        SubscribeLocalEvent<NudityExaminableComponent, DidUnequipEvent>(OnClothingChanged);
        SubscribeLocalEvent<NudityExaminableComponent, ComponentStartup>(OnStartup);
    }

    private void OnStartup(EntityUid uid, NudityExaminableComponent comp, ComponentStartup args)
    {
        UpdateNudityState(uid, comp);
    }

    private void OnClothingChanged<T>(EntityUid uid, NudityExaminableComponent comp, T args) where T : notnull
    {
        UpdateNudityState(uid, comp);
    }

    private void UpdateNudityState(EntityUid uid, NudityExaminableComponent comp)
    {
        _inventory.TryGetSlotEntity(uid, "jumpsuit", out var jumpsuit);
        _inventory.TryGetSlotEntity(uid, "outerClothing", out var outer);
        _inventory.TryGetSlotEntity(uid, "undershirt", out var undershirt);
        _inventory.TryGetSlotEntity(uid, "underpants", out var underpants);
        _inventory.TryGetSlotEntity(uid, "socks", out var socks);
        _inventory.TryGetSlotEntity(uid, "shoes", out var shoes);

        var hasJumpsuit = jumpsuit != null;
        var hasUndershirt = undershirt != null;
        var hasUnderpants = underpants != null;
        var hasSocks = socks != null;

        // Hide socks under shoes that cover them
        if (shoes != null && TryComp<NudityCheckComponent>(shoes.Value, out var shoesNudity) && shoesNudity.CoversSocks)
            hasSocks = false;

        // Chest: covered by jumpsuit, outer clothing, or undershirt that explicitly covers chest
        comp.ChestCovered = (hasJumpsuit && jumpsuit != null && HasCoverFlag(jumpsuit.Value, coversChest: true))
            || (outer != null && HasCoverFlag(outer.Value, coversChest: true))
            || (hasUndershirt && undershirt != null && HasCoverFlag(undershirt.Value, coversChest: true));

        // Groin: covered by jumpsuit or underpants that explicitly cover groin
        comp.GroinCovered = (hasJumpsuit && jumpsuit != null && HasCoverFlag(jumpsuit.Value, coversGroin: true))
            || (hasUnderpants && underpants != null && HasCoverFlag(underpants.Value, coversGroin: true));

        var isFemale = TryComp<HumanoidAppearanceComponent>(uid, out var appearance) && appearance.Sex == Sex.Female;
        comp.WarnChestExposure = isFemale && !comp.ChestCovered;

        // Determine which nudity message to show
        if (comp.IgnoreNudity)
        {
            // Species that don't understand nudity — just a neutral "nothing worn" message
            comp.NudityMessageId = "examine-nothing-worn";
            comp.NudityMessageItem = null;
            comp.WarnChestExposure = false;
            comp.GroinCovered = true;
        }
        else if (!comp.ChestCovered && !comp.GroinCovered)
        {
            // Check if anything at all is worn (excluding underwear slots)
            var hasAnyClothing = _inventory.TryGetSlots(uid, out var allSlots)
                && allSlots.Any(s =>
                    s.Name is not ("undershirt" or "underpants")
                    && _inventory.TryGetSlotEntity(uid, s.Name, out _));

            if (!hasAnyClothing)
            {
                // Completely naked — nothing equipped at all
                comp.NudityMessageId = "examine-can-see-nothing";
                comp.NudityMessageItem = null;
            }
            else if (isFemale)
            {
                // Female with some clothing but chest and groin exposed
                comp.NudityMessageId = "examine-chest-groin-exposed";
                comp.NudityMessageItem = null;
            }
            else
            {
                // Male with clothing but groin exposed — warn about groin only
                comp.NudityMessageId = string.Empty;
                comp.NudityMessageItem = null;
                comp.WarnChestExposure = false;
            }
        }
        else
        {
            // Everything covered, or partial exposure handled via WarnChestExposure/GroinCovered
            comp.NudityMessageId = string.Empty;
            comp.NudityMessageItem = null;
        }

        Dirty(uid, comp);
    }

    private bool HasCoverFlag(EntityUid item, bool coversChest = false, bool coversGroin = false)
    {
        return TryComp<NudityCheckComponent>(item, out var nudity)
            && ((coversChest && nudity.CoversChest) || (coversGroin && nudity.CoversGroin));
    }
}