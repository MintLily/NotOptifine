using MelonLoader;
using UnityEngine;
using UnityEngine.XR;

namespace NotOptifine
{
    public static class BuildInfo
    {
        public const string Name = "NotOptifine";
        public const string Author = "Moons, Lily";
        public const string Company = null;
        public const string Version = "1.4.1";
        public const string DownloadLink = "https://github.com/MintLily/NotOptifine";
        public const string Description = "DesktopFOV continuation; looks like it has features like Minecraft's Optifine, just not with the performance features.";
    }

    internal class Main : MelonMod
    {
        MelonPreferences_Category melon;
        internal MelonPreferences_Entry<bool> hideReticleWhileZoomed, enableZoom, disableCTRLZoom;
        internal MelonPreferences_Entry<float> FOV, FOVChangedAmount, zoomMultiplier;
        internal static MelonPreferences_Entry<string> ZoomKeybind;
        bool _isZoomed;
        float _preZoom = 60f, _lastFOV;
        KeyCode _zKey = KeyCode.LeftAlt;
        GameObject _reticleObj, _hudVoiceIcon;

        private GameObject Reticle
        {
            get {
                if (_reticleObj == null) _reticleObj = GameObject.Find("UserInterface/UnscaledUI/HudContent_Old/Hud/ReticleParent");
                return _reticleObj;
            }
        }

        private GameObject HUDIcon {
            get {
                if (_hudVoiceIcon == null) _hudVoiceIcon = GameObject.Find("UserInterface/UnscaledUI/HudContent_Old/Hud/VoiceDotParent");
                return _hudVoiceIcon;
            }
        }

        public override void OnApplicationStart()
        {
            melon = MelonPreferences.CreateCategory("DesktopFOV", BuildInfo.Name);
            FOV = melon.CreateEntry("DefaultFOV", 60f, "Field of View");
            FOVChangedAmount = melon.CreateEntry("FOVChangeAmount", 1f, "Change FOV Amount");
            zoomMultiplier = melon.CreateEntry("ZoomMultiplier", 6f, "Zoom Multiplier");
            hideReticleWhileZoomed = melon.CreateEntry("HideReticleWhenZoomed", true, "Hide Reticle While Zoomed?");
            enableZoom = melon.CreateEntry("EnableZoom", true, "Enable Zoom Feature");
            ZoomKeybind = melon.CreateEntry("ZoomKeybind", "X", "Zoom Keybind");
            disableCTRLZoom = melon.CreateEntry("disableCTRLZoom", false, "Disable CTRL Zoom Feature");

            LoggerInstance.Msg("Settings can be configured in UserData\\MelonPreferences.cfg or with UI Expansion Kit");
            LoggerInstance.Msg($"[Ctrl + ScrollWheel] -> {(disableCTRLZoom.Value ? "Function Disabled" : "Increase or decrease field of view")}");
            LoggerInstance.Msg("[Ctrl + ClickMiddleMouse] -> Reset field of view");
            _zKey = Utils.GetParseZoomKeybind();
            if (enableZoom.Value) LoggerInstance.Msg($"[{ZoomKeybind.Value}] -> Zoom");
        }
        

        public override void OnPreferencesSaved() => _zKey = Utils.GetParseZoomKeybind();

        public override void OnUpdate() => LoopEveryFrame_CuzThatsWhatOnUpdateMeans_ForVRChat_NotMinecraft_ThisIsNotOptifine(); // Sorry, just wanted funny name

        private void LoopEveryFrame_CuzThatsWhatOnUpdateMeans_ForVRChat_NotMinecraft_ThisIsNotOptifine()
        {
            if (Utils.GetVRCPlayer() == null) return;
            if (XRDevice.isPresent) return;
            if (!Application.isFocused && _isZoomed) {
                _isZoomed = false;
                FOV.Value = _preZoom;
                Reticle.SetActive(true);
                HUDIcon.SetActive(true);
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
            if (Utils.GetKey(_zKey) && enableZoom.Value && !_isZoomed) {
                _isZoomed = true;
                _preZoom = FOV.Value;
                FOV.Value /= zoomMultiplier.Value;
                if (hideReticleWhileZoomed.Value) {
                    Reticle.SetActive(false);
                    HUDIcon.SetActive(false);
                }
            }
            else if (Utils.GetKeyUp(_zKey) && enableZoom.Value && _isZoomed) {
                _isZoomed = false;
                FOV.Value = _preZoom;
                Reticle.SetActive(true);
                HUDIcon.SetActive(true);
            }

            // Log FOV change
            if (!disableCTRLZoom.Value) {
                if ((Utils.GetKeyUp(KeyCode.LeftControl) || Utils.GetKeyUp(KeyCode.RightControl)) && !_isZoomed && _lastFOV != FOV.Value) {
                    LoggerInstance.Msg($"FOV Changed: {FOV.Value}");
                    _lastFOV = FOV.Value;
                }
            }

            if (Camera.main != null) Camera.main.fieldOfView = FOV.Value;
        }
    }
}
