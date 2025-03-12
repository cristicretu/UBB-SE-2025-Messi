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
    
    public T ExecuteScalar<T> (string storedProcedure, SqlParameters[] sqlParameters)
    {
        try
        {
            using (SqlCommand command = CreateCommand(storedProcedure, sqlParameters))
            {
                var result = command.ExecuteScalar(storedProcedure, sqlParameters);
                if (result == DBNull.Value || result == null)
                {
                    return (T) Convert.ChangeType(result, T);
                }

                return result;
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error executing scalar: {ex.Message}");
        }
    }

    public DataTable ExecuteReader(string storedProcedure, SqlParameters[] sqlParameters)
    {
        try 
        {
            using (SqlCommand command = CreateCommand(storedProcedure, sqlParameters))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    DataTable dataTable = new DataTable;
                    dataTable.Load(reader);
                    return dataTable;
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error executing reader: {ex.Message}");
        }
    }
    
    public int ExecuteNonQuery(string storedProcedure, SqlParameters[] sqlParameters)
    {
        try {
            using (SqlCommand sqlCommand = SqlCommand(storedProcedure, sqlParameters))
            {
                return sqlCommand.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            throw new Exeception($"Error - ExecuteNonQuery: {ex.Message}"); 
        }
    }
}