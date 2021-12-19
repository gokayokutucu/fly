using Fly.Common.Interface;

namespace Fly.Shopping.Helper
{
    public struct Clock : IClock
    {
        public static DateTime Now => DateTime.UtcNow;
    }
}
