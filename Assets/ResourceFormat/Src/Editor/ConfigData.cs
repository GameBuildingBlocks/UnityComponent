using System.Collections.Generic;
using EditorCommon;

namespace ResourceFormat
{
    public class ConfigData
    {
        public static List<TextureImportData> TextureImportData
        {
            get
            {
                if (m_texSelectData == null)
                {
                    m_texSelectData = EditorTool.LoadJsonData<List<TextureImportData>>(FormatConfig.TextureImportPath);
                }

                if (m_texSelectData == null)
                {
                    m_texSelectData = new List<TextureImportData>();
                }

                return m_texSelectData;
            }
        }

        public static List<ModelImportData> ModelSelectData
        {
            get
            {
                if (m_modelSelectData == null)
                {
                    m_modelSelectData = EditorTool.LoadJsonData<List<ModelImportData>>(FormatConfig.ModelImportPath);
                }

                if (m_modelSelectData == null)
                {
                    m_modelSelectData = new List<ModelImportData>();
                }

                return m_modelSelectData;
            }
        }

        public static List<AnimationImportData> AnimationImportData
        {
            get
            {
                if (m_aniSelectData == null)
                {
                    m_aniSelectData = EditorTool.LoadJsonData<List<AnimationImportData>>(FormatConfig.AnimationImportPath);
                }

                if (m_aniSelectData == null)
                {
                    m_aniSelectData = new List<AnimationImportData>();
                }

                return m_aniSelectData;
            }
        }

        public static void SaveData()
        {
            if (m_texSelectData != null)
            {
                EditorTool.SaveJsonData<List<TextureImportData>>(m_texSelectData, FormatConfig.TextureImportPath);
            }
            if (m_modelSelectData != null)
            {
                EditorTool.SaveJsonData<List<ModelImportData>>(m_modelSelectData, FormatConfig.ModelImportPath);
            }
            if (m_aniSelectData != null)
            {
                EditorTool.SaveJsonData<List<AnimationImportData>>(m_aniSelectData, FormatConfig.AnimationImportPath);
            }
        }

        private static List<AnimationImportData> m_aniSelectData = null;
        private static List<ModelImportData> m_modelSelectData = null;
        private static List<TextureImportData> m_texSelectData = null;
    }
}