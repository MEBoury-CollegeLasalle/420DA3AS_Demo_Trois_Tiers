using _420DA3AS_Demo_Trois_Tiers.DataLayer.DTOs;
using System.Data;
using System.Data.Common;

namespace _420DA3AS_Demo_Trois_Tiers.DataLayer.DAOs;
internal class UserDAO {
    private static readonly string TABLE_NAME = "Users";
    private static readonly string FETCH_ALL_QUERY = $"SELECT * FROM {TABLE_NAME};";
    private static readonly string INSERT_QUERY = $"INSERT INTO {TABLE_NAME} (UserName, PasswordHash) " +
        $"VALUES (@userName, @passwordHash); " +
        $"SELECT SCOPE_IDENTITY();";
    private static readonly string FETCH_QUERY = $"SELECT * FROM {TABLE_NAME} " +
        $"WHERE Id = @id;";
    private static readonly string UPDATE_QUERY = $"UPDATE {TABLE_NAME} " +
        $"SET UserName = @userName, PasswordHash = @passwordHash " +
        $"WHERE Id = @id;";
    private static readonly string DELETE_QUERY = $"DELETE FROM {TABLE_NAME} " +
        $"WHERE Id = @id;";
    private readonly DbConnection connection;

    public UserDAO(DbConnection connection) {
        this.connection = connection;
    }

    public List<UserDTO> GetAll() {
        try {

            // declare and initialize an empty list of user DTOs
            List<UserDTO> allUsers = new List<UserDTO>();

            // Opening the connection if it is not already open
            if (this.connection.State != ConnectionState.Open) {
                connection.Open();
            }

            // create the database request/command
            DbCommand command = this.connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = FETCH_ALL_QUERY;

            // No parameters in this query


            // Execute the command. Insert command returns an int-like scalar value (last inserted ID)
            // via the sub-query 'SELECT SCOPE_IDENTITY();' in the command's text
            DbDataReader reader = command.ExecuteReader();

            // read every row from the result set, create a DTO object instance and add it
            // to the list.
            while (reader.Read()) {
                UserDTO dto = new UserDTO(reader.GetInt32(0));
                dto.UserName = reader.GetString(1);
                dto.PasswordHash = reader.GetString(2);
                allUsers.Add(dto);
            }

            // return the list.
            return allUsers;

        } catch (Exception ex) {
            throw new Exception("Failure to insert user in the database.", ex);
        }
    }

    public UserDTO GetById(int id) {
        UserDTO userDTO = new UserDTO(id);
        return this.Fetch(userDTO);
    }

    public UserDTO Insert(UserDTO dto) {
        if (dto.Id > 0) {
            throw new ArgumentException("Cannot insert a DTO with an identifier value set.");
        }
        try {
            // Opening connection if it is not already open
            if (this.connection.State != ConnectionState.Open) {
                connection.Open();
            }

            // create the database request/command
            DbCommand command = this.connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = INSERT_QUERY;

            // Set parameter values
            DbParameter userNameParam = command.CreateParameter();
            userNameParam.DbType = DbType.String;
            userNameParam.ParameterName = "@userName";
            userNameParam.Value = dto.UserName;
            command.Parameters.Add(userNameParam);
            DbParameter passwordHashParam = command.CreateParameter();
            userNameParam.DbType = DbType.String;
            userNameParam.ParameterName = "@passwordHash";
            userNameParam.Value = dto.PasswordHash;
            command.Parameters.Add(passwordHashParam);

            // Execute the command. Insert command returns an int-like scalar value (last inserted ID)
            // via the sub-query 'SELECT SCOPE_IDENTITY();' in the command's text
            object? newId = command.ExecuteScalar();

            // but it can fail, so check that we do get something back
            if (newId is DBNull || newId is null) {
                throw new Exception("Return of the database-generated ID when inserting user instance failed.");
            }

            // cast the returned int-like value as an integer and set it as the DTO's Id value
            dto.Id = (int) newId;

            // Fetch (re-load) the DTO object and return the result of the fetching operation.
            // This is done in case the table possess other auto-generated value columns
            // like DEFAULT CURDATE() date-created columns.
            // By fetching the complete row after insertion we ensure that we get ALL the values
            // that are auto-generated.
            return this.Fetch(dto);

        } catch (Exception ex) {
            throw new Exception("Failure to insert user in the database.", ex);
        }

    }

    public UserDTO Fetch(UserDTO dto) {
        if (dto.Id <= 0) {
            throw new ArgumentException("Cannot fetch (load) a DTO with no identifier value set.");
        }
        try {
            // Opening connection if it is not already open
            if (this.connection.State != ConnectionState.Open) {
                connection.Open();
            }

            // create the database request/command
            DbCommand command = this.connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = FETCH_QUERY;

            // Set parameter values
            DbParameter idParam = command.CreateParameter();
            idParam.DbType = DbType.Int32;
            idParam.ParameterName = "@id";
            idParam.Value = dto.Id;
            command.Parameters.Add(idParam);

            // Execute the command. Insert command returns an int-like scalar value (last inserted ID)
            // via the sub-query 'SELECT SCOPE_IDENTITY();' in the command's text
            DbDataReader reader = command.ExecuteReader();
            if (!reader.Read()) {
                // no rows returned in the result set -> data not found
                throw new Exception($"User ID# [{dto.Id}] not found in the database.");
            }

            // specific for the User table - model
            // read values for the columns of the first row in the result set (aka first row)
            // and set the values in the dto object (AKA, refresh the DTO with values from the database)
            dto.Id = reader.GetInt32(0);
            dto.UserName = reader.GetString(1);
            dto.PasswordHash = reader.GetString(2);

            // this here is not functionally needed, however, if the result set still has other rows to read
            // it means that you have two rows in your database with the same Id value, which shouldn't
            // ever happen, so i'm throwing an exception 'cause something is very wrong!
            if (reader.Read()) {
                // reader has more rows with the specified Id than the first one reade to re-fill the DTO object.
                // BIG problem with the way the database is built!
                throw new Exception($"Multiple rows found in database for user ID# [{dto.Id}]!");
            }

            // return the re-loaded DTO
            return dto;

        } catch (Exception ex) {
            throw new Exception("Failure to insert user in the database.", ex);
        }

    }

    public UserDTO Update(UserDTO dto) {
        if (dto.Id <= 0) {
            throw new ArgumentException("Cannot update a DTO with no identifier value set.");
        }

        throw new NotImplementedException("Not implemented in this demo because i'm a lazy bum hahaha.");
    }

    public void Delete(UserDTO dto) {
        if (dto.Id <= 0) {
            throw new ArgumentException("Cannot delete a DTO with no identifier value set.");
        }

        throw new NotImplementedException("Not implemented in this demo because i'm a lazy bum hahaha.");
    }
}
