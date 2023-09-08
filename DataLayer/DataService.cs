﻿using _420DA3AS_Demo_Trois_Tiers.BusinessLayer.Services;
using _420DA3AS_Demo_Trois_Tiers.DataLayer.DAOs;
using _420DA3AS_Demo_Trois_Tiers.DataLayer.DTOs;
using System.Data;
using System.Data.Common;

namespace _420DA3AS_Demo_Trois_Tiers.DataLayer;
internal class DataService : AbstractService {
    private readonly DataSet dataSet;
    private readonly List<IDAO> daoList;

    public DataService(DbProviderFactory factory, DbConnection connection) : base() {
        this.dataSet = new DataSet();
        this.daoList = new List<IDAO> {
            new DAO<UserDTO>(factory, connection, this.dataSet)
        };
    }

    public DataTable GetDataTable<TDTO>() where TDTO : class, IDTO, new() {
        return this.daoList.FirstOrDefault(dao => {
            return dao is DAO<TDTO>;
        })?.GetDataTable()
            ?? throw new Exception($"No data table found for DTO type [{typeof(TDTO).FullName}].");
    }

    public void ReloadData<TDTO>() where TDTO : class, IDTO, new() {
        IDAO dao = this.daoList.FirstOrDefault(dao => {
            return dao is DAO<TDTO>;
        }) ?? throw new Exception($"No dao found for DTO type [{typeof(TDTO).FullName}].");
        dao.ReloadData();
    }

    public int SaveChanges<TDTO>() where TDTO : class, IDTO, new() {
        IDAO dao = this.daoList.FirstOrDefault(dao => {
            return dao is DAO<TDTO>;
        }) ?? throw new Exception($"No dao found for DTO type [{typeof(TDTO).FullName}].");
        return dao.SaveChanges();
    }

    public void CancelChanges<TDTO>() where TDTO : class, IDTO, new() {
        IDAO dao = this.daoList.FirstOrDefault(dao => {
            return dao is DAO<TDTO>;
        }) ?? throw new Exception($"No dao found for DTO type [{typeof(TDTO).FullName}].");
        dao.CancelChanges();
    }

}
