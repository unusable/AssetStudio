using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssetStudio
{
    public class FileIdentifier
    {
        public Guid guid;
        public int type; //enum { kNonAssetType = 0, kDeprecatedCachedAssetType = 1, kSerializedAssetType = 2, kMetaAssetType = 3 };
        public string pathName;

        //custom
        public string fileName;

        public string Dump()
        {
            return string.Format("{0} {1} {2} {3}", guid.ToString(), type, fileName, pathName);
        }

    }
}
