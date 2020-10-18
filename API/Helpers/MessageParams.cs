namespace API.Helpers
{
    public class MessageParams : PaginationParams
    {
        public string Usernam { get; set; }
        public string Container { get; set; } = "UnRead";
    }
}