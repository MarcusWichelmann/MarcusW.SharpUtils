using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace MarcusW.SharpUtils.Redis.Extensions
{
    public static class DatabaseExtensions
    {
        private static readonly int[] LockRetryDelays = { 100, 300, 1000 };

        /// <summary>
        /// Creates a new managed transaction helper.
        /// </summary>
        /// <param name="database">Redis database</param>
        /// <returns>Managed transaction</returns>
        public static ManagedTransaction CreateManagedTransaction(this IDatabase database)
        {
            if (database == null)
                throw new ArgumentNullException(nameof(database));
            return new ManagedTransaction(database.CreateTransaction());
        }

        /// <summary>
        /// Tries to acquire a lock for a given key.
        /// If the key is already locked, this method will do multiple retries before an exception is thrown.
        /// </summary>
        /// <param name="database">Redis database</param>
        /// <param name="key">Redis key that should be locked</param>
        /// <param name="expiry">Time after which the lock will be automatically released</param>
        /// <returns>Lock token</returns>
        /// <exception cref="CannotAcquireLockException">Is thrown, when the lock could not be acquired.</exception>
        public static async Task<string> LockKeyAsync(this IDatabase database, string key, TimeSpan expiry)
        {
            if (database == null)
                throw new ArgumentNullException(nameof(database));
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            string token = Guid.NewGuid().ToString();

            // Iterate over retry delays
            foreach (var delay in LockRetryDelays)
            {
                // Try to acquire the lock
                if (await database.LockTakeAsync($"lock:{key}", token, expiry).ConfigureAwait(false))
                    return token;

                // Wait before trying again
                await Task.Delay(delay).ConfigureAwait(false);
            }

            throw new CannotAcquireLockException($"Cannot acquire lock for key {key}");
        }

        /// <summary>
        /// Tries to acquire a lock for a given key.
        /// If the key is already locked, this method will do multiple retries before an exception is thrown.
        /// </summary>
        /// <param name="database">Redis database</param>
        /// <param name="key">Redis key that should be locked</param>
        /// <param name="milliseconds">Milliseconds after which the lock will be automatically released</param>
        /// <returns>Lock token</returns>
        /// <exception cref="CannotAcquireLockException">Is thrown, when the lock could not be acquired.</exception>
        public static Task<string> LockKeyAsync(this IDatabase database, string key, int milliseconds) =>
            LockKeyAsync(database, key, TimeSpan.FromMilliseconds(milliseconds));

        /// <summary>
        /// Releases a lock for a given key.
        /// </summary>
        /// <param name="database"Redis database></param>
        /// <param name="key">The locked key</param>
        /// <param name="token">The token that was used for the lock</param>
        /// <returns>True if the lock was successfully released, false otherwise.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static Task<bool> UnlockKeyAsync(this IDatabase database, string key, string token)
        {
            if (database == null)
                throw new ArgumentNullException(nameof(database));
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (token == null)
                throw new ArgumentNullException(nameof(token));

            return database.LockReleaseAsync($"lock:{key}", token);
        }
    }
}
