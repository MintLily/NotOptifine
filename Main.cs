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
        public const string Version = "1.3.0";
        public const string DownloadLink = "https://github.com/MintLily/NotOptifine";
        public const string Description = "DesktopFOV continuation; looks like it has features like Minecraft's Optifine, just not with the performance features.";
    }

    internal class Main : MelonMod
    {
        private MelonMod Instance;
        private MelonPreferences_Category melon;
        internal MelonPreferences_Entry<bool> hideReticleWhileZoomed, enableZoom, disableCTRLZoom;
        internal MelonPreferences_Entry<float> FOV, FOVChangedAmount, zoomMultiplier;
        internal static MelonPreferences_Entry<string> zoomKeybind;
        private bool isZoomed, _isInVr;
        private float preZoom = 60f, lastFOV;
        private KeyCode zKey = KeyCode.LeftAlt;
        private GameObject ReticleObj;

        private GameObject Reticle
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
            zoomKeybind = melon.CreateEntry("ZoomKeybind", "X", "Zoom Keybind");
            disableCTRLZoom = melon.CreateEntry("disableCTRLZoom", false, "Disable CTRL Zoom Feature");

            MelonLogger.Msg("Settings can be configured in UserData\\MelonPreferences.cfg or with UI Expansion Kit");
            MelonLogger.Msg($"[Ctrl + ScrollWheel] -> {(disableCTRLZoom.Value ? "Function Disabled" : "Increase or decrease field of view")}");
            MelonLogger.Msg("[Ctrl + ClickMiddleMouse] -> Reset field of view");
            zKey = Utils.GetParseZoomKeybind();
            if (enableZoom.Value) MelonLogger.Msg($"[{zoomKeybind.Value}] -> Zoom");
            MelonCoroutines.Start(WaitForApiPlayer());
        }

        private IEnumerator WaitForApiPlayer()
        {
            while (Player.prop_Player_0 == null || Player.prop_Player_0.prop_VRCPlayerApi_0 == null)
                yield return new WaitForSeconds(0.1F);
            _isInVr = Utils.IsInVR;
        }
        

        public override void OnPreferencesSaved() => zKey = Utils.GetParseZoomKeybind();

        public override void OnUpdate() => LoopEveryFrame_CuzThatsWhatOnUpdateMeans_ForVRChat_NotMinecraft_ThisIsNotOptifine(); // Sorry, just wanted funny name

        private void LoopEveryFrame_CuzThatsWhatOnUpdateMeans_ForVRChat_NotMinecraft_ThisIsNotOptifine()
        {
            if(_isInVr) return;
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
            if (!disableCTRLZoom.Value) {
                if ((Utils.GetKeyUp(KeyCode.LeftControl) || Utils.GetKeyUp(KeyCode.RightControl)) && !isZoomed && lastFOV != FOV.Value) {
                    MelonLogger.Msg($"FOV Changed: {FOV.Value}");
                    lastFOV = FOV.Value;
                }
            }

            Camera.main.fieldOfView = FOV.Value;
        }
    }
}
