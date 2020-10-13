namespace TestStore_server.Models
{
    public class User
    {
        public int Id { get; internal set; }
        public string email { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string role { get; set; } = "user";
        public string password { get; set; }
    }
}
