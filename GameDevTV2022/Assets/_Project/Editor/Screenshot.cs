using System;
using UnityEditor;
using UnityEngine;

namespace Common.Editor
{
    public class Screenshot
    {
        [MenuItem("Tools/Take Screenshot")]
        internal static void TakeScreenshot()
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string filename = $"screenshot_{date}.png";
            Debug.Log($"Screenshot {filename}");
            ScreenCapture.CaptureScreenshot(filename);
        }
    }
}
