using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;
using LitJson;

namespace ResourceFormat {
    public class ConfigData {
        public static List<TextureImportData> TextureImportData {
            get {
                if (m_texSelectData == null) {
                    m_texSelectData = EditorCommon.LoadJsonData<List<TextureImportData>>(RFConfig.TextureSelectFilePath);
                }

                if (m_texSelectData == null) {
                    m_texSelectData = new List<TextureImportData>();
                }

                return m_texSelectData;
            }
        }

        public static List<ModelImportData> ModelSelectData {
            get {
                if (m_modelSelectData == null) {
                    m_modelSelectData = EditorCommon.LoadJsonData<List<ModelImportData>>(RFConfig.ModelSelectFilePath);
                }

                if (m_modelSelectData == null) {
                    m_modelSelectData = new List<ModelImportData>();
                }

                return m_modelSelectData;
            }
        }

        public static List<AnimationImportData> AnimationImportData {
            get {
                if (m_aniSelectData == null) {
                    m_aniSelectData = EditorCommon.LoadJsonData<List<AnimationImportData>>(RFConfig.AnimationSelectFilePath);
                }

                if (m_aniSelectData == null) {
                    m_aniSelectData = new List<AnimationImportData>();
                }

                return m_aniSelectData;
            }
        }

        public static void SaveData() {
            if (m_texSelectData != null) {
                EditorCommon.SaveJsonData<List<TextureImportData>>(m_texSelectData, RFConfig.TextureSelectFilePath);
            }
            if (m_modelSelectData != null) {
                EditorCommon.SaveJsonData<List<ModelImportData>>(m_modelSelectData, RFConfig.ModelSelectFilePath);
            }
            if (m_aniSelectData != null) {
                EditorCommon.SaveJsonData<List<AnimationImportData>>(m_aniSelectData, RFConfig.AnimationSelectFilePath);
            }
        }

        private static List<AnimationImportData> m_aniSelectData = null;
        private static List<ModelImportData> m_modelSelectData = null;
        private static List<TextureImportData> m_texSelectData = null;
    }
}