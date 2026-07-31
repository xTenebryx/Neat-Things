/*using GadgetCore;
using GadgetCore.API;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using static PreviewLabs.PlayerPrefs;
using static NeatThings.Gadgets.MoreCharacterSlotsAndInfo;
using NeatThings.Patches.MoreCharacterSlotsPatches;

namespace NeatThings.Gadgets
{
    [Gadget("MoreCharacterSlotsAndInfo", false)]
    public class MoreCharacterSlotsAndInfo : Gadget<MoreCharacterSlotsAndInfo>
    {
        // More Character Slots Config values
        internal static int ChosenCharacterPageCount = 1;

        public const int _mininumPages = 1;
        public const int _maxinumPages = 10;

        // More Character Info Config values
        internal static bool ShowExtraInfoOnCharacterSelection = true;
        internal static KeyCode CharacterStatsMenuKey = KeyCode.Semicolon;

        public static GadgetLogger MCSLogger;

		protected override void LoadConfig()
		{
			Config.Load();
			
			string fileVersion = Config.ReadString("ConfigVersion", NeatThings.CONFIG_VERSION, comments: "The Config Version (not to be confused with mod version)");

            if (fileVersion != NeatThings.CONFIG_VERSION)
            {
                Config.Reset();
                Config.WriteString("ConfigVersion", NeatThings.CONFIG_VERSION, comments: "The Config Version (not to be confused with mod version)");
            }

            ChosenCharacterPageCount = Config.ReadInt(
                "Character Pages",
                defaultValue: 1,
                requiresRestart: false,
                minValue: _mininumPages,
                maxValue: _maxinumPages,
                comments: "Character pages amount, including first. WIP."
                );
            ShowExtraInfoOnCharacterSelection = Config.ReadBool(
                "Info on Char. Sel.",
                defaultValue: true,
                requiresRestart: false,
                comments: "Extra info for character you hover on selection screen"
                );
            CharacterStatsMenuKey = Config.ReadKeyCode(
                "Stats Key",
                defaultValue: KeyCode.Semicolon,
                requiresRestart: false,
                comments: "Key to show current characters stats, like on Death Screen. Unlock conditions and what you achieved follow."
                );

            Config.Save();
        }

        public override string GetModDescription()
        {
            return "This Gadget lets you have MORE characters in one save file, and have info about hovered character's Class, Allegiance and Lifetime, for easier distinguishing!\n\n" +
                   "By default, there's 2 more character selection pages to work with, but you can adjust that number. Be careful, once you create a character deep in, and decrease \"Character Pages\" config value, it won't be accesible anymore until you revert it (or change all keys in PlayerPrefs.txt associated with this character to lower value. Tedious at best, prone to errors at worst).\n" +
                   "And, just for your own peace of mind, don't meddle with it too much, it's untested and loading times might increase even more from dozens of saved characters.\n" +
                   "Report any issues to #general-modded/#general-help on New Roguelands Discord Server, or DM @.tenebry";
        }

        protected override void Initialize()
        {
            MCSLogger = Logger;
            MCSLogger.Log("NeatThings v" + Info.Mod.Version);

            if (ChosenCharacterPageCount > 1)
            {
                if (!CharacterSlotPagesManager.AddedOnLoad)
                {
                    CharacterSlotPagesManager.CreatePageTextAndButtons();
                    CharacterSlotPagesManager.AddedOnLoad = true;
                }
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
        }

        protected override void Unload()
        {
            CharacterSlotPagesManager.DestroyPageSelectionButtons();
            CharacterSlotPagesManager.AddedOnLoad = false;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        internal void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.buildIndex == 0)
            {
                MCSLogger.LogConsole(nameof(OnSceneLoaded) + ": Added action.");
                CharacterSlotPagesManager.CreatePageTextAndButtons();
            }
        }
    }



    internal static class CharacterSlotPagesManager
    {
        public static int CurrentPage
        { get; private set; } = 0;

        public static bool AddedOnLoad = false;

        private static GameObject _currentPageTextHolder;
        private static GameObject _currentPageTextShadowHolder;
        private static TextMesh[] _PageTextHolderTextMeshes;

        private static GameObject _characterPageNext;
        public const string CharacterPageNextName = "bCharacterPageNext";
        private static GameObject _characterPagePrevious;
        public const string CharacterPagePrevious = "bCharacterPagePrevious";

        internal static void RearrangeVanillaButtonsAndAddNew()
        {

        }

        private static Vector3 _pageTextOffset = new Vector3(0f, 8.5f, 0f);
        private static Vector3 _selectionButtonsOffset = new Vector3(8.5f, 0f, 0f);
        private static Vector3 _selectionButtonsSize = new Vector3(2f, 2f, 1f);

        internal static void CreatePageTextAndButtons()
        {
            // Page Text Creation

            Transform txtSelectTransform = InstanceTracker.Menuu.menuCharSelect.transform.Find("txtSELECT");
            TextMesh textMeshToCopy = txtSelectTransform.GetComponent<TextMesh>();

            if (txtSelectTransform != null &&
                textMeshToCopy != null)
            {
                _currentPageTextHolder = new GameObject(nameof(MoreCharacterSlotsAndInfo) + "_Page_Text");
                _currentPageTextHolder.transform.position = txtSelectTransform.position;
                _currentPageTextHolder.transform.localScale = txtSelectTransform.localScale;
                _currentPageTextHolder.transform.SetParent(txtSelectTransform);
                _currentPageTextHolder.transform.localPosition -= _pageTextOffset;
                TextMesh mainPageTextMesh = _currentPageTextHolder.AddComponent<TextMesh>();
                mainPageTextMesh.characterSize = 0.6f;
                mainPageTextMesh.anchor = TextAnchor.MiddleCenter;
                mainPageTextMesh.alignment = TextAlignment.Center;
                mainPageTextMesh.font = textMeshToCopy.font;
                mainPageTextMesh.color = Color.white;
                mainPageTextMesh.text = $"Page: {GetCurrentPageVisual()}/{GetMaximumPage()}";
                _PageTextHolderTextMeshes.AddItem(mainPageTextMesh);
                _currentPageTextHolder.SetActive(false);

                _currentPageTextShadowHolder = new GameObject(nameof(MoreCharacterSlotsAndInfo) + "_Page_Text_Shadow");
                _currentPageTextShadowHolder.transform.position = _currentPageTextHolder.transform.position;
                _currentPageTextShadowHolder.transform.localScale = txtSelectTransform.localScale;
                _currentPageTextShadowHolder.transform.SetParent(_currentPageTextHolder.transform);
                _currentPageTextShadowHolder.transform.localPosition += new Vector3(1f, -1f, 0.1f);
                TextMesh mainPageTextShadowMesh = _currentPageTextShadowHolder.AddComponent<TextMesh>();
                mainPageTextShadowMesh.characterSize = mainPageTextMesh.characterSize;
                mainPageTextShadowMesh.anchor = mainPageTextMesh.anchor;
                mainPageTextShadowMesh.alignment = mainPageTextMesh.alignment;
                mainPageTextShadowMesh.font = textMeshToCopy.font;
                mainPageTextShadowMesh.color = Color.black;
                mainPageTextShadowMesh.text = mainPageTextMesh.text;
                _PageTextHolderTextMeshes.AddItem(mainPageTextShadowMesh);

                MCSLogger.LogConsole(nameof(MoreCharacterSlotsAndInfo.OnSceneLoaded) + ": Should have created text.");
            }

            // Page Selection Buttons Creation

            GameObject boxButtonOriginal = InstanceTracker.Menuu.box[0];
            Transform backButton = InstanceTracker.Menuu.menuCharSelect.transform.Find("BACK");
            Transform musicButtonUp = InstanceTracker.Menuu.menuOptions.transform.Find("optionsmenu/bHigherM");
            Transform musicButtonDown = InstanceTracker.Menuu.menuOptions.transform.Find("optionsmenu/bLowerM");

            AnimationClip floatAnim = backButton.Find("bBack").gameObject.GetComponent<Animation>().clip;

            if (boxButtonOriginal != null &&
                backButton != null &&
                musicButtonUp != null &&
                musicButtonDown != null)
            {
                _characterPageNext = new GameObject(CharacterPageNextName);
                _characterPageNext.transform.position = backButton.position;
                _characterPageNext.transform.SetParent(backButton);
                _characterPageNext.transform.localScale = _selectionButtonsSize;
                _characterPageNext.transform.localPosition += _selectionButtonsOffset;
                _characterPageNext.layer = 0;
                GameObject _characterPageNextChild = Object.Instantiate(boxButtonOriginal);
                _characterPageNextChild.name = CharacterPageNextName + "_Child";
                _characterPageNextChild.transform.SetParent(_characterPageNext.transform);
                _characterPageNextChild.layer = 0;
                ButtonMenu CPNextButtonMenu = Object.Instantiate(musicButtonUp).GetComponent<ButtonMenu>();
                _characterPageNextChild.GetComponent<BoxCollider>().size = Vector3.one;
                _characterPageNextChild.GetComponent<MeshRenderer>().enabled = true;
                _characterPageNextChild.GetComponent<MeshRenderer>().material = musicButtonUp.GetComponent<ButtonMenu>().button;
                CPNextButtonMenu.minorButton = true;
                CPNextButtonMenu.button = musicButtonUp.GetComponent<ButtonMenu>().button;
                CPNextButtonMenu.buttonSelect = musicButtonUp.GetComponent<ButtonMenu>().buttonSelect;
                Animation CPNextAnimation = _characterPageNextChild.AddComponent<Animation>();
                CPNextAnimation.AddClip(floatAnim, "floatPage");
                CPNextAnimation.clip = CPNextAnimation.GetClip("floatPage");
                CPNextAnimation.playAutomatically = true;
                CPNextAnimation.Play();
                _characterPageNext.SetActive(false);

                _characterPagePrevious = new GameObject(CharacterPagePrevious);
                _characterPagePrevious.transform.position = backButton.position;
                _characterPagePrevious.transform.SetParent(backButton);
                _characterPagePrevious.transform.localScale = _selectionButtonsSize;
                _characterPagePrevious.transform.localPosition -= _selectionButtonsOffset;
                _characterPagePrevious.layer = 0;
                GameObject _characterPagePreviousChild = Object.Instantiate(boxButtonOriginal);
                _characterPagePreviousChild.name = CharacterPagePrevious + "_Child";
                _characterPagePreviousChild.transform.SetParent(_characterPagePrevious.transform);
                _characterPagePreviousChild.layer = 0;
                ButtonMenu CPPreviousButtonMenu = Object.Instantiate(musicButtonDown).GetComponent<ButtonMenu>();
                _characterPagePreviousChild.GetComponent<BoxCollider>().size = Vector3.one;
                _characterPagePreviousChild.GetComponent<MeshRenderer>().enabled = true;
                _characterPagePreviousChild.GetComponent<MeshRenderer>().material = musicButtonDown.GetComponent<ButtonMenu>().button;
                CPPreviousButtonMenu.minorButton = true;
                CPPreviousButtonMenu.button = musicButtonDown.GetComponent<ButtonMenu>().button;
                CPPreviousButtonMenu.buttonSelect = musicButtonDown.GetComponent<ButtonMenu>().buttonSelect;
                Animation CPPrevioisAnimation = _characterPagePreviousChild.AddComponent<Animation>();
                CPPrevioisAnimation.AddClip(floatAnim, "floatPage");
                CPPrevioisAnimation.clip = CPPrevioisAnimation.GetClip("floatPage");
                CPPrevioisAnimation.playAutomatically = true;
                CPPrevioisAnimation.Play();
                _characterPagePrevious.SetActive(false);

                MCSLogger.LogConsole(nameof(MoreCharacterSlotsAndInfo.OnSceneLoaded) + ": Should have created buttons.");
            }
        }

        private static int GetCurrentPageVisual()
        {
            MCSLogger.LogConsole($"{nameof(GetCurrentPageVisual)}: Called, should return {CurrentPage}");
            return CurrentPage + 1;
        }

        private static int GetMaximumPage()
        {
            if (ChosenCharacterPageCount > _mininumPages && ChosenCharacterPageCount < _maxinumPages)
            {
                MCSLogger.LogConsole($"{nameof(GetMaximumPage)}: Called, should return {ChosenCharacterPageCount}");
                return ChosenCharacterPageCount;
            }
            else
            {
                MCSLogger.LogError(nameof(GetMaximumPage) + ": \"Character Pages\" config value weren't in range, fallback to minimum (buttons should be removed).", true);
                return _mininumPages;
            }
        }

        public static void ChangePage(bool toNextPage)
        {
            if (toNextPage)
            {
                if (CurrentPage + 1 > GetMaximumPage())
                {
                    MCSLogger.LogConsole($"{nameof(ChangePage)}: Last page was reached, returning.");
                    return;
                }
                else
                {
                    MCSLogger.LogConsole($"{nameof(ChangePage)}: Last page wasn't reached, adding up.");
                    CurrentPage++;
                    RefreshCurrentPage();
                }
            }
            else
            {
                if (CurrentPage <= 0)
                {
                    MCSLogger.LogConsole($"{nameof(ChangePage)}: First page was reached, returning.");
                    return;
                }
                else
                {
                    MCSLogger.LogConsole($"{nameof(ChangePage)}: Last page wasn't reached, substracting.");
                    CurrentPage--;
                    RefreshCurrentPage();
                }
            }
        }

        public static int GetCurrentCharacterIndex(int defaultCharacterIndex)
        {
            return defaultCharacterIndex + 6 * CurrentPage;
        }

        private static void RefreshCurrentPage()
        {
            if (!InstanceTracker.Menuu.menuDeleteChar.activeSelf)
            {
                MCSLogger.LogConsole($"{nameof(RefreshCurrentPage)}: Starting to refresh current page.");

                foreach (TextMesh textHolder in _PageTextHolderTextMeshes)
                {
                    textHolder.text = $"Page: {GetCurrentPageVisual()}/{GetMaximumPage()}";
                }

                Patch_Menuu_CharSelect.PopulateCharacterButtons(InstanceTracker.Menuu.txtCharName,
                                                                InstanceTracker.Menuu.buttonDelete,
                                                                InstanceTracker.Menuu.charIcon,
                                                                InstanceTracker.Menuu.charIcon2);

                InstanceTracker.Menuu.beam[5].GetComponent<Animation>().Play();
                InstanceTracker.Menuu.beam[6].GetComponent<Animation>().Play();
                InstanceTracker.Menuu.beam[7].GetComponent<Animation>().Play();
                InstanceTracker.Menuu.beam[8].GetComponent<Animation>().Play();
            }
        }

        public static void EnableOrDisablePageSelectionButtons()
        {
            if (GetMaximumPage() > _mininumPages && SaveFileConverter.IsUsingNewFormat)
            {
                MCSLogger.LogConsole($"{nameof(EnableOrDisablePageSelectionButtons)}: Maximum pages count is valid AND save file was converted, should enable text and buttons.");

                _currentPageTextHolder?.SetActive(true);
                _characterPageNext?.SetActive(true);
                _characterPagePrevious?.SetActive(true);

                /*if (_PageTextHolderTextMeshes.Length > 0)
                {
                    foreach (TextMesh textHolder in _PageTextHolderTextMeshes)
                    {
                        if (textHolder != null)
                        {
                            textHolder.text = $"Page: {GetCurrentPageVisual()}/{GetMaximumPage()}";
                        }
                    }
                }
            }
            else
            {
                MCSLogger.LogConsole($"{nameof(EnableOrDisablePageSelectionButtons)}: Maximum pages count is invalid OR save file wasn't converted, should disable text and buttons.");

                _currentPageTextHolder?.SetActive(false);
                _characterPageNext?.SetActive(false);
                _characterPagePrevious?.SetActive(false);
            }
        }

        public static void DestroyPageSelectionButtons()
        {
            if (_currentPageTextHolder != null ||
                _characterPageNext != null ||
                _characterPagePrevious != null)
                Object.Destroy(_currentPageTextHolder);
            _currentPageTextHolder = null;
            Object.Destroy(_characterPageNext);
            _characterPageNext = null;
            Object.Destroy(_characterPagePrevious);
            _characterPagePrevious = null;
        }
    }



    internal static class SaveFileConverter
    {
        public static string SaveFormatPlayerPrefsKey = nameof(MoreCharacterSlotsAndInfo) + "_Save_Reinvention";
        public static string SaveFormatFirstOpenPlayerPrefsKey = nameof(MoreCharacterSlotsAndInfo) + "_For_First_Open";

        public static bool IsUsingNewFormat = false;
        public const string CharSaveSlotSeparator = "|";

        public static void SaveFileConverterInitialise()
        {
            if (!HasKey(SaveFormatPlayerPrefsKey) ||
                !HasKey(SaveFormatFirstOpenPlayerPrefsKey))
            {
                SetBool(SaveFormatPlayerPrefsKey, false);
                SetBool(SaveFormatFirstOpenPlayerPrefsKey, false);
                Flush();
            }

            if (!GetBool(SaveFormatFirstOpenPlayerPrefsKey))
            {
                GadgetCoreAPI.DisplayYesNoDialog(
                    text: "For supporting more than 9 Characters on savefile, the mod requires you to undergo one-time migration," +
                    "otherwise it would look like everything has viped (although there might be some problems, still in development).\n" +
                    "You can revert back in options menu, or load backup from GadgetCore in case of such occasion.\n\n" +
                    "Proceed?",
                    onYes: () => SaveFileConverterProcess(true));
            }

            CheckForUsingNewFormat();
        }

        public static void SaveFileConverterProcess(bool toNewFormat)
        {
            if (toNewFormat)
            {


                if (!GetBool(SaveFormatFirstOpenPlayerPrefsKey))
                {
                    SetBool(SaveFormatFirstOpenPlayerPrefsKey, true);
                    Flush();
                }
            }
            else 
            {

            }
        }

        public static void CheckForUsingNewFormat()
        {
            IsUsingNewFormat = GetBool(SaveFormatPlayerPrefsKey);
        }

        public static string GetConditionalSeparator()
        {
            return IsUsingNewFormat ? CharSaveSlotSeparator : string.Empty;
        }
    }



    internal class ExtraInfoDefinition
    {
        public GameObject CharacterRepresentation;
        public string ClassName;
        public string AllegianceNameAndLevel;
        public string Lifetime;
    }
}*/