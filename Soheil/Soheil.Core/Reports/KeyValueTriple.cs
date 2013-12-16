namespace Soheil.Core.Reports
{
    public struct KeyValueTriple<TPrimaryKey, TSecondaryKey,TValue>
    {
        public TPrimaryKey PrimaryKey { get; set; }
        public TSecondaryKey SecondaryKey { get; set; }
        public TValue Value { get; set; }

        public KeyValueTriple(TPrimaryKey primaryKey, TSecondaryKey secondaryKey,TValue value) : this()
        {
            PrimaryKey = primaryKey;
            SecondaryKey = secondaryKey;
            Value = value;
        }
    }
}
