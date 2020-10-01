namespace MongoQueue.Core.Entities
{
    /// <summary>
    /// Write guaranties
    /// </summary>
    public enum Ack
    {
        /// <summary>
        /// Fire and forget
        /// This option is the fastesr, but provides no guaranties
        /// </summary>
        None = 0,
        /// <summary>
        /// Wait response from master
        /// </summary>
        Master = 1,
        /// <summary>
        /// Quorum write
        /// This option is the most resilent, but the slowest
        /// </summary>
        Majority = 2
    }
}