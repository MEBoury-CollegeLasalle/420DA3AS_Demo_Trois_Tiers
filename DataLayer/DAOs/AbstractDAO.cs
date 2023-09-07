using _420DA3AS_Demo_Trois_Tiers.BusinessLayer.Services;
using _420DA3AS_Demo_Trois_Tiers.DataLayer.DTOs;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace _420DA3AS_Demo_Trois_Tiers.DataLayer.DAOs;

internal abstract class AbstractDAO<TDTO> : IDAO where TDTO : class, IDTO, new() {
    protected string DtoTableName { get; private set; }
    protected DataSet DataSet { get; private set; }
    protected DbDataAdapter DataAdapter { get; set; }

    protected AbstractDAO(DbProviderFactory factory, DataSet dataSet) {
        this.DtoTableName = new TDTO().GetDbTableName();
        this.DataAdapter = this.CreateDataAdapter(factory);
        this.DataSet = dataSet;
    }

    protected abstract DbDataAdapter CreateDataAdapter(DbProviderFactory factory);

    public DataTable GetDataTable() {
        if (!this.DataSet.Tables.Contains(this.DtoTableName)) {
            this.LoadData();
        }
        return this.DataSet.Tables[this.DtoTableName] ?? throw new Exception($"Table [{this.DtoTableName}] not found.");
    }

    public void LoadData() {
        bool isForUserDTO = typeof(TDTO).IsAssignableFrom(typeof(IHasPasswordFields));
        if (isForUserDTO && this.DataSet.Tables.Contains(this.DtoTableName)) {
            this.GetDataTable().RowChanging -= this.UserRowAddedEventHandler<TDTO>;
        }
        _ = this.DataAdapter.Fill(this.DataSet, this.DtoTableName);
        if (isForUserDTO) {
            this.GetDataTable().RowChanging += this.UserRowAddedEventHandler<TDTO>;
        }
    }

    public int SaveChanges() {
        this.GetDataTable().AcceptChanges();
        return this.DataAdapter.Update(this.GetDataTable());
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


    private void UserRowAddedEventHandler<PasswordedBearingType>(object sender, DataRowChangeEventArgs args) where PasswordedBearingType :new() {
        if (args.Action == DataRowAction.Add) {
            foreach (string passwordedFieldName in ((IHasPasswordFields) new PasswordedBearingType()).GetPasswordFieldsNames()) {
                DataColumn column = this.GetDataTable().Columns[passwordedFieldName] 
                    ?? throw new Exception($"Unable to find password-bearing property [{passwordedFieldName}] in object of type [{typeof(PasswordedBearingType).FullName}].");
                args.Row.SetField<string>(column, SecurityService.HashPassword(args.Row.Field<string>(column)));
            }
        }
    }

}
