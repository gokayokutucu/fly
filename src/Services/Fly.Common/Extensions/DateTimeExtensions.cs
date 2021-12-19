namespace Fly.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static long Timestamp(this DateTime time)
        {
            return new DateTimeOffset(time).ToUnixTimeSeconds();
        }
    }
}
