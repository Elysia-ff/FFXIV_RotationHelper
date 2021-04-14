namespace FFXIV_RotationHelper.StrongType
{
    public readonly struct GameIdx
    {
        private readonly int v;

        public GameIdx(int value)
        {
            v = value;
        }

        public static explicit operator int(GameIdx gameIdx) { return gameIdx.v; }
        public static explicit operator GameIdx(int value) { return new GameIdx(value); }
    }
}
