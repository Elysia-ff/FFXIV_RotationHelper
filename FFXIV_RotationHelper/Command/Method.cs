namespace FFXIV_RotationHelper
{
    public class Method
    {
        public delegate void Del();

        private readonly Del del = null;

        public Method(Del _del)
        {
            del = _del;
        }

        public void Run()
        {
            del?.Invoke();
        }
    }
}
