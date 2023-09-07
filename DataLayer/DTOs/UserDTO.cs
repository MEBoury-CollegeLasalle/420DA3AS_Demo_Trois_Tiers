using System.Runtime.CompilerServices;

namespace _420DA3AS_Demo_Trois_Tiers.DataLayer.DTOs;

internal class UserDTO : IDTO, IHasPasswordFields {
    private const string TABLE_NAME = "Users";

    public int Id { get; set; }
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;

    public UserDTO() { }

    public UserDTO(int id) {
        this.Id = id;
    }

    public UserDTO(string userName, string passwordHash) {
        this.UserName = userName;
        this.Password = passwordHash;
    }

    public UserDTO(int id, string userName, string passwordHash) : this(userName, passwordHash) {
        this.Id = id;
    }

    public string GetDbTableName() {
        return TABLE_NAME;
    }

    public Type GetIdentifierType() {
        return this.Id.GetType();
    }

    public object GetIdentifierValue() {
        return this.Id;
    }

    public override string ToString() {
        return this.UserName;
    }

    public string[] GetPasswordFieldsNames() {
        return new string[] { "Password" };
    }
}
