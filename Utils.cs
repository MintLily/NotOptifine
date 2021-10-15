using System;
using System.Linq;
using MelonLoader;
using UnityEngine;
using VRC;

namespace NotOptifine
{
    internal static class Utils
    {
        public static VRCPlayer GetVRCPlayer() { return VRCPlayer.field_Internal_Static_VRCPlayer_0; }

		public static KeyCode GetParseZoomKeybind()
		{
			string yes = Main.zoomKeybind.Value;
			if (string.IsNullOrWhiteSpace(yes)) yes = "LeftAlt";

			if (yes.Length == 1) yes = yes.ToUpper();
			else yes = char.ToUpper(yes[0]).ToString() + yes.Substring(1);

            return Enum.TryParse(yes, out KeyCode keyCode) ? keyCode : KeyCode.LeftAlt;
        }

		public static bool GetKey(KeyCode key, bool control = false, bool shift = false)
		{
			bool controlFlag = !control;
			bool shiftFlag = !shift;
			if (control && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
				controlFlag = true;
			if (shift && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
				shiftFlag = true;
			return controlFlag && shiftFlag && Input.GetKey(key);
		}

		public static bool GetKeyDown(KeyCode key, bool control = false, bool shift = false)
		{
			bool controlFlag = !control;
			bool shiftFlag = !shift;
			if (control && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
				controlFlag = true;
			if (shift && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
				shiftFlag = true;
			return controlFlag && shiftFlag && Input.GetKeyDown(key);
		}

		public static bool GetKeyUp(KeyCode key, bool control = false, bool shift = false)
		{
			bool controlFlag = !control;
			bool shiftFlag = !shift;
			if (control && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
				controlFlag = true;
			if (shift && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
				shiftFlag = true;
			return controlFlag && shiftFlag && Input.GetKeyUp(key);
		}

		public static bool GetMouseButtonDown(int button, bool control = false, bool shift = false)
		{
			bool controlFlag = !control;
			bool shiftFlag = !shift;
			if (control && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
				controlFlag = true;
			if (shift && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
				shiftFlag = true;
			return controlFlag && shiftFlag && Input.GetMouseButtonDown(button);
		}

		public static float GetAxis(string axis, bool control = false, bool shift = false)
		{
			bool controlFlag = !control;
			bool shiftFlag = !shift;
			if (control && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
				controlFlag = true;
			if (shift && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
				shiftFlag = true;
			if (controlFlag && shiftFlag)
				return Input.GetAxis(axis);
			return 0f;
		}
    }
}
