namespace Contracts
{
    public interface ICreateResult
    {
        bool Succeeded { get; set; }
        string Error { get; set; }

        string Key { get; set; }
    }

    public interface ITableData
    {
        void Encrypt();

        void Index();

        bool isIndexed { get; set; }

        bool isEncrypted { get; set; }

        string Encpwd { get; }

        string TableName { get; }
    }
}
