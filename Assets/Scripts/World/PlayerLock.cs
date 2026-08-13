namespace SeriousGameBPD.World
{
    public static class PlayerLock
    {
        static int _locks;

        public static bool IsLocked => _locks > 0;

        public static void Push() => _locks++;

        public static void Pop()
        {
            if (_locks > 0)
                _locks--;
        }

        public static void Reset() => _locks = 0;
    }
}
