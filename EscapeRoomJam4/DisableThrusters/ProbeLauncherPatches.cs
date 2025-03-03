﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeRoomJam4.DisableThrusters;

[HarmonyPatch(typeof(ProbeLauncher))]
public static class ProbeLauncherPatches
{
    public static bool ProbeLauncherDisabled;

    [HarmonyPrefix]
    [HarmonyPatch(nameof(ProbeLauncher.LaunchProbe))]
    public static bool ProbeLauncher_LaunchProbe(ProbeLauncher __instance)
    {
        if (EscapeRoomJam4.InEscapeSystem() && ProbeLauncherDisabled)
        {
            NotificationData data = new NotificationData(__instance._notificationFilter, 
                EscapeRoomJam4.Instance.NewHorizons.GetTranslationForOtherText("PROBE_LAUNCHER_JAMMED"), 3f, false);
            NotificationManager.SharedInstance.PostNotification(data, false);
            Locator.GetPlayerAudioController().PlayNegativeUISound();

            if (__instance._name == ProbeLauncher.Name.Player)
            {
                __instance._photoMode = !__instance._photoMode;
                __instance._effects.PlayChangeModeClip();
            }

            return false;
        }
        return true;
    }
}
