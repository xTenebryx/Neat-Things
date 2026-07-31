using GadgetCore.API;
using HarmonyLib;
using static NeatThings.Gadgets.SwingMeleeAnywhere;

namespace NeatThings.Patches.SwingMeleeAnywhere
{
    [HarmonyPatch(typeof(PlayerScript))]
    [HarmonyPatch(nameof(PlayerScript.Awake))]
    [HarmonyGadget(nameof(SwingMeleeAnywhere))]
    public static class Patch_PlayerScript_Awake
    {

        /*[HarmonyPrefix]
        public static bool Prefix(PlayerScript __instance)
        {
            // Add code to run before `MethodName` is called.
            return true; // Return false to prevent the vanilla method from running.
        }*/

        [HarmonyPostfix]
        public static void Postfix(PlayerScript __instance)
        {
            attackCube1BaseRotation = __instance.attackCube.transform.localRotation;
            attackCube2BaseRotation = __instance.attackCube2.transform.localRotation;
            attackCube3BaseRotation = __instance.attackCube3.transform.localRotation;
        }
    }
}