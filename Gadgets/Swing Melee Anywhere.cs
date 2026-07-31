using GadgetCore;
using GadgetCore.API;
using UnityEngine;

namespace NeatThings.Gadgets
{
    [Gadget("SwingMeleeAnywhere", false,
        Dependencies: new string[] { "MeleeRangePlus" })]
    public class SwingMeleeAnywhere : Gadget<SwingMeleeAnywhere>
    {
        // Knockback Adjustments Config values

        public static Quaternion heldAppearanceParentRotation;

        public static Quaternion attackCube1BaseRotation;
        public static Quaternion attackCube2BaseRotation;
        public static Quaternion attackCube3BaseRotation;

        public static GadgetLogger SMALogger;

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
            return "An addon for MeleeRangePlus mod that allows to... Swing melee weapons anywhere around yourself, not just left/right.\n\n" +
                "Best used for lances, as you might imagine. Swords are a little janky, and animation gets chopped down if you repeadetly attack, or have AutoAttack mod by SuperKael.";
        }

        protected override void Initialize()
        {
            SMALogger = Logger;
            SMALogger.Log("NeatThings v" + Info.Mod.Version);

            // TODO: Do stuff like registering items
        }
    }
}