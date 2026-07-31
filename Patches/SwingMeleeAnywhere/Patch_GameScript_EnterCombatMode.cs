using GadgetCore.API;
using HarmonyLib;
using static MeleeRangePlus.MeleeRangePlus;
using static NeatThings.Gadgets.SwingMeleeAnywhere;

namespace NeatThings.Patches.SwingMeleeAnywhere
{
    [HarmonyPatch(typeof(GameScript))]
    [HarmonyPatch(nameof(GameScript.EnterCombatMode))]
    [HarmonyGadget(nameof(SwingMeleeAnywhere))]
    public static class Patch_GameScript_EnterCombatMode
    {
        [HarmonyPostfix]
        public static void Prefix()
        {
            heldAppearanceParentRotation = HeldAppearanceParent.localRotation;
        }
    }
}
