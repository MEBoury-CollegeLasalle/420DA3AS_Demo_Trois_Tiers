using _420DA3AS_Demo_Trois_Tiers.DataLayer.DTOs;
using System.Data;
using System.Data.Common;

namespace _420DA3AS_Demo_Trois_Tiers.DataLayer.DAOs;
internal class DAO<TDTO> : AbstractDAO<TDTO> where TDTO : class, IDTO, new() {
    public static readonly Type DtoType = typeof(TDTO);

    public DAO(DbProviderFactory factory, DataSet dataSet) : base(factory, dataSet) { }


    protected override DbDataAdapter CreateDataAdapter(DbProviderFactory factory) {
        DbDataAdapter adapter = factory.CreateDataAdapter()
            ?? throw new Exception("Failed to create DbDataAdapter instance for UserDAO instance.");

        adapter.SelectCommand = factory.CreateCommand() ?? throw new Exception("Failed to create 'Select' DbCommand instance for UserDAO instance.");
        adapter.SelectCommand.CommandText = $"SELECT * FROM {this.DtoTableName};";

        DbCommandBuilder builder = factory.CreateCommandBuilder() ?? throw new Exception("Failed to create DbCommandBuilder instance for UserDAO instance.");
        builder.DataAdapter = adapter;
        adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

        adapter.InsertCommand = builder.GetInsertCommand();
        adapter.UpdateCommand = builder.GetUpdateCommand();
        adapter.DeleteCommand = builder.GetDeleteCommand();

        _ = adapter.Fill(this.GetDataTable());

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
}
