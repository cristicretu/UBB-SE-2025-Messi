using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;

public class DataLink
{   
    private SqlConnection sqlConnection;
    private readonly string connectionString;

    public DataLink(IConfiguration configuration)
    {
        string localDataSource = configuration["LocalDataSource"];
        connectionString = "DataSource=" + localDataSource +
                        "Initial Catalog=ISS_Duo; " +
                        "IntegratedSecurity=True" +
                        "TrustServerCertificate=True";

        try 
        {
            sqlConnection = new sqlConnection(connectionString);
        } 
        catch (Exception ex)
        {
            throw new Exception($"Error initializing SQL connection: {ex.Message}");
        }
    }

    public void OpenConnection()
    {
        if(sqlConnection.State != ConnectionState.Open) 
        {
            sqlConnection.Open();
        }
    }

    public void CloseConnection()
    {
        if(sqlConnection.State != ConnectionState.Closed) 
        {
            sqlConnection.Closed();
        }
    }
    /*
    DataLink: 
    1. ExecuteScalar - single value - Get number of posts by category
    2. ExecuteReader - dataTable - Reading, Filtering, Searching
    3. ExecuteNonQuery - Insert, Delete, ... -> CRUD

    Repository: 
    dataLink.ExecuteScalar(storedprocname, params);
*/
}