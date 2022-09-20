namespace MoogleEngine
{
    class ClosenessData : IComparable<ClosenessData>
    {
        public string FileName
        {
            get;
            private set;
        }
        public int Closeness
        {
            get;
            private set;
        }
        public ClosenessData(string fileName, int closeness)
        {
            FileName = fileName;
            Closeness = closeness;
        }

        public int CompareTo(ClosenessData other)
        {
            if (Closeness < other.Closeness) return 1;
            else if (Closeness > other.Closeness) return -1;
            else return 0;
        }
    }
}