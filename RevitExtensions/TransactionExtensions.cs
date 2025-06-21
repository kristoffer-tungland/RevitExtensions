using System;
using Autodesk.Revit.DB;

namespace RevitExtensions
{
    /// <summary>
    /// Extension methods for <see cref="Transaction"/>.
    /// </summary>
    public static class TransactionExtensions
    {
        /// <summary>
        /// Commits the transaction and ensures it succeeded.
        /// </summary>
        /// <param name="transaction">The transaction to commit.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="transaction"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the transaction fails to commit.</exception>
        public static void CommitAndEnsure(this Transaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));

            var status = transaction.Commit();
            if (status != TransactionStatus.Committed)
            {
                throw new InvalidOperationException("Failed to commit transaction.");
            }
        }
    }
}
