namespace BuildReportTool
{
    /// <summary>
    /// Represents one entry in an asset list.
    /// </summary>
    [System.Serializable]
    public class SizePart
    {
        /// <summary>
        /// The filename with path, but relative to project's Assets folder
        /// </summary>
        public string Name;

        // -----------------------------------

        /// <summary>
        /// How much the asset takes up space in the final build, in percentage.
        /// Value will be from the editor log if possible. If not, it will be calculated manually.
        /// </summary>
        public double Percentage;

        // -----------------------------------

        /// <summary>
        /// For Unused Assets, this is the raw file size as existing in the assets folder, expressed in human-readable format.
        /// For Used Assets, this is the size upon being built, as found in the Editor log.
        /// </summary>
        public string Size;

        /// <summary>
        /// The <see cref="Size"/> converted into bytes.
        /// </summary>
        public long SizeBytes = -1;

        // same as getting the `Size` but since we now have two size types,
        // for consistency, we now refer to the size as either RawSize and ImportedSize
        public string RawSize { get => this.Size; set => this.Size = value; }

        public long RawSizeBytes { get => this.SizeBytes; set => this.SizeBytes = value; }

        // -----------------------------------

        /// <summary>
        /// The file size as imported into Unity, expressed in human-readable format.
        /// If this SizePart is for an asset that has no imported size (e.g. built-in asset)
        /// this will be empty.
        /// </summary>
        public string ImportedSize;

        /// <summary>
        /// The imported file size, expressed in bytes.
        /// </summary>
        public long ImportedSizeBytes;

        // -----------------------------------

        /// <summary>
        /// For Used Assets, this is the file size as existing in the assets folder, expressed in human-readable format.
        /// </summary>
        public string SizeInAssetsFolder;

        /// <summary>
        /// The <see cref="SizeInAssetsFolder"/> in bytes
        /// </summary>
        public long SizeInAssetsFolderBytes = -1;

        // -----------------------------------

        /// <summary>
        /// In cases where we don't have exact values of file size (we just got it from
        /// editor log as string, which was converted to readable format already).
        ///
        /// Expressed in bytes (but with fractions because of the inaccuracies).
        ///
        /// This applies to the "Used Assets" list
        /// </summary>
        public double DerivedSize;

        // -----------------------------------

        /// <summary>
        /// Helper function to get the proper raw file size
        /// </summary>
        public double UsableSize
        {
            get
            {
                if (this.DerivedSize > 0) return this.DerivedSize;

                if (this.SizeBytes > 0) return this.SizeBytes;

                return this.ImportedSizeBytes;
            }
        }

        /// <summary>
        /// Return value of imported size, but if unavailable, will return raw size instead.
        /// </summary>
        public double ImportedSizeOrRawSize
        {
            get
            {
                if (this.ImportedSizeBytes > 0) return this.ImportedSizeBytes;

                if (this.DerivedSize > 0) return this.DerivedSize;

                if (this.SizeBytes > 0) return this.SizeBytes;

                return 0;
            }
        }

        // -----------------------------------

        public bool IsTotal => this.Name == "Complete size";

        public bool IsStreamingAssets => this.Name == "Streaming Assets";

        public void SetNameToStreamingAssets()
        {
            this.Name = "Streaming Assets";
        }

        // -----------------------------------

        private string _auxTextData;

        public string GetTextAuxData()
        {
            return this._auxTextData;
        }

        public void SetTextAuxData(string newTextAuxData)
        {
            this._auxTextData = newTextAuxData;
        }

        private int _auxIntData;

        public int GetIntAuxData()
        {
            return this._auxIntData;
        }

        public void SetIntAuxData(int newIntAuxData)
        {
            this._auxIntData = newIntAuxData;
        }

        private float _auxFloatData;

        public float GetFloatAuxData()
        {
            return this._auxFloatData;
        }

        public void SetFloatAuxData(float newFloatAuxData)
        {
            this._auxFloatData = newFloatAuxData;
        }
    }
} // namespace BuildReportTool