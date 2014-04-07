namespace Soheil.Core
{
    public class ColumnInfo
    {
        public ColumnInfo(string name, string header, int index, bool readOnly = false)
        {
            Name = name; 
            Header = header;
            Index = index;
            ReadOnly = readOnly;
        }
        public ColumnInfo(string name, int index, bool readOnly = false)
        {
            Name = name; 
            Header = "txt" + name;
            Index = index;
            ReadOnly = readOnly;
        }
        public string Name { get; set; }
        public string Header { get; set; }
        public bool ReadOnly { get; set; }

        public int Index { get; set; }
    }
}
