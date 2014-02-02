namespace Soheil.Core
{
    public class ColumnInfo
    {
        public ColumnInfo(string name, string header, bool readOnly = false)
        {
            Name = name; 
            Header = header; 
            ReadOnly = readOnly;
        }
        public ColumnInfo(string name, bool readOnly = false)
        {
            Name = name; 
            Header = "txt" + name; 
            ReadOnly = readOnly;
        }
        public string Name { get; set; }
        public string Header { get; set; }
        public bool ReadOnly { get; set; }
    }
}
