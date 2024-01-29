using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace SpectralDepths.Tools
{
	[CustomEditor(typeof(PLAspectRatioSafeZones), true)]
	public class PLScreenshotEditor : Editor
	{
		static string FolderName = "Screenshots";

		[MenuItem("Tools/Spectral Depths/Screenshot/Take Screenshot Real Size", false, 801)]
		public static void MenuScreenshotSize1()
		{
			string savePath = TakeScreenCaptureScreenshot(1);
		}
		[MenuItem("Tools/Spectral Depths/Screenshot/Take Screenshot Size x2", false, 802)]
		public static void MenuScreenshotSize2()
		{
			string savePath = TakeScreenCaptureScreenshot(2);
		}
		[MenuItem("Tools/Spectral Depths/Screenshot/Take Screenshot Size x3 %k", false, 803)]
		public static void MenuScreenshotSize3()
		{
			string savePath = TakeScreenCaptureScreenshot(3);
		}

		protected static string TakeScreenCaptureScreenshot(int gameViewSizeMultiplier)
		{
			if (!Directory.Exists(FolderName))
			{
				Directory.CreateDirectory(FolderName);
			}

			float width = Screen.width * gameViewSizeMultiplier;
			float height = Screen.height * gameViewSizeMultiplier;
			string savePath = FolderName + "/screenshot_" + width + "x" + height + "_" + System.DateTime.Now.ToString("yyyy-PL-dd_HH-mm-ss") + ".png";

			ScreenCapture.CaptureScreenshot(savePath, gameViewSizeMultiplier);
			Debug.Log("[PLScreenshot] Screenshot taken with size multiplier of " + gameViewSizeMultiplier + " and saved at " + savePath);
			return savePath;
		}
	}
}