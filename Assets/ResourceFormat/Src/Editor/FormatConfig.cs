using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ResourceFormat
{
    public static class FormatConfig
    {
        public const string ResourceRootPath = "Assets";
        public const string ConfigDataRoot = "Assets/ResourceFormat/Data";
        public const string DataSuffix = ".txt";
        public const string DataPathLoader = ConfigDataRoot + "/PathLoader" + DataSuffix;
        public const string TextureImportPath = ConfigDataRoot + "/TextureImportData" + DataSuffix;
        public const string ModelImportPath = ConfigDataRoot + "/ModelImportData" + DataSuffix;
        public const string AnimationImportPath = ConfigDataRoot + "/AnimationImportData" + DataSuffix;
    }
}