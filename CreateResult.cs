using Contracts;

namespace Models
{
    public class CreateResult : ICreateResult
    {
        public bool Succeeded { get; set; }

        public string Error { get; set; }

        public string Key { get; set; }
    }
}