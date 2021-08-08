using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using MelonLoader;
using UnityEngine;
using UnityEngine.XR;
using VRC;

namespace NotOptifine
{
    public static class BuildInfo
    {
        public const string Name = "NotOptifine";
        public const string Author = "Moons, Lily";
        public const string Company = null;
        public const string Version = "1.2.0";
        public const string DownloadLink = "https://github.com/MintLily/NotOptifine";
        public const string Description = "DesktopFOV continuation; looks like it has features like Minecraft's Optifine, just not with the performance features.";
    }

    internal class Main : MelonMod
    {
        MelonMod Instance;
        MelonPreferences_Category melon;
        internal MelonPreferences_Entry<bool> hideReticleWhileZoomed, enableZoom;
        internal MelonPreferences_Entry<float> FOV, FOVChangedAmount, zoomMultiplier;
        internal static MelonPreferences_Entry<string> zoomKeybind;
        bool isZoomed;
        float preZoom = 60f, lastFOV;
        KeyCode zKey = KeyCode.LeftAlt;
        GameObject ReticleObj;

        public GameObject Reticle
        {
            get {
                if (ReticleObj == null) ReticleObj = GameObject.Find("UserInterface/UnscaledUI/HudContent/Hud/ReticleParent");
                return ReticleObj;
            }
        }

        public override void OnApplicationStart()
        {
            Instance = this;

            melon = MelonPreferences.CreateCategory("DesktopFOV", BuildInfo.Name);
            FOV = melon.CreateEntry("DefaultFOV", 60f, "Field of View");
            FOVChangedAmount = melon.CreateEntry("FOVChangeAmount", 1f, "Change FOV Amount");
            zoomMultiplier = melon.CreateEntry("ZoomMultiplier", 6f, "Zoom Multiplier");
            hideReticleWhileZoomed = melon.CreateEntry("HideReticleWhenZoomed", true, "Hide Reticle While Zoomed?");
            enableZoom = melon.CreateEntry("EnableZoom", true, "Enable Zoom Feature");
            zoomKeybind = melon.CreateEntry("ZoomKeybind", "LeftAlt", "Zoom Keybind");

            MelonLogger.Msg("Settings can be configured in UserData\\MelonPreferences.cfg or with UI Expansion Kit");
            MelonLogger.Msg("[Ctrl + ScrollWheel] -> Increase or decrease field of view");
            MelonLogger.Msg("[Ctrl + ClickMiddleMouse] -> Reset field of view");
            zKey = Utils.GetParseZoomKeybind();
            if (enableZoom.Value) MelonLogger.Msg($"[{zoomKeybind.Value}] -> Zoom");
        }
        

        public override void OnPreferencesSaved() => zKey = Utils.GetParseZoomKeybind();

        public override void OnUpdate() => LoopEveryFrame_CuzThatsWhatOnUpdateMeans_ForVRChat_NotMinecraft_ThisIsNotOptifine(); // Sorry, just wanted funny name

        internal void LoopEveryFrame_CuzThatsWhatOnUpdateMeans_ForVRChat_NotMinecraft_ThisIsNotOptifine()
        {
            if (Utils.GetVRCPlayer() == null) return;
            if (!Application.isFocused && isZoomed) {
                isZoomed = false;
                FOV.Value = preZoom;
                Reticle.SetActive(true);
            }

            // FOV Up
            if (Utils.GetAxis("Mouse ScrollWheel", true) < 0f)
                FOV.Value += FOVChangedAmount.Value;
            // FOV Down
            if (Utils.GetAxis("Mouse ScrollWheel", true) > 0f)
                FOV.Value -= FOVChangedAmount.Value;
            // FOV Reset
            if (Utils.GetMouseButtonDown(2, true))
                FOV.Value = 60f;

            // Zooooooooooommmm
            if (Utils.GetKey(zKey) && enableZoom.Value && !isZoomed) {
                isZoomed = true;
                preZoom = FOV.Value;
                FOV.Value /= zoomMultiplier.Value;
                if (hideReticleWhileZoomed.Value) Reticle.SetActive(false);
            }
            else if (Utils.GetKeyUp(zKey) && enableZoom.Value && isZoomed) {
                isZoomed = false;
                FOV.Value = preZoom;
                Reticle.SetActive(true);
            }

            // Log FOV change
            if ((Utils.GetKeyUp(KeyCode.LeftControl) || Utils.GetKeyUp(KeyCode.RightControl)) && !isZoomed && lastFOV != FOV.Value) {
                MelonLogger.Msg($"FOV Changed: {FOV.Value}");
                lastFOV = FOV.Value;
            }

            Camera.main.fieldOfView = FOV.Value;
        }
    }
}
