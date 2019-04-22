using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace MarcusW.SharpUtils.Redis
{
    /// <summary>
    /// Helper class for Redis transactions
    /// </summary>
    public class ManagedTransaction
    {
        private readonly ITransaction _baseTransaction;

        private readonly IList<Task> _tasks = new List<Task>();

        /// <summary>
        /// Create a new instance of the transaction helper class.
        /// </summary>
        /// <param name="baseTransaction">Base Redis transaction</param>
        public ManagedTransaction(ITransaction baseTransaction)
        {
            _baseTransaction = baseTransaction ?? throw new ArgumentNullException(nameof(baseTransaction));
        }

        /// <summary>
        /// Add a precondition for this transaction.
        /// </summary>
        /// <param name="condition">Precondition</param>
        public void AddCondition(Condition condition)
        {
            if (condition == null)
                throw new ArgumentNullException(nameof(condition));
            _baseTransaction.AddCondition(condition);
        }

        /// <summary>
        /// Adds an operation to the transaction.
        /// </summary>
        /// <param name="operation">Operation</param>
        public void AddOperation(Func<ITransaction, Task> operation)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));
            _tasks.Add(operation(_baseTransaction));
        }

        /// <summary>
        /// Adds an operation which returns a result as soon as the transaction has been executed.
        /// </summary>
        /// <param name="operation">Operation with result</param>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <returns>Result</returns>
        public Task<TResult> AddOperationWithResultAsync<TResult>(Func<ITransaction, Task<TResult>> operation)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));
            Task<TResult> task = operation(_baseTransaction);
            _tasks.Add(task);
            return task;
        }

        /// <summary>
        /// Execute the transaction.
        /// </summary>
        /// <exception cref="RedisTransactionFailedException">Is thrown when the transaction could not be commited.</exception>
        public async Task ExecuteAsync()
        {
            bool committed = await TryExecuteAsync().ConfigureAwait(false);
            if (!committed)
                throw new RedisTransactionFailedException("Redis transaction wasn't commited.");
        }

        /// <summary>
        /// Try to execute the transaction.
        /// </summary>
        /// <returns>True, if the transaction could be committed.</returns>
        public async Task<bool> TryExecuteAsync()
        {
            bool committed = await _baseTransaction.ExecuteAsync().ConfigureAwait(false);
            try
            {
                await Task.WhenAll(_tasks).ConfigureAwait(false);
            }
            catch (TaskCanceledException) when (!committed) { }

            return committed;
        }
    }
}
