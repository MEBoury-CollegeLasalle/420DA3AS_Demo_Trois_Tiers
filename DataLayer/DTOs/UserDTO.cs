namespace _420DA3AS_Demo_Trois_Tiers.DataLayer.DTOs;

internal class UserDTO {

    public int Id { get; set; }
    public string UserName { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;

    public UserDTO(int id) {
        this.Id = id;
    }

    public UserDTO(string userName, string passwordHash) {
        this.UserName = userName;
        this.PasswordHash = passwordHash;
    }

    public UserDTO(int id, string userName, string passwordHash) : this(userName, passwordHash) {
        this.Id = id;
    }

    public override string ToString() {
        return this.UserName;
    }
}
