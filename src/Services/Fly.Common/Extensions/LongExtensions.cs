namespace Fly.Common.Extensions
{
    public static class LongExtensions
    {
        public static byte[] ToByte(this long num)
        {
            return BitConverter.GetBytes(num);
        }
    }
}
