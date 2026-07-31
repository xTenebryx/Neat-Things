/*using GadgetCore;
using GadgetCore.API;
using HarmonyLib;
using UnityEngine;
using static PreviewLabs.PlayerPrefs;
using static NeatThings.Gadgets.KnockbackAdjustments;
using NeatThings.Patches.MoreCharacterSlots_InfoPatches;
using System.Collections;
using System.IO;
using System.Globalization;
using GadgetCore.Util;

namespace NeatThings.Gadgets
{
    [Gadget("MoreCharacterSlotsAndInfo", true)]
    internal class KnockbackAdjustments : Gadget<KnockbackAdjustments>
    {
        // Knockback Adjustments Config values

        internal static float xLevelPosCoeff = 22.5f;
        internal static float yLevelPosCoeff = 22.5f;

        internal static bool shouldEnemiesUseNewKB = true;
        internal static bool shouldPlayerUseNewKB = false;

        internal static float flyingEnemiesKBTime = 0.75f;
        internal static float flyingEnemiesKBSlowCoeff = 1.25f;

        public static GadgetLogger KALogger;

        public enum KnockbackSource
        {
            PlayerAttackCubeOrProj,
            EnemyHazardColOrProj,
            Other
        }

        protected override void LoadConfig()
        {
            Config.Load();

            string fileVersion = Config.ReadString("ConfigVersion", NeatThings.CONFIG_VERSION, comments: "The Config Version (not to be confused with mod version)");

            if (fileVersion != NeatThings.CONFIG_VERSION)
            {
                Config.Reset();
                Config.WriteString("ConfigVersion", NeatThings.CONFIG_VERSION, comments: "The Config Version (not to be confused with mod version)");
            }

            // Do stuff with `Config`

            Config.Save();
        }

        public override string GetModDescription()
        {
            return "Did you ever fought with vile creatures of vast planets only for them to be knocked right to you, because the bullet just so happened to hit head/legs slightly to the other side?\r\n... No? Anyway, this mod aims to mitigate this quirk, introducing y-level dependency of damage source. Now you can juggle enemies left and right!\r\n\r\n" +
                "Melee hits knockback might be almost indistingushable from vanilla, as you are the source, and there's little to no maneuver to try to balance anything in the air, but ranged projectiles benefit from this fully.\r\n" +
                "Caution, staff \"cornering\" tactic is mildly affected as well, it'll be harder to pull of with fast projectiles that quickly change direction, not necessarily back and forth from the cursor.\r\n\r\n" +
                "There's even an option to apply same logic to you!.. Why would you nerf yourself is another question, I implemented it because funi.\r\n" +
                "And yes, all coefficients are configurable, you can revert y-level contribution, but increase x one, or crank them with \"Smooth Knockack\" enabled for Slap Hand from Terraria effect at home.";
        }

        protected override void Initialize()
        {
            KALogger = Logger;
            KALogger.Log("NeatThings v" + Info.Mod.Version);

            // TODO: Do stuff like registering items
        }

        internal static void InitiateKnockback(GameObject target, KnockbackSource knockbackSource, float xPosSource, float yPosSource)
        {
            switch (knockbackSource)
            {
                case KnockbackSource.PlayerAttackCubeOrProj:
                    target.GetComponent<EnemyScript>().StartCoroutine(KnockbackEnemy(target, xPosSource, yPosSource));
                    break;

                case KnockbackSource.EnemyHazardColOrProj:
                    target.GetComponent<PlayerScript>().StartCoroutine(KnockbackPlayer(target, xPosSource, yPosSource));
                    break;

                case KnockbackSource.Other:

                    break;
            }

        }

        // Vanilla-esque implementation, with slight adjustments... for knockback... yeah

        internal const float yLevelPosAdjustment = 1.0f;

        internal static Vector3 HandleVelocity(Transform thisT, float xPosSource, float yPosSource)
        {
            return new Vector3((thisT.position.x - xPosSource) * xLevelPosCoeff,
                               (thisT.position.y - yPosSource) * yLevelPosCoeff + yLevelPosAdjustment,
                               0f);
        }

        internal static IEnumerator KnockbackPlayer(GameObject target, float xPosSource, float yPosSource)
        {
            PlayerScript thisPS = InstanceTracker.PlayerScript;
            Transform thisT = target.transform;
            Rigidbody thisR = target.GetComponent<Rigidbody>();

            if (!GameScript.dead && !thisPS.GetFieldValue<bool>("knock"))
            {
                thisPS.SetFieldValue("knock", true);
                thisPS.SetFieldValue("knocking", true);
                thisR.velocity = new Vector3(0f, 0f, 0f);

                thisR.velocity = HandleVelocity(thisT, xPosSource, yPosSource);

                yield return new WaitForSeconds(0.1f);
                thisPS.SetFieldValue("knocking", false);
                yield return new WaitForSeconds(0.5f);
                thisPS.SetFieldValue("knock", false);
                yield return null;
            }
            yield break;
        }

        internal static IEnumerator KnockbackEnemy(GameObject target, float xPosSource, float yPosSource)
        {
            EnemyScript thisES = target.GetComponent<EnemyScript>();
            Transform thisT = target.transform;
            Rigidbody thisR = target.GetComponent<Rigidbody>();

            if (thisR != null)
            {
                thisES.knocking = true;
                bool f = false;
                if (thisR.isKinematic)
                {
                    thisR.isKinematic = false;
                    f = true;
                }
                thisR.velocity = new Vector3(0f, 0f, 0f);

                thisR.velocity = HandleVelocity(thisT, xPosSource, yPosSource);

                yield return new WaitForSeconds(0.2f);
                thisES.knocking = false;
                if (!thisR.isKinematic)
                {
                    float timer = 0f;

                    while (timer < flyingEnemiesKBTime)
                    {
                        thisR.velocity /= flyingEnemiesKBSlowCoeff;
                        timer += Time.deltaTime;
                        yield return null;
                    }
                    thisR.velocity = new Vector3(0f, 0f, 0f);
                }
                yield return new WaitForSeconds(0.1f);
                thisES.observing = true;
                if (f)
                {
                    thisR.isKinematic = true;
                }
                yield return null;
                yield break;
            }
        }
    }
}*/
