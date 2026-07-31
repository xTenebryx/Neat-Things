/*using GadgetCore;
using GadgetCore.API;
using GadgetCore.Util;
using HarmonyLib;
using NeatThings.Gadgets;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using static NeatThings.Gadgets.MoreCharacterSlotsAndInfo;

namespace NeatThings.Patches.MoreCharacterSlotsPatches
{
    [HarmonyAfter("GadgetCore.core")]
    [HarmonyPatch(typeof(Menuu))]
    [HarmonyPatch(nameof(Menuu.Update))]
    [HarmonyGadget(nameof(MoreCharacterSlotsAndInfo))]
    internal static class Patch_Menuu_Update // Responsible for clickable buttons
    {
        [HarmonyOverrides]
        [HarmonyPrefix]
        public static bool Prefix(Menuu __instance, ref Ray ___ray, ref RaycastHit ___hit)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                MCSLogger.LogConsole(nameof(Patch_Menuu_Update) + "_" + nameof(Prefix) + ": Started analysing for raycast.");

                ___ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(___ray, out ___hit, 10f))
                {
                    switch (___hit.transform.gameObject.name)
                    {
                        case "bCharacterPageNext":
                            __instance.GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Au/confirm"), Menuu.soundLevel / 10f);
                            CharacterSlotPagesManager.ChangePage(true);
                            break;
                        case "bCharacterPagePrevious":
                            __instance.GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Au/confirm"), Menuu.soundLevel / 10f);
                            CharacterSlotPagesManager.ChangePage(false);
                            break;
                    }
                }
            }
            return true;
        }

        /*[HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var p = TranspilerHelper.CreateProcessor(instructions, generator);

            var allegianceUpRef = p.FindRefByInsns(new[]
            {
                new CodeInstruction(OpCodes.Ldsfld, "System.Int32 curAllegiance"),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Add),
                new CodeInstruction(OpCodes.Stsfld, "System.Int32 curAllegiance"),
                new CodeInstruction(OpCodes.Ldsfld, "System.Int32 curAllegiance"),
                new CodeInstruction(OpCodes.Ldc_I4_3),
                new CodeInstruction(OpCodes.Ble),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Stsfld, "System.Int32 curAllegiance"),
            });
            p.InjectInsn(allegianceUpRef, new CodeInstruction(OpCodes.Ldc_I4_1));
            p.InjectHook(allegianceUpRef, typeof(AllegianceRegistry).GetMethod("CycleAllegianceSelection"));
            p.RemoveInsns(allegianceUpRef, 9);

            var allegianceDownRef = p.FindRefByInsns(new[]
            {
                new CodeInstruction(OpCodes.Ldsfld, "System.Int32 curAllegiance"),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Sub),
                new CodeInstruction(OpCodes.Stsfld, "System.Int32 curAllegiance"),
                new CodeInstruction(OpCodes.Ldsfld, "System.Int32 curAllegiance"),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Bge),
                new CodeInstruction(OpCodes.Ldc_I4_3),
                new CodeInstruction(OpCodes.Stsfld, "System.Int32 curAllegiance"),
            });
            p.InjectInsn(allegianceDownRef, new CodeInstruction(OpCodes.Ldc_I4_0));
            p.InjectHook(allegianceDownRef, typeof(AllegianceRegistry).GetMethod("CycleAllegianceSelection"));
            p.RemoveInsns(allegianceDownRef, 9);

            return p.GetInstructions();
        }
    }
}*/