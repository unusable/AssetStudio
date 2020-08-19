using System;
using System.Collections.Generic;
using System.IO;
using AssetStudio;

namespace AssetStudioConsole
{

    internal static class Properties
    {
        public static class Settings
        {
            public static Dictionary<string, object> Default = new Dictionary<string, object>();
        }
    }

    class Program
    {

        static void Main(string[] args)
        {
            string basePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../.."));
            Console.WriteLine(basePath);
            string inputPath = Path.Combine(basePath, "Sample/FitnessGym/assets/bin/Data");
            string outputPath = Path.Combine(basePath, "Sample/FitnessGym_output");
            //string inputPath = Path.Combine(basePath, "Sample/Market/assets/bin/Data");
            //string outputPath = Path.Combine(basePath, "Sample/Market_output");
            //string inputPath = Path.Combine(basePath, "Sample/ThemePark/assets/bin/Data");
            //string outputPath = Path.Combine(basePath, "Sample/ThemePark_output");
            //string inputPath = Path.Combine(basePath, "Sample/IdleCourier/assets/bin/Data");
            //string outputPath = Path.Combine(basePath, "Sample/IdleCourier_output");
            //string inputPath = Path.Combine(basePath, "Sample/Farmville3/assets/bin/Data");
            //string outputPath = Path.Combine(basePath, "Sample/Farmville3_output");
            //string[] inputFiles = new string[] { Path.Combine(basePath, "Sample/tmp/model_assets") };
            //string outputPath = Path.Combine(basePath, "Sample/tmp_output");
            MakeSettings();
            Logger.Default = new ConsoleLogger();
            AssetExporter exporter = new AssetExporter();
            exporter.Export(inputPath, outputPath);
            //exporter.ExportFiles(inputFiles, outputPath);
            Console.WriteLine("Completed!");
            //Console.ReadKey();
        }

        static void MakeSettings()
        {
            /*
  <Settings>
    <Setting Name="displayAll" Type="System.Boolean" Scope="User">
      <Value Profile="(Default)">False</Value>
    </Setting>
    <Setting Name="enablePreview" Type="System.Boolean" Scope="User">
      <Value Profile="(Default)">True</Value>
    </Setting>
    <Setting Name="displayInfo" Type="System.Boolean" Scope="User">
      <Value Profile="(Default)">True</Value>
    </Setting>
    <Setting Name="openAfterExport" Type="System.Boolean" Scope="User">
      <Value Profile="(Default)">True</Value>
    </Setting>
    <Setting Name="assetGroupOption" Type="System.Int32" Scope="User">
      <Value Profile="(Default)">0</Value>
    </Setting>
    <Setting Name="convertTexture" Type="System.Boolean" Scope="User">
      <Value Profile="(Default)">True</Value>
    </Setting>
    <Setting Name="convertAudio" Type="System.Boolean" Scope="User">
      <Value Profile="(Default)">True</Value>
    </Setting>
    <Setting Name="convertType" Type="System.String" Scope="User">
      <Value Profile="(Default)">PNG</Value>
    </Setting>
    <Setting Name="displayOriginalName" Type="System.Boolean" Scope="User">
      <Value Profile="(Default)">False</Value>
    </Setting>
    <Setting Name="eulerFilter" Type="System.Boolean" Scope="User">
      <Value Profile="(Default)">True</Value>
    </Setting>
    <Setting Name="filterPrecision" Type="System.Decimal" Scope="User">
      <Value Profile="(Default)">0.25</Value>
    </Setting>
    <Setting Name="exportAllNodes" Type="System.Boolean" Scope="User">
      <Value Profile="(Default)">True</Value>
    </Setting>
    <Setting Name="exportSkins" Type="System.Boolean" Scope="User">
      <Value Profile="(Default)">True</Value>
    </Setting>
    <Setting Name="exportAnimations" Type="System.Boolean" Scope="User">
      <Value Profile="(Default)">True</Value>
    </Setting>
    <Setting Name="boneSize" Type="System.Decimal" Scope="User">
      <Value Profile="(Default)">10</Value>
    </Setting>
    <Setting Name="fbxVersion" Type="System.Int32" Scope="User">
      <Value Profile="(Default)">3</Value>
    </Setting>
    <Setting Name="fbxFormat" Type="System.Int32" Scope="User">
      <Value Profile="(Default)">0</Value>
    </Setting>
    <Setting Name="scaleFactor" Type="System.Decimal" Scope="User">
      <Value Profile="(Default)">1</Value>
    </Setting>
    <Setting Name="exportBlendShape" Type="System.Boolean" Scope="User">
      <Value Profile="(Default)">True</Value>
    </Setting>
    <Setting Name="castToBone" Type="System.Boolean" Scope="User">
      <Value Profile="(Default)">False</Value>
    </Setting>
  </Settings>
            */
            Properties.Settings.Default["displayAll"] = false;
            Properties.Settings.Default["enablePreview"] = true;
            Properties.Settings.Default["displayInfo"] = true;
            Properties.Settings.Default["openAfterExport"] = true;
            Properties.Settings.Default["assetGroupOption"] = 0;
            Properties.Settings.Default["convertTexture"] = true;
            Properties.Settings.Default["convertAudio"] = true;
            Properties.Settings.Default["convertType"] = "PNG";
            Properties.Settings.Default["displayOriginalName"] = true;
            Properties.Settings.Default["eulerFilter"] = true;
            Properties.Settings.Default["filterPrecision"] = new System.Decimal(0.25f);
            Properties.Settings.Default["exportAllNodes"] = true;
            Properties.Settings.Default["exportSkins"] = true;
            Properties.Settings.Default["exportAnimations"] = true;
            Properties.Settings.Default["boneSize"] = new System.Decimal(10);
            Properties.Settings.Default["fbxVersion"] = 3;
            Properties.Settings.Default["fbxFormat"] = 0;
            Properties.Settings.Default["scaleFactor"] = new System.Decimal(1);
            Properties.Settings.Default["exportBlendShape"] = true;
            Properties.Settings.Default["castToBone"] = false;
        }
    }
}
