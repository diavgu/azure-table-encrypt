using Contracts;
using Microsoft.WindowsAzure.Storage.Table;
using Security;

namespace Models
{
    public class TestModel : TableEntity, ITableData
    {
        /// <summary>
        /// you decide what the keys should be but read this article:
        /// https://docs.microsoft.com/en-us/rest/api/storageservices/understanding-the-table-service-data-model
        /// if you have lots of data:
        /// https://docs.microsoft.com/en-us/rest/api/storageservices/designing-a-scalable-partitioning-strategy-for-azure-table-storage
        /// 
        /// </summary>
        /// <param name="partitionkey"></param>
        /// <param name="rowkey"></param>
        public TestModel(string partitionkey, string rowkey)
        {
            this.PartitionKey = partitionkey;
            this.RowKey = rowkey;
            this.isEncrypted = false;
            this.isIndexed = false;
        }

        public TestModel() { this.isEncrypted = false; }

        public string Email { get; set; }

        public string Product { get; set; }

        public string EmailIndex { get; set; }

        public string ProductIndex { get; set; }

        public bool Succeeded { get; set; }

        public string Error { get; set; }

        public string Key { get; set; }
        public bool isIndexed { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public bool isEncrypted { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        //must ignore or the azure sdk will try to write this to the table...
        [IgnoreProperty]
        public string Encpwd { get { return "arbitrarylongsstringusedtoencryptyourdata"; } }

        [IgnoreProperty]
        public string TableName { get { return "Test"; } }

        public void Encrypt()
        {
            if (!isEncrypted && !isIndexed)
            {
                this.Index();
            }

            if (this.isEncrypted)
            {
                return;
            }

            string iv, salt;

            this.Email = Cryptography.Encrypt(this.Email, Encpwd, out iv, out salt);
            this.Product = Cryptography.Encrypt(this.Product, Encpwd, iv, salt);
            //you're gonna want to save the iv and salt somewhere...
            this.isEncrypted = true;
        }

        public void Index()
        {
            this.EmailIndex = Cryptography.Index(this.Email, Encpwd);
            this.ProductIndex = Cryptography.Index(this.Product, Encpwd);
            this.isIndexed = true;
        }

        public void Decrypt()
        {
            if (this.isEncrypted == false)
            {
                return;
            }

            var iv = "";
            var salt = "";
            this.Email = Cryptography.Decrypt(this.Email, Encpwd, iv, salt);
            this.Product = Cryptography.Decrypt(this.Product, Encpwd, iv, salt);
            this.isEncrypted = false;
        }
    }
}