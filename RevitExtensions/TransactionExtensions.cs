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

        /// <summary>
        /// Assimilates the transaction group and ensures it succeeded.
        /// </summary>
        /// <param name="group">The transaction group to assimilate.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="group"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the transaction group fails to assimilate.</exception>
        public static void AssimilateAndEnsure(this TransactionGroup group)
        {
            if (group == null) throw new ArgumentNullException(nameof(group));

            var status = group.Assimilate();
            if (status != TransactionStatus.Committed)
            {
                throw new InvalidOperationException("Failed to assimilate transaction group.");
            }
        }

        /// <summary>
        /// Commits the subtransaction and ensures it succeeded.
        /// </summary>
        /// <param name="subTransaction">The subtransaction to commit.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="subTransaction"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the subtransaction fails to commit.</exception>
        public static void CommitAndEnsure(this SubTransaction subTransaction)
        {
            if (subTransaction == null) throw new ArgumentNullException(nameof(subTransaction));

            var status = subTransaction.Commit();
            if (status != TransactionStatus.Committed)
            {
                throw new InvalidOperationException("Failed to commit subtransaction.");
            }
        }
    }
}
