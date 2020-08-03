using AssetStudio;

namespace AssetStudioConsole
{
    public class AssetItem
    {
        public Object Asset;
        public SerializedFile SourceFile;
        public long FullSize;
        public ClassIDType Type;
        public string TypeString;

        public string Extension;
        public string InfoText;
        public string UniqueID;
        public string Text;

        public AssetItem(Object asset)
        {
            Asset = asset;
            SourceFile = asset.assetsFile;
            FullSize = asset.byteSize;
            Type = asset.type;
            TypeString = Type.ToString();
        }

        public void Print()
        {
            Logger.Info($"{this.SourceFile.fileName} -- {this.TypeString} - {this.Text}");
        }
    }
}
