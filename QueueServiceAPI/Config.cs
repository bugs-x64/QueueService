using System;

namespace QueueServiceAPI
{
    public static class Config
    {
        public static int SyntheticDelayMilliseconds { get; private set; } = 0;

        public static void SetSyntheticDelayMilliseconds(int milliseconds)
        {
            SyntheticDelayMilliseconds = milliseconds;
        }
        public static void SetSyntheticDelayMilliseconds(TimeSpan time)
        {
            SyntheticDelayMilliseconds = Convert.ToInt32(time.TotalMilliseconds);
        }
    }
}
