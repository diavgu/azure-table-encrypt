using Contracts;
using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Table; // Namespace for Table storage types
using Models;
using Security;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureTableManager
{
    public static class TABLEDAL
    {

        public static CloudStorageAccount GetStorageAcct()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            return storageAccount;
        }

        public static CloudTableClient GetTableClient()
        {
            var storageAccount = GetStorageAcct();
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            return tableClient;
        }

        public static async Task<ICreateResult> AddEntityAsync<T>(T entity) where T : ITableData, ITableEntity
        {
            var result = new CreateResult();
            var storageAccount = GetStorageAcct();
            var tableClient = GetTableClient();
            var tblref = tableClient.GetTableReference(entity.TableName);
            entity.Index();
            entity.Encrypt();
            var insertOperation = TableOperation.Insert(entity);
            await tblref.ExecuteAsync(insertOperation);
            result.Succeeded = true;
            return result;
        }

        public static ICreateResult AddEntity<T>(T entity) where T : ITableData, ITableEntity
        {
            var result = new CreateResult();
            var storageAccount = GetStorageAcct();
            var tableClient = GetTableClient();
            var tblref = tableClient.GetTableReference(entity.TableName);
            entity.Index();
            entity.Encrypt();
            var insertOperation = TableOperation.Insert(entity);
            tblref.Execute(insertOperation);
            result.Succeeded = true;
            return result;
        }

        public static IEnumerable<T> GetSpecificEntity<T>(T entity, string q, string column) where T : ITableData, ITableEntity, new()
        {
            var storageAccount = GetStorageAcct();
            var tableClient = GetTableClient();
            var tblref = tableClient.GetTableReference(entity.TableName);
            var query = new TableQuery<T>().Where(TableQuery.GenerateFilterCondition(column, QueryComparisons.Equal, Cryptography.Index(q, entity.Encpwd)));
            var entities = tblref.ExecuteQuery(query);
            return entities;
        }

        public static IEnumerable<T> GetSpecificEntityByRowKey<T>(T entity, string key) where T : ITableData, ITableEntity, new()
        {
            var storageAccount = GetStorageAcct();
            var tableClient = GetTableClient();
            var tblref = tableClient.GetTableReference(entity.TableName);
            var query = new TableQuery<T>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, key));
            var entities = tblref.ExecuteQuery(query).ToList();
            foreach (var ent in entities)
            {
                ent.isEncrypted = true;
            }
            return entities;
        }

        public static IEnumerable<T> GetAllEntities<T>(T entity) where T : ITableData, ITableEntity, new()
        {
            var storageAccount = GetStorageAcct();
            var tableClient = GetTableClient();
            var tblref = tableClient.GetTableReference(entity.TableName);
            var query = new TableQuery<T>();
            var entities = tblref.ExecuteQuery(query);
            return entities;
        }
    }
}
