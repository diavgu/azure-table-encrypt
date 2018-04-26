using Contracts;
using Models;
using System.Threading.Tasks;

namespace AzureTableManager
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await DoTestAsync();
        }
        public async static Task<ICreateResult> DoTestAsync()
        {
            //create a model of your table 
            var entity = new TestModel();
            return await TABLEDAL.AddEntityAsync(entity);
        }
    }
}
