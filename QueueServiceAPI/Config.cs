using System;

namespace QueueServiceAPI
{
    /// <summary>
    /// Статический конфиг.
    /// </summary>
    public static class Config
    {
        /// <summary>
        /// Синтетическая задержка.
        /// </summary>
        public static TimeSpan SyntheticDelay { get; set; }
    }
}
