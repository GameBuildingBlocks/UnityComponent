using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ResourceFormat
{
    public class TableConst
    {
        public static float TopBarHeight = 25;
        public static int InspectorWidth = 400;
        public static int TableBorder = 10;
        public static float SplitterRatio = 0.3f;

        public static string CurrentMode = "Resource Format Current Mode";
        public static string[] Modes = new string[] { "Texture", "Model", "Animation" };

        public static string[] TextureType = { "Default", "NormalMap", "Lightmap", "Sprite(2D and UI)" };
        public static TextureImporterType[] ImporterType = { TextureImporterType.Default, TextureImporterType.NormalMap, TextureImporterType.Lightmap, TextureImporterType.Sprite };

        public static string[] TextureShape = { "2D", "Cube" };
        public static TextureImporterShape[] ImporterShape = { TextureImporterShape.Texture2D, TextureImporterShape.TextureCube };

        public static string[] AlpaMode = { "FromTexture", "None" };
        public static int[] MaxSizeInt = { -1, 128, 256, 512, 1024, 2048 };
        public static string[] MaxSize = { "-1", "128", "256", "512", "1024", "2048" };
        public static string[] AndoridFormat = { "RGB24", "ETC_RGB4", "ETC2_RGB" };
        public static string[] IosFormat = { "RGB24", "PVRTC_RGB4" };
        public static string[] MatierlMode = { "None", "SplitTexture", "HalfTexture" };
        public static string[] AnimationType = { "None", "Legacy", "Generic", "Human" };
        public static string[] AnimationCompression = { "Off", "KeyframeReduction", "KeyframeReductionAndCompression", "Optimal" };
        public static string[] MeshCompression = { "Off", "Low", "Medium", "High" };


        public static TextureImporterFormat[] AndroidImporterFormat = { TextureImporterFormat.RGB24, TextureImporterFormat.ETC_RGB4, TextureImporterFormat.ETC2_RGB4 };
        public static TextureImporterFormat[] IosImporterFormat = { TextureImporterFormat.RGB24, TextureImporterFormat.PVRTC_RGB4 };
    }

    public class TableStyles
    {
        public static GUIStyle Toolbar = "Toolbar";
        public static GUIStyle ToolbarButton = "ToolbarButton";

        public static GUIStyle TextField = "TextField";
    }
}