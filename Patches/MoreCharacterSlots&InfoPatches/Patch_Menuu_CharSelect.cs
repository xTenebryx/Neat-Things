/*using GadgetCore;
using GadgetCore.API;
using GadgetCore.Util;
using HarmonyLib;
using NeatThings.Gadgets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using static PreviewLabs.PlayerPrefs;
using static NeatThings.Gadgets.MoreCharacterSlotsAndInfo;

namespace NeatThings.Patches.MoreCharacterSlotsPatches
{
    [HarmonyPatch]
    [HarmonyGadget(nameof(MoreCharacterSlotsAndInfo))]
    public static class Patch_Menuu_CharSelect // Responsible for page selection and player confirmation depending on opened page
    {
        public static Type IteratorType = typeof(Menuu).GetNestedType("<CharSelect>c__Iterator3", BindingFlags.NonPublic);
        public static FieldInfo PC = IteratorType.GetField("$PC", BindingFlags.NonPublic | BindingFlags.Instance);

        [HarmonyTargetMethod]
        public static MethodBase TargetMethod()
        {
            return IteratorType.GetMethod("MoveNext", BindingFlags.Public | BindingFlags.Instance);
        }

        [HarmonyPrefix]
        public static bool Prefix()
        {
            CharacterSlotPagesManager.EnableOrDisablePageSelectionButtons();
            InstanceTracker.Menuu.buttonHolder[11].transform.Find("bs5").GetComponent<BoxCollider>().size = new Vector3(1.4f, 0.2f, 1f);

            InstanceTracker.Menuu.StartCoroutine(CharSelectModified());
            return false;
        }

        /*[HarmonyPatch]
        [HarmonyGadget(nameof(MoreCharacterSlots))]
        public static class Patch_Menuu_CharSelect
        {
            [HarmonyTargetMethod]
            public static MethodBase TargetMethod()
            {
                return typeof(Menuu).GetNestedType("<CharSelect>c__Iterator3", BindingFlags.NonPublic).GetMethod("MoveNext", BindingFlags.Public | BindingFlags.Instance);
            }

            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
            {
            var processor = TranspilerHelper.CreateProcessor(instructions, generator);
            var ilRefStartLoop = processor.FindRefByInsns(new[] {
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Stloc_1),
                new CodeInstruction(OpCodes.Br)
                });
            var ilRefEndLoop = processor.FindRefByInsns(new[] {
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Ldc_I4_6),
                new CodeInstruction(OpCodes.Blt)
                });

            return processor.Insns;
        }

        private const int _charsPerPage = 9;

        internal static IEnumerator CharSelectModified() // For page selection and extra buttons
        {
            GameObject[] beam = InstanceTracker.Menuu.beam;
            GameObject[] buttonHolders = InstanceTracker.Menuu.buttonHolder;
            TextMesh[] txtCharName = InstanceTracker.Menuu.txtCharName;
            GameObject[] buttonDelete = InstanceTracker.Menuu.buttonDelete;
            GameObject[] charIcon = InstanceTracker.Menuu.charIcon;
            GameObject[] charIcon2 = InstanceTracker.Menuu.charIcon2;
            //GameObject[] charIcon3 = InstanceTracker.Menuu.charIcon3;

            beam[5].transform.localScale = new Vector3(30f, 0f, 1f);
            beam[6].transform.localScale = new Vector3(30f, 0f, 1f);
            beam[7].transform.localScale = new Vector3(30f, 0f, 1f);
            beam[8].transform.localScale = new Vector3(30f, 0f, 1f);
            buttonHolders[6].transform.position = new Vector3(-40f, 0f, 0f);
            buttonHolders[7].transform.position = new Vector3(-40f, 0f, 0f);
            buttonHolders[8].transform.position = new Vector3(-40f, 0f, 0f);
            buttonHolders[9].transform.position = new Vector3(-40f, 0f, 0f);
            buttonHolders[10].transform.position = new Vector3(-40f, 0f, 0f);
            buttonHolders[11].transform.position = new Vector3(-40f, 0f, 0f);
            buttonHolders[12].transform.position = new Vector3(-40f, 0f, 0f);
            buttonHolders[buttonHolders.Length - 2].transform.position = new Vector3(-40f, 0f, 0f); // added modded buttons for characters
            buttonHolders[buttonHolders.Length - 1].transform.position = new Vector3(-40f, 0f, 0f);
            buttonHolders[buttonHolders.Length].transform.position = new Vector3(-40f, 0f, 0f);
            InstanceTracker.Menuu.menuDeleteChar.SetActive(false);

            PopulateCharacterButtons(txtCharName, buttonDelete, charIcon, charIcon2);

            InstanceTracker.Menuu.menuMain.SetActive(false);
            InstanceTracker.Menuu.menuCharSelect.SetActive(true);
            beam[5].GetComponent<Animation>().Play();
            beam[6].GetComponent<Animation>().Play();
            beam[7].GetComponent<Animation>().Play();
            beam[8].GetComponent<Animation>().Play();

            yield return new WaitForSeconds(0.1f);
            buttonHolders[6].GetComponent<Animation>().Play();  // vanilla buttons for characters
            buttonHolders[7].GetComponent<Animation>().Play();
            buttonHolders[8].GetComponent<Animation>().Play();
            yield return new WaitForSeconds(0.1f);
            buttonHolders[9].GetComponent<Animation>().Play();
            buttonHolders[10].GetComponent<Animation>().Play();
            buttonHolders[11].GetComponent<Animation>().Play();
            yield return new WaitForSeconds(0.1f);
            buttonHolders[buttonHolders.Length - 2].GetComponent<Animation>().Play();   // 3 modded buttons for characters
            buttonHolders[buttonHolders.Length - 1].GetComponent<Animation>().Play();
            buttonHolders[buttonHolders.Length].GetComponent<Animation>().Play();
            yield return new WaitForSeconds(0.1f);
            buttonHolders[12].GetComponent<Animation>().Play(); // back
            yield return null;
            yield break;
        }


        public static void PopulateCharacterButtons(TextMesh[] txtCharName, GameObject[] buttonDelete, GameObject[] charIcon, GameObject[] charIcon2)
        {
            for (int i = CharacterSlotPagesManager.GetCurrentCharacterIndex(0); i < CharacterSlotPagesManager.GetCurrentCharacterIndex(_charsPerPage); i++)
            {
                int @int = GetInt(i + "isChar");
                if (@int > 0)
                {
                    txtCharName[i].text = GetString(i + SaveFileConverter.GetConditionalSeparator() + "name") + "   Lv." + InstanceTracker.Menuu.GetPlayerLevel(GetInt(i + SaveFileConverter.GetConditionalSeparator() + "exp"));
                    txtCharName[i].color = Color.yellow;
                    buttonDelete[i].SetActive(true);
                    charIcon[i].SetActive(true);
                    int int2 = GetInt(string.Concat(new object[]
                    {
                        i,
                        SaveFileConverter.GetConditionalSeparator(),
                        38,
                        "id"
                    }));
                    charIcon[i].GetComponent<Renderer>().material = (Material)Resources.Load(string.Concat(new object[]
                    {
                        "r/r",
                        GetInt(i + SaveFileConverter.GetConditionalSeparator() + "race"),
                        "v",
                        GetInt(i + SaveFileConverter.GetConditionalSeparator() + "variant")
                    }));
                    if (int2 == 0)
                    {
                        charIcon2[i].GetComponent<Renderer>().material = (Material)Resources.Load("aug/aug" + GetInt(i + SaveFileConverter.GetConditionalSeparator() + "augment"));
                    }
                    else
                    {
                        charIcon2[i].GetComponent<Renderer>().material = (Material)Resources.Load("h/h" + int2);
                    }
                    InstanceTracker.Menuu.charIcon3[i].GetComponent<Renderer>().material = (Material)Resources.Load("aug/proff" + GetInt(i + "prof"));
                    txtCharName[i + 6].text = txtCharName[i].text;
                }
                else
                {
                    txtCharName[i].text = "EMPTY";
                    txtCharName[i].color = Color.white;
                    txtCharName[i + 6].text = txtCharName[i].text;
                    buttonDelete[i].SetActive(false);
                    charIcon[i].SetActive(false);
                }
            }
        }
    }
}*/