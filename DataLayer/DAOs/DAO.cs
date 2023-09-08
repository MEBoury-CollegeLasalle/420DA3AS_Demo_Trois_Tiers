using _420DA3AS_Demo_Trois_Tiers.BusinessLayer;
using _420DA3AS_Demo_Trois_Tiers.BusinessLayer.Services;
using _420DA3AS_Demo_Trois_Tiers.DataLayer.DTOs;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Data.Common;
using System.Diagnostics;

namespace _420DA3AS_Demo_Trois_Tiers.DataLayer.DAOs;
internal class DAO<TDTO> : AbstractDAO<TDTO> where TDTO : class, IDTO, new() {
    public static readonly Type DtoType = typeof(TDTO);

    public DAO(DbProviderFactory factory, DbConnection connection, DataSet dataSet) : base(factory, connection, dataSet) { }


    protected override DbDataAdapter CreateDataAdapter(DbProviderFactory factory) {
        DbDataAdapter adapter = factory.CreateDataAdapter()
            ?? throw new Exception("Failed to create DbDataAdapter instance for UserDAO instance.");

        adapter.SelectCommand = factory.CreateCommand() ?? throw new Exception("Failed to create 'Select' DbCommand instance for UserDAO instance.");
        adapter.SelectCommand.CommandText = $"SELECT * FROM {this.DtoTableName};";
        adapter.SelectCommand.Connection = this.Connection;

        DbCommandBuilder builder = factory.CreateCommandBuilder() ?? throw new Exception("Failed to create DbCommandBuilder instance for UserDAO instance.");
        builder.DataAdapter = adapter;
        adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

        DbCommand insertCommand = builder.GetInsertCommand();
        insertCommand.Connection = this.Connection;
        DbCommand updateCommand = builder.GetUpdateCommand();
        updateCommand.Connection = this.Connection;
        DbCommand deleteCommand = builder.GetDeleteCommand();
        deleteCommand.Connection = this.Connection;
        adapter.InsertCommand = insertCommand;
        adapter.UpdateCommand = updateCommand;
        adapter.DeleteCommand = deleteCommand;

        if (adapter is SqlDataAdapter sqlAdapter) {
            sqlAdapter.RowUpdating += this.OnRowUpdating;
        } else if (adapter is MySqlDataAdapter mysqlAdapter) {
            mysqlAdapter.RowUpdating += this.OnRowUpdating;
        } else if (adapter is OracleDataAdapter oracleAdapter) {
            oracleAdapter.RowUpdating += this.OnRowUpdating;
        }

        if (MyApplication.DO_DEBUG) {
            DebuggerService.Success($"DataAdapter of DAO [{MyApplication.GetRealTypeName(this.GetType())}] successfully constructed.");
            DebuggerService.Info("Generated commands:");
            DebuggerService.Info($"INSERT COMMAND: {insertCommand.CommandText}");
            DebuggerService.Info($"UPDATE COMMAND: {updateCommand.CommandText}");
            DebuggerService.Info($"DELETE COMMAND: {deleteCommand.CommandText}");
        }

        _ = adapter.Fill(this.DataSet, this.DtoTableName);

        return adapter;
    }

    public List<TDTO> GetAll() {
        List<TDTO> userList = new List<TDTO>();
        foreach (DataRow row in this.GetDataTable().Select()) {
            userList.Add(this.DataRowToDTO(row));
        }
        return userList;
    }

    public TDTO? GetById(int id) {
        DataRow? row = this.GetDataTable().Select("Id = " + id.ToString()).FirstOrDefault();
        return row == null ? null : this.DataRowToDTO(row);
    }

    public TDTO Insert(TDTO dto) {
        if (dto.GetIdentifierValue() != Activator.CreateInstance(dto.GetIdentifierType())) {
            throw new ArgumentException("Cannot insert a DTO with an identifier value set.");
        }

        DataRow row = this.DtoToDataRow(dto);
        _ = this.DataAdapter.Update(this.GetDataTable());
        return this.DataRowToDTO(row, dto);
    }

    public TDTO Load(TDTO dto) {
        if (dto.GetIdentifierValue() == Activator.CreateInstance(dto.GetIdentifierType())) {
            throw new ArgumentException("Cannot load a DTO with no identifier value set.");
        }

        DataRow row = this.GetDataTable().Select("Id = " + dto.GetIdentifierValue().ToString()).FirstOrDefault()
            ?? throw new Exception($"No row found for selection condition [Id = {dto.GetIdentifierValue()}].");

        return this.DataRowToDTO(row, dto);

    }

    public TDTO Update(TDTO dto) {
        if (dto.GetIdentifierValue() == Activator.CreateInstance(dto.GetIdentifierType())) {
            throw new ArgumentException("Cannot update a DTO with no identifier value set.");
        }

        DataRow row = this.GetDataTable().Select("Id = " + dto.GetIdentifierValue().ToString()).FirstOrDefault()
            ?? throw new Exception($"No row found for selection condition [Id = {dto.GetIdentifierValue()}].");

        _ = this.DtoToDataRow(dto, row);
        _ = this.DataAdapter.Update(this.GetDataTable());

        return this.DataRowToDTO(row, dto);
    }

    public void Delete(TDTO dto) {
        if (dto.GetIdentifierValue() == Activator.CreateInstance(dto.GetIdentifierType())) {
            throw new ArgumentException("Cannot delete a DTO with no identifier value set.");
        }

        DataRow row = this.GetDataTable().Select("Id = " + dto.GetIdentifierValue().ToString()).FirstOrDefault()
            ?? throw new Exception($"No row found for selection condition [Id = {dto.GetIdentifierValue()}].");

        row.Delete();
        _ = this.DataAdapter.Update(this.GetDataTable());
    }

    private void OnRowUpdating(object sender, RowUpdatingEventArgs args) {
        DebuggerService.Warn($"ROW UPDATING! - {args.StatementType}");
        bool isForUserDTO = typeof(TDTO) == typeof(UserDTO);
        if (isForUserDTO && args.StatementType == StatementType.Insert) {
            DebuggerService.Warn("Attempting to auto-hash set password!");
            IDbCommand command = args.Command ?? throw new Exception("Command not found!");
            object what =  command.Parameters[1] ?? throw new Exception("Parameter not found!");
            IDbDataParameter parameter = (IDbDataParameter) what;
            string password = args.Row.Field<string>("Password") ?? throw new Exception("Password column not found!");
            string passwordHash = SecurityService.HashPassword(password) ?? throw new Exception("No password to hash!");
            parameter.Value = passwordHash;
            args.Row.SetField("Password", passwordHash);
            DebuggerService.Success("Password auto-hash should have been successful|");
        }
    }
}
