using MelonLoader;
using HarmonyLib;
using Il2Cpp;
using System;

[assembly: MelonInfo(typeof(NoWintermuteMisclicks.Main), "No Wintermute Misclicks", "1.0.0", "Lans42")]
[assembly: MelonGame("Hinterland", "TheLongDark")]

namespace NoWintermuteMisclicks
{
    public class Main : MelonMod
    {
        public static void OnConfirmYes()
        {
            MelonLogger.Msg("Player confirmed transition. Launching Wintermute...");

            Panel_MainMenu panelMenu = InterfaceManager.GetPanel<Panel_MainMenu>();
            if (panelMenu != null)
            {
                Patch_Panel_MainMenu_OnStory.isConfirmedCall = true;
                
                panelMenu.OnStory();
                
                Patch_Panel_MainMenu_OnStory.isConfirmedCall = false;
            }
        }
    }

    [HarmonyPatch(typeof(Panel_MainMenu), "OnStory")]
    internal class Patch_Panel_MainMenu_OnStory
    {
        public static bool isConfirmedCall = false;

        private static bool Prefix()
        {
            if (isConfirmedCall)
            {
                return true;
            }

            // MelonLogger.Warning("Intercepted click on Wintermute. Showing confirmation dialogue.");

            Panel_Confirmation panel = InterfaceManager.GetPanel<Panel_Confirmation>();
            if (panel != null && !panel.isActiveAndEnabled)
            {
                Action yesTarget = new Action(Main.OnConfirmYes);

                panel.ShowConfirmPanel(
                    locID: "Are you sure you want to switch to Story mode?",
                    buttonPromptLocId1: "Yes",
                    buttonPromptLocId2: "No",
                    confirmCallback: yesTarget,
                    cancelCallback: null
                );
            }
            else
            {
                MelonLogger.Warning("Failed to show dialogue: Panel_Confirmation is null or already active.");
            }

            return false;
        }
    }
}
