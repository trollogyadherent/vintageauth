public class User
{
    public int Id { get; set; }
    public string Uuid { get; set; }
    public string Name { get; set; }
    public bool Active { get; set; }
    public string Role { get; set; }
    public string Salt { get; set; }
    public string Hash { get; set; }

    public User(int id, string uuid, string name, bool active, string hash, string salt, string role)
    {
        Id = id;
        Uuid = uuid;
        Name = name;
        Active = active;
        Hash = hash;
        Salt = salt;
        Role = role;
    }
}