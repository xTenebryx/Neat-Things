using GadgetCore.API;
using GadgetCore.Util;
using HarmonyLib;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using static NeatThings.Gadgets.SwingMeleeAnywhere;
using static MeleeRangePlus.MeleeRangePlus;

namespace NeatThings.Patches.SwingMeleeAnywhere
{
    [HarmonyPatch(typeof(PlayerScript))]
    [HarmonyPatch(nameof(PlayerScript.Animate))]
    [HarmonyGadget(nameof(SwingMeleeAnywhere))]
    public static class Patch_PlayerScript_Animate
    {
        private static readonly FieldInfo _animFI = typeof(NetworkPlayerN).GetField("anim", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly Func<NetworkPlayerN, Animation> _animComponentGetter = ReflectionUtils.CreateGetter<NetworkPlayerN, Animation>(_animFI);

        private static Coroutine changingRotation;
        private const float _attackSwingLength = 0.65f;

        [HarmonyPostfix]
        public static void Postfix(PlayerScript __instance, int a)
        {
            if (a == 3 || a == 5)
            {
                if (changingRotation != null)
                {
                    __instance.StopCoroutine(changingRotation);
                    changingRotation = __instance.StartCoroutine(AdjustRotation(__instance, a));
                }
                else
                {
                    changingRotation = __instance.StartCoroutine(AdjustRotation(__instance, a));
                }
            }
        }

        private static IEnumerator AdjustRotation(PlayerScript playerScriptInst, int mode)
        {
            Quaternion quaternion = GetRotationCursorToPlayer(playerScriptInst);

            //SMALogger.LogConsole("Currently aimed at: " + quaternion.eulerAngles.ToString());

            HeldAppearanceParent.localRotation = quaternion;
            playerScriptInst.attackCube.transform.localRotation = Quaternion.Euler(0f, 0f, quaternion.z + 180f);
            playerScriptInst.attackCube2.transform.localRotation = quaternion;
            playerScriptInst.attackCube3.transform.localRotation = quaternion;

            if (mode == 3)
                yield return new WaitForSeconds(_attackSwingLength * _animComponentGetter(playerScriptInst.gameObject.GetComponent<NetworkPlayerN>())["s1"].speed / 1.1f);
            else
                yield return new WaitForSeconds(_attackSwingLength * _animComponentGetter(playerScriptInst.gameObject.GetComponent<NetworkPlayerN>())["l1"].speed / 1.1f);

            HeldAppearanceParent.localRotation = heldAppearanceParentRotation;
            playerScriptInst.attackCube.transform.localRotation = attackCube1BaseRotation;
            playerScriptInst.attackCube2.transform.localRotation = attackCube2BaseRotation;
            playerScriptInst.attackCube3.transform.localRotation = attackCube3BaseRotation;

            changingRotation = null;
            yield break;
        }

        private static Quaternion GetRotationCursorToPlayer(PlayerScript playerScriptInst)
        {
            Quaternion rotateTo;

            Vector3 playerPos = playerScriptInst.transform.position;
            Vector3 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (cursorPos.x > playerPos.x)
                rotateTo = Quaternion.Euler(0f, 0f, -(Mathf.Rad2Deg * Mathf.Atan2(cursorPos.y - playerPos.y, cursorPos.x - playerPos.x)));
            else
                rotateTo = Quaternion.Euler(0f, 0f, 180f + (Mathf.Rad2Deg * Mathf.Atan2(cursorPos.y - playerPos.y, cursorPos.x - playerPos.x)));

            return rotateTo;
        }
    }
}