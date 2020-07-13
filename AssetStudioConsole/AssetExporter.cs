//using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AssetStudio;
using static AssetStudioConsole.Exporter;
using Object = AssetStudio.Object;

namespace AssetStudioConsole
{
    public enum ExportType
    {
        Convert,
        Raw,
        Dump
    }

    public class AssetExporter
    {

        private AssetsManager assetsManager = new AssetsManager();
        private List<AssetItem> exportableAssets = new List<AssetItem>();

        public void Export(string inputFolder, string outputFolder)
        {
            if(!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }
            this.assetsManager.LoadFolder(inputFolder);

            var productName = string.Empty;
            var tempDic = new Dictionary<Object, AssetItem>();
            BuildAssetList(tempDic, true, true, out productName);
            ExportAssets(outputFolder, this.exportableAssets, 0, true, ExportType.Convert);

        }

        public AssetExporter()
        {
        }

        public void BuildAssetList(Dictionary<Object, AssetItem> tempDic, bool displayAll, bool displayOriginalName, out string productName)
        {
            //StatusStripUpdate("Building asset list...");
            exportableAssets.Clear();

            productName = string.Empty;
            var assetsNameHash = new HashSet<string>();
            var progressCount = assetsManager.assetsFileList.Sum(x => x.Objects.Count);
            int j = 0;
            Progress.Reset();
            foreach (var assetsFile in assetsManager.assetsFileList)
            {
                var tempExportableAssets = new List<AssetItem>();
                AssetBundle ab = null;
                foreach (var asset in assetsFile.Objects.Values)
                {
                    var assetItem = new AssetItem(asset);
                    tempDic.Add(asset, assetItem);
                    assetItem.UniqueID = " #" + j;
                    var exportable = false;
                    switch (asset)
                    {
                        case GameObject m_GameObject:
                            assetItem.Text = m_GameObject.m_Name;
                            break;
                        case Texture2D m_Texture2D:
                            if (!string.IsNullOrEmpty(m_Texture2D.m_StreamData?.path))
                                assetItem.FullSize = asset.byteSize + m_Texture2D.m_StreamData.size;
                            assetItem.Text = m_Texture2D.m_Name;
                            exportable = true;
                            break;
                        case AudioClip m_AudioClip:
                            if (!string.IsNullOrEmpty(m_AudioClip.m_Source))
                                assetItem.FullSize = asset.byteSize + m_AudioClip.m_Size;
                            assetItem.Text = m_AudioClip.m_Name;
                            exportable = true;
                            break;
                        case VideoClip m_VideoClip:
                            if (!string.IsNullOrEmpty(m_VideoClip.m_OriginalPath))
                                assetItem.FullSize = asset.byteSize + (long)m_VideoClip.m_Size;
                            assetItem.Text = m_VideoClip.m_Name;
                            exportable = true;
                            break;
                        case Shader m_Shader:
                            assetItem.Text = m_Shader.m_ParsedForm?.m_Name ?? m_Shader.m_Name;
                            exportable = true;
                            break;
                        case Mesh _:
                        case TextAsset _:
                        case AnimationClip _:
                        case Font _:
                        case MovieTexture _:
                        case Sprite _:
                            assetItem.Text = ((NamedObject)asset).m_Name;
                            exportable = true;
                            break;
                        case Animator m_Animator:
                            if (m_Animator.m_GameObject.TryGet(out var gameObject))
                            {
                                assetItem.Text = gameObject.m_Name;
                            }
                            exportable = true;
                            break;
                        case MonoBehaviour m_MonoBehaviour:
                            if (m_MonoBehaviour.m_Name == "" && m_MonoBehaviour.m_Script.TryGet(out var m_Script))
                            {
                                assetItem.Text = m_Script.m_ClassName;
                            }
                            else
                            {
                                assetItem.Text = m_MonoBehaviour.m_Name;
                            }
                            exportable = true;
                            break;
                        case PlayerSettings m_PlayerSettings:
                            productName = m_PlayerSettings.productName;
                            break;
                        case AssetBundle m_AssetBundle:
                            ab = m_AssetBundle;
                            assetItem.Text = ab.m_Name;
                            break;
                        case NamedObject m_NamedObject:
                            assetItem.Text = m_NamedObject.m_Name;
                            break;
                    }
                    if (assetItem.Text == "")
                    {
                        assetItem.Text = assetItem.TypeString + assetItem.UniqueID;
                    }
                    //assetItem.SubItems.AddRange(new[] { assetItem.TypeString, assetItem.FullSize.ToString() });
                    //处理同名文件
                    if (!assetsNameHash.Add((assetItem.TypeString + assetItem.Text).ToUpper()))
                    {
                        assetItem.Text += assetItem.UniqueID;
                    }
                    //处理非法文件名
                    assetItem.Text = FixFileName(assetItem.Text);
                    if (displayAll)
                    {
                        exportable = true;
                    }
                    if (exportable)
                    {
                        tempExportableAssets.Add(assetItem);
                    }

                    Progress.Report(++j, progressCount);
                }
                if (displayOriginalName && ab != null)
                {
                    foreach (var item in tempExportableAssets)
                    {
                        var originalPath = ab.m_Container.FirstOrDefault(y => y.Value.asset.m_PathID == item.Asset.m_PathID).Key;
                        if (!string.IsNullOrEmpty(originalPath))
                        {
                            var extension = Path.GetExtension(originalPath);
                            if (!string.IsNullOrEmpty(extension) && item.Type == ClassIDType.TextAsset)
                            {
                                item.Extension = extension;
                            }

                            item.Text = Path.GetDirectoryName(originalPath) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(originalPath);
                            if (!assetsNameHash.Add((item.TypeString + item.Text).ToUpper()))
                            {
                                item.Text += item.UniqueID;
                            }
                        }
                    }
                }
                exportableAssets.AddRange(tempExportableAssets);
                tempExportableAssets.Clear();
            }

            //visibleAssets = exportableAssets;
            assetsNameHash.Clear();
        }

        public static void ExportAssets(string savePath, List<AssetItem> toExportAssets, int assetGroupSelectedIndex, bool openAfterExport, ExportType exportType)
        {
            //ThreadPool.QueueUserWorkItem(state =>
            //{
                //Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

                int toExportCount = toExportAssets.Count;
                int exportedCount = 0;
                int i = 0;
                Progress.Reset();
                foreach (var asset in toExportAssets)
                {
                    var exportpath = savePath + Path.DirectorySeparatorChar;
                    if (assetGroupSelectedIndex == 1)
                    {
                        exportpath += Path.GetFileNameWithoutExtension(asset.SourceFile.fullName) + "_export" + Path.DirectorySeparatorChar;
                    }
                    else if (assetGroupSelectedIndex == 0)
                    {
                        exportpath = savePath + Path.DirectorySeparatorChar + asset.TypeString + Path.DirectorySeparatorChar;
                    }
                    //StatusStripUpdate($"Exporting {asset.TypeString}: {asset.Text}");
                    try
                    {
                        switch (exportType)
                        {
                            case ExportType.Raw:
                                if (ExportRawFile(asset, exportpath))
                                {
                                    exportedCount++;
                                }
                                break;
                            case ExportType.Dump:
                                if (ExportDumpFile(asset, exportpath))
                                {
                                    exportedCount++;
                                }
                                break;
                            case ExportType.Convert:
                                switch (asset.Type)
                                {
                                    case ClassIDType.Texture2D:
                                        if (ExportTexture2D(asset, exportpath))
                                        {
                                            exportedCount++;
                                        }
                                        break;
                                    case ClassIDType.AudioClip:
                                        if (ExportAudioClip(asset, exportpath))
                                        {
                                            exportedCount++;
                                        }
                                        break;
                                    case ClassIDType.Shader:
                                        if (ExportShader(asset, exportpath))
                                        {
                                            exportedCount++;
                                        }
                                        break;
                                    case ClassIDType.TextAsset:
                                        if (ExportTextAsset(asset, exportpath))
                                        {
                                            exportedCount++;
                                        }
                                        break;
                                    case ClassIDType.MonoBehaviour:
                                        if (ExportMonoBehaviour(asset, exportpath))
                                        {
                                            exportedCount++;
                                        }
                                        break;
                                    case ClassIDType.Font:
                                        if (ExportFont(asset, exportpath))
                                        {
                                            exportedCount++;
                                        }
                                        break;
                                    case ClassIDType.Mesh:
                                        if (ExportMesh(asset, exportpath))
                                        {
                                            exportedCount++;
                                        }
                                        break;
                                    case ClassIDType.VideoClip:
                                        if (ExportVideoClip(asset, exportpath))
                                        {
                                            exportedCount++;
                                        }
                                        break;
                                    case ClassIDType.MovieTexture:
                                        if (ExportMovieTexture(asset, exportpath))
                                        {
                                            exportedCount++;
                                        }
                                        break;
                                    case ClassIDType.Sprite:
                                        if (ExportSprite(asset, exportpath))
                                        {
                                            exportedCount++;
                                        }
                                        break;
                                    case ClassIDType.Animator:
                                        if (ExportAnimator(asset, exportpath))
                                        {
                                            exportedCount++;
                                        }
                                        break;
                                    case ClassIDType.AnimationClip:
                                        break;
                                    default:
                                        if (ExportRawFile(asset, exportpath))
                                        {
                                            exportedCount++;
                                        }
                                        break;
                                }
                                break;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        //MessageBox.Show($"Export {asset.Type}:{asset.Text} error\r\n{ex.Message}\r\n{ex.StackTrace}");
                    }

                    Progress.Report(++i, toExportCount);
                }

                var statusText = exportedCount == 0 ? "Nothing exported." : $"Finished exporting {exportedCount} assets.";

                if (toExportCount > exportedCount)
                {
                    statusText += $" {toExportCount - exportedCount} assets skipped (not extractable or files already exist)";
                }

                //StatusStripUpdate(statusText);

                if (openAfterExport && exportedCount > 0)
                {
                    Process.Start(savePath);
                }
            //});
        }

        public static string FixFileName(string str)
        {
            if (string.IsNullOrEmpty(str)) return string.Empty;
            if (str.Length >= 260) return Path.GetRandomFileName();
            return Path.GetInvalidFileNameChars().Aggregate(str, (current, c) => current.Replace(c, '_'));
        }

        public static string GetScriptString(ObjectReader reader)
        {
            //if (scriptDumper == null)
            //{
            //    var openFolderDialog = new OpenFolderDialog();
            //    openFolderDialog.Title = "Select Assembly Folder";
            //    if (openFolderDialog.ShowDialog() == DialogResult.OK)
            //    {
            //        scriptDumper = new ScriptDumper(openFolderDialog.Folder);
            //    }
            //    else
            //    {
            //        scriptDumper = new ScriptDumper();
            //    }
            //}

            //return scriptDumper.DumpScript(reader);
            return string.Empty;
        }

    }



}
