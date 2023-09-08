using _420DA3AS_Demo_Trois_Tiers.BusinessLayer;
using _420DA3AS_Demo_Trois_Tiers.BusinessLayer.Services;
using _420DA3AS_Demo_Trois_Tiers.DataLayer.DTOs;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Reflection;

namespace _420DA3AS_Demo_Trois_Tiers.DataLayer.DAOs;

internal abstract class AbstractDAO<TDTO> : IDAO where TDTO : class, IDTO, new() {
    protected string DtoTableName { get; private set; }
    protected DbConnection Connection { get; private set; }
    protected DataSet DataSet { get; private set; }
    protected DbDataAdapter DataAdapter { get; set; }

    protected AbstractDAO(DbProviderFactory factory, DbConnection connection, DataSet dataSet) {
        if (MyApplication.DO_DEBUG) {
            DebuggerService.Info($"INITIALIZING DAO OF TYPE [{MyApplication.GetRealTypeName(this.GetType())}]...");
        }
        this.DtoTableName = new TDTO().GetDbTableName();
        this.Connection = connection;
        this.DataSet = dataSet;
        this.DataAdapter = this.CreateDataAdapter(factory);
    }

    protected abstract DbDataAdapter CreateDataAdapter(DbProviderFactory factory);

    public DataTable GetDataTable() {
        DataTable table = !this.DataSet.Tables.Contains(this.DtoTableName)
            ? this.CreateAndLoadTable()
            : this.DataSet.Tables[this.DtoTableName] ?? throw new Exception($"Table [{this.DtoTableName}] not found.");
        return table;
    }

    private DataTable CreateAndLoadTable() {
        DataTable table = new DataTable(this.DtoTableName);
        this.DataSet.Tables.Add(table);
        _ = this.DataAdapter.Fill(table);
        return table;
    }

    public void ReloadData() {
        DataTable table = this.GetDataTable();
        table.Clear();
        _ = this.DataAdapter.Fill(table);
    }

    public int SaveChanges() {
        int rowsChanged = this.DataAdapter.Update(this.GetDataTable());
        return rowsChanged;
    }

    public void CancelChanges() {
        this.GetDataTable().RejectChanges();
    }

    protected TDTO DataRowToDTO(DataRow row) {
        TDTO dto = new TDTO();
        return this.DataRowToDTO(row, dto);
    }

    protected TDTO DataRowToDTO(DataRow row, TDTO dto) {
        foreach (DataColumn column in row.Table.Columns) {
            PropertyInfo property = this.GetDtoProperty(typeof(TDTO), column.ColumnName);

            if (property != null && row[column] != DBNull.Value && row[column].ToString() != "NULL") {
                property.SetValue(dto, this.ChangeType(row[column], property.PropertyType), null);
            }
        }
        return dto;
    }

    protected DataRow DtoToDataRow(TDTO dto) {
        return this.DtoToDataRow(dto, this.GetDataTable().NewRow());
    }

    protected DataRow DtoToDataRow(TDTO dto, DataRow row) {
        foreach (DataColumn column in row.Table.Columns) {
            PropertyInfo property = this.GetDtoProperty(typeof(TDTO), column.ColumnName);

            row[column.ColumnName] = this.ChangeType(property.GetValue(dto), column.DataType) ?? DBNull.Value;
        }
        return row;

    }

    private PropertyInfo GetDtoProperty(Type type, string propName) {

        return type.GetProperty(propName)
            ?? type.GetProperties().Where(prop => {
                return prop.IsDefined(typeof(DisplayAttribute), false)
                    && prop.GetCustomAttributes(typeof(DisplayAttribute), false).Cast<DisplayAttribute>().Single().Name == propName;
            }).FirstOrDefault()
            ?? throw new Exception($"Failed to find property [{propName}] in type [{type.FullName}]");
    }

    public object? ChangeType(object? value, Type type) {

        return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>))
            ? value ?? Convert.ChangeType(value, Nullable.GetUnderlyingType(type) ?? type.GetGenericTypeDefinition())
            : Convert.ChangeType(value, type);
    }

}
