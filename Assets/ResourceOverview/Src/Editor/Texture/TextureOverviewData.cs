using EditorCommon;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ResourceFormat{
    public enum TextureOverviewMode
    {
        ReadWrite = 0,
        MipMap,
        Type,
        Size,
        WidthVSHeight,
        AndroidFormat,
        iOSFormat,

        Count,
    }

    public class TextureOverviewData
    {
        public int Count;
        public int Memory;
        public bool ReadWriteEnable;
        public bool MipmapEnable;
        public TextureImporterType ImportType;
        public TextureImporterFormat AndroidFormat;
        public TextureImporterFormat IosFormat;
        public int SizeIndex;
        public string SizeStr;
        public bool WidthAndHeight;
        public TextureOverviewMode Mode;

        public static TextureOverviewData CreateNew(TextureOverviewMode mode, TextureInfo texInfo)
        {
            TextureOverviewData retData = new TextureOverviewData();
            retData.Mode = mode;
            retData.ReadWriteEnable = texInfo.ReadWriteEnable;
            retData.MipmapEnable = texInfo.MipmapEnable;
            retData.ImportType = texInfo.ImportType;
            retData.AndroidFormat = texInfo.AndroidFormat;
            retData.IosFormat = texInfo.IosFormat;
            retData.WidthAndHeight = texInfo.Width == texInfo.Height;
            retData.SizeIndex = OverviewTableConst.GetTextureSizeIndex(texInfo.Width, texInfo.Height);
            retData.SizeStr = OverviewTableConst.TextureSizeStr[retData.SizeIndex];

            return retData;
        }

        public static void SwitchDataTableMode(TextureOverviewMode mode, TableView tableView)
        {
            float leftWide = 0.4f;
            tableView.ClearColumns();
            switch (mode)
            {
            case TextureOverviewMode.ReadWrite:
                tableView.AddColumn("ReadWriteEnable", "R/W Enable", leftWide);
                break;
            case TextureOverviewMode.MipMap:
                tableView.AddColumn("MipmapEnable", "MipmapEnable", leftWide);
                break;
            case TextureOverviewMode.Type:
                tableView.AddColumn("ImportType", "TextureType", leftWide);
                break;
            case TextureOverviewMode.Size:
                tableView.AddColumn("SizeStr", "Size Range", leftWide);
                break;
            case TextureOverviewMode.WidthVSHeight:
                tableView.AddColumn("WidthAndHeight", "Width VS Height", leftWide);
                break;
            case TextureOverviewMode.AndroidFormat:
                tableView.AddColumn("AndroidFormat", "AndroidFormat", leftWide);
                break;
            case TextureOverviewMode.iOSFormat:
                tableView.AddColumn("IosFormat", "iOSFormat", leftWide);
                break;
            }
            tableView.AddColumn("Count", "Count", (1.0f - leftWide) / 2.0f);
            tableView.AddColumn("Memory", "Memory", (1.0f - leftWide) / 2.0f, TextAnchor.MiddleCenter, "<fmt_bytes>");
        }

        public bool IsMatch(TextureInfo texInfo)
        {
            switch (Mode)
            {
            case TextureOverviewMode.ReadWrite:
                return ReadWriteEnable == texInfo.ReadWriteEnable;
            case TextureOverviewMode.MipMap:
                return MipmapEnable == texInfo.MipmapEnable;
            case TextureOverviewMode.Type:
                return ImportType == texInfo.ImportType;
            case TextureOverviewMode.Size:
                return SizeIndex == OverviewTableConst.GetTextureSizeIndex(texInfo.Width, texInfo.Height);
            case TextureOverviewMode.WidthVSHeight:
                return WidthAndHeight == (texInfo.Width == texInfo.Height);
            case TextureOverviewMode.AndroidFormat:
                return AndroidFormat == texInfo.AndroidFormat;
            case TextureOverviewMode.iOSFormat:
                return IosFormat == texInfo.IosFormat;
            }
            return false;
        }

        public void AddObject(TextureInfo texInfo)
        {
            Count = Count + 1;
            if (Mode == TextureOverviewMode.AndroidFormat)
                Memory += texInfo.AndroidSize;
            else if (Mode == TextureOverviewMode.iOSFormat)
                Memory += texInfo.IosSize;
            else 
                Memory += texInfo.MemSize;
            m_objects.Add(texInfo);
        }

        public List<object> GetObjects()
        {
            return m_objects;
        }

        protected List<object> m_objects = new List<object>();
    }
}