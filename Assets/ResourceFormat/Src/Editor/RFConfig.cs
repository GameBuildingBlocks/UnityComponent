using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ResourceFormat {

    public static class RFConfig {
        public const string ResourceRootPath = "Assets/Packages";
        public const string ConfigDataRoot = "Assets/ResourceFormat/Data";
        public const string DataSuffix = ".txt";
        public const string DataPathLoader = ConfigDataRoot + "/PathLoader" + DataSuffix;
        public const string TextureSelectFilePath = ConfigDataRoot + "/TextureSelectFile" + DataSuffix;
        public const string MatierialSelectFilePath = ConfigDataRoot + "/MaterialSelectFile" + DataSuffix;
        public const string ModelSelectFilePath = ConfigDataRoot + "/ModelSelectFile" + DataSuffix;
        public const string AnimationSelectFilePath = ConfigDataRoot + "/AnimationSelectFile" + DataSuffix;
        public const string PathSelectFilePath = ConfigDataRoot + "/PathSelectFile" + DataSuffix;
        public const string SettingPath = ConfigDataRoot + "/Setting" + DataSuffix;
        public const string WhiteTexturePath = ConfigDataRoot + "/WhiteTexture.png";
    }
}