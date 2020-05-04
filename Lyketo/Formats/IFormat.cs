namespace Lyketo.Formats
{
    /// <summary>
    /// Defines an interface for any possible format to convert.
    /// </summary>
    public interface IFormat
    {
        /// <summary>
        /// Initializes the format (for example it creates or loads the basic file).
        /// </summary>
        /// <param name="fileName">The name of the file to initialize.</param>
        /// <param name="write">If the format should be written.</param>
        /// <returns>True if the operation succeeded, otherwise false.</returns>
        bool Initialize(string fileName, bool write);

        /// <summary>
        /// Finalizes the format process, this function writes any changes to the disk.
        /// </summary>
        /// <returns>True if the operation succeeded, otherwise false.</returns>
        bool Finalize();

        /// <summary>
        /// Moves to the next entry, either next Mob information or next Item information.
        /// </summary>
        void Next();

        /// <summary>
        /// Gets all the data loaded from the format.
        /// </summary>
        /// <returns>A number containing all the data loaded.</returns>
        int Count();

        /// <summary>
        /// Gets the filter name.
        /// 
        /// Like: XML file (*.xml)|*.xml
        /// </summary>
        /// <returns>A filter string.</returns>
        string FilterName();

        #region Get functions
        string GetString(string field, int len);
        uint GetUint32(string field);
        int GetInt32(string field);
        short GetInt16(string field);
        ushort GetUInt16(string field);
        float GetFloat(string field);
        double GetDouble(string field);
        char GetInt8(string field);
        byte GetUInt8(string field);
        long GetInt64(string field);
        ulong GetUInt64(string field);
        #endregion

        #region Set functions
        void Set(string field, string value, int len);
        void Set(string field, uint value);
        void Set(string field, int value);
        void Set(string field, short value);
        void Set(string field, ushort value);
        void Set(string field, float value);
        void Set(string field, double value);
        void Set(string field, char value);
        void Set(string field, byte value);
        void Set(string field, long value);
        void Set(string field, ulong value);
        #endregion
    }
}
