namespace FFXIV_RotationHelper.StrongType
{
    public readonly struct DBIdx
    {
        private readonly int v;

        public DBIdx(int value)
        {
            v = value;
        }

        public override bool Equals(object o)
        {
            return o != null && v == ((DBIdx)o).v;
        }

        public override int GetHashCode()
        {
            return v;
        }

        public static bool operator ==(DBIdx v1, DBIdx v2) { return v1.v == v2.v; }
        public static bool operator !=(DBIdx v1, DBIdx v2) { return v1.v != v2.v; }

        public static explicit operator int(DBIdx dbIdx) { return dbIdx.v; }
        public static explicit operator DBIdx(int value) { return new DBIdx(value); }

        public static explicit operator GameIdx(DBIdx dbIdx) { return (GameIdx)dbIdx.v; }
    }
}
