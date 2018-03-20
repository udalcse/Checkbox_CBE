using System;

namespace Prezza.Framework.Common
{
    /// <summary>
    /// Interface marking objects as transactional, which means they support rollback and notification of abort or commit events.  Useful as
    /// a marker in cases where eventing is not possible due to deserialization issues that cause events to become unbound.
    /// </summary>
    public interface ITransactional
    {
        /// <summary>
        /// Event marking transaction aborted.
        /// </summary>
        event EventHandler TransactionAborted;

        /// <summary>
        /// Event marking transaction committed.
        /// </summary>
        event EventHandler TransactionCommitted;

        /// <summary>
        /// Rollback transaction.
        /// </summary>
        void Rollback();

        /// <summary>
        /// Notify the transaction object that a transaction has been aborted.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void NotifyAbort(object sender, EventArgs e);

        /// <summary>
        /// Notify the transaction object that a transaction has been committed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void NotifyCommit(object sender, EventArgs e);
    }
}
