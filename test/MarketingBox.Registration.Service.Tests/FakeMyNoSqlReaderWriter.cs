using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.MyNoSql.Brands;
using MarketingBox.Registration.Service.Domain.Models.TrafficEngine;
using MarketingBox.Registration.Service.MyNoSql.TrafficEngine;
using MyNoSqlServer.Abstractions;

namespace MarketingBox.Registration.Service.Tests
{
    public class FakeMyNoSqlReaderWriter<T> :
        IMyNoSqlServerDataReader<T>, 
        IMyNoSqlServerDataWriter<T> 
        where T : MyNoSqlDbEntity, new()
    {
        public Dictionary<string, Dictionary<string, T>> Storage { get; set; }
            = new();
        public T Get(string partitionKey, string rowKey)
        {
            if (!Storage.TryGetValue(partitionKey, out var row))
            {
                return null;
            }

            if (row == null)
                return null;

            if (!row.TryGetValue(rowKey, out var val))
            {
                return null;
            }

            if (val is BrandCandidateNoSql brandCandidateNoSql)
            {
                brandCandidateNoSql.BrandCandidate.UpdatedAt = Date;
            }
            return val;
        }

        public DayOfWeek Date { get; set; } = DateTime.Today.DayOfWeek;

        public IReadOnlyList<T> Get(string partitionKey)
        {
            if (!Storage.TryGetValue(partitionKey, out var row))
            {
                return null;
            }

            return row?.Values.ToArray();
        }

        public IReadOnlyList<T> Get(string partitionKey, int skip, int take)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<T> Get(string partitionKey, int skip, int take, Func<T, bool> condition)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<T> Get(string partitionKey, Func<T, bool> condition)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<T> Get(Func<T, bool> condition = null)
        {
            throw new NotImplementedException();
        }

        public int Count()
        {
            throw new NotImplementedException();
        }

        public int Count(string partitionKey)
        {
            throw new NotImplementedException();
        }

        public int Count(string partitionKey, Func<T, bool> condition)
        {
            throw new NotImplementedException();
        }

        public IMyNoSqlServerDataReader<T> SubscribeToUpdateEvents(Action<IReadOnlyList<T>> updateSubscriber, Action<IReadOnlyList<T>> deleteSubscriber)
        {
            throw new NotImplementedException();
        }

        public ValueTask InsertAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public ValueTask InsertOrReplaceAsync(T entity)
        {
            var partition = entity.PartitionKey;
            var row = entity.RowKey;

            if (Storage.TryGetValue(partition, out var stor))
            {
                stor[row] = entity;
                return ValueTask.CompletedTask;
            }

            var dict = new Dictionary<string, T>();
            dict[row] = entity;
            Storage[partition] = dict;

            return ValueTask.CompletedTask;
        }

        public ValueTask CleanAndKeepLastRecordsAsync(string partitionKey, int amount)
        {
            throw new NotImplementedException();
        }

        public ValueTask BulkInsertOrReplaceAsync(IEnumerable<T> entity, DataSynchronizationPeriod dataSynchronizationPeriod = DataSynchronizationPeriod.Sec5)
        {
            throw new NotImplementedException();
        }

        public ValueTask CleanAndBulkInsertAsync(IEnumerable<T> entity, DataSynchronizationPeriod dataSynchronizationPeriod = DataSynchronizationPeriod.Sec5)
        {
            throw new NotImplementedException();
        }

        public ValueTask CleanAndBulkInsertAsync(string partitionKey, IEnumerable<T> entity,
            DataSynchronizationPeriod dataSynchronizationPeriod = DataSynchronizationPeriod.Sec5)
        {
            throw new NotImplementedException();
        }

        public ValueTask<OperationResult> ReplaceAsync(string partitionKey, string rowKey, Func<T, bool> updateCallback,
            DataSynchronizationPeriod syncPeriod = DataSynchronizationPeriod.Sec5)
        {
            throw new NotImplementedException();
        }

        public ValueTask<OperationResult> MergeAsync(string partitionKey, string rowKey, Func<T, bool> updateCallback,
            DataSynchronizationPeriod syncPeriod = DataSynchronizationPeriod.Sec5)
        {
            throw new NotImplementedException();
        }

        public ValueTask<IEnumerable<T>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<T> GetAllAsync(int bulkRecordsCount)
        {
            throw new NotImplementedException();
        }

        public ValueTask<IEnumerable<T>> GetAsync(string partitionKey)
        {
            throw new NotImplementedException();
        }

        public ValueTask<T> GetAsync(string partitionKey, string rowKey)
        {
            throw new NotImplementedException();
        }

        public ValueTask<IReadOnlyList<T>> GetMultipleRowKeysAsync(string partitionKey, IEnumerable<string> rowKeys)
        {
            throw new NotImplementedException();
        }

        public ValueTask<T> DeleteAsync(string partitionKey, string rowKey)
        {
            if (Storage.TryGetValue(partitionKey, out var dict))
            {
                if (dict.Remove(rowKey, out var val))
                {
                    return ValueTask.FromResult(val);
                }
            }

            return ValueTask.FromResult<T>(null);
        }

        public ValueTask<IEnumerable<T>> QueryAsync(string query)
        {
            throw new NotImplementedException();
        }

        public ValueTask<IEnumerable<T>> GetHighestRowAndBelow(string partitionKey, string rowKeyFrom, int amount)
        {
            throw new NotImplementedException();
        }

        public ValueTask CleanAndKeepMaxPartitions(int maxAmount)
        {
            throw new NotImplementedException();
        }

        public ValueTask CleanAndKeepMaxRecords(string partitionKey, int maxAmount)
        {
            throw new NotImplementedException();
        }

        public ValueTask<int> GetCountAsync(string partitionKey)
        {
            throw new NotImplementedException();
        }

        public ValueTask<ITransactionsBuilder<T>> BeginTransactionAsync()
        {
            throw new NotImplementedException();
        }
    }
}