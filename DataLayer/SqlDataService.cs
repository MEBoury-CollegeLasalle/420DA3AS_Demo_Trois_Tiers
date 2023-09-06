using _420DA3AS_Demo_Trois_Tiers.DataLayer.DAOs;
using _420DA3AS_Demo_Trois_Tiers.DataLayer.DTOs;
using Microsoft.Data.SqlClient;

namespace _420DA3AS_Demo_Trois_Tiers.DataLayer;
internal class SqlDataService {
    private SqlConnection connection;
    private readonly UserDAO userDAO;

    public SqlDataService() {
        this.connection = new SqlConnection();
        // TODO: configure SQL-style connection here
        this.userDAO = new UserDAO(this.connection);
    }

    public List<UserDTO> GetAllUsers() {
        return this.userDAO.GetAll();
    }

    public UserDTO GetUser(int id) {
        return this.userDAO.GetById(id);
    }

    public UserDTO InsertUser(UserDTO user) {
        return this.userDAO.Insert(user);
    }

    public UserDTO ReloadUser(UserDTO user) {
        return this.userDAO.Fetch(user);
    }

    public UserDTO UpdateUser(UserDTO user) {
        return this.userDAO.Update(user);
    }

    public void DeleteUser(UserDTO user) {
        this.userDAO.Delete(user);
    }
}
