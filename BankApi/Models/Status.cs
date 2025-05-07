namespace BankApi.Models
{
    /// <summary>
    /// Defines the possible review statuses for a news article.
    /// </summary>
    public enum Status
    {
        /// <summary>
        /// The review is pending and has not been processed yet.
        /// </summary>
        Pending,

        /// <summary>
        /// The review has been approved.
        /// </summary>
        Approved,

        /// <summary>
        /// The review has been rejected.
        /// </summary>
        Rejected,

        /// <summary>
        /// Represents all statuses.
        /// </summary>
        All,
    }
}
