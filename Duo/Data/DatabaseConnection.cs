using Microsoft.Data.SqlClient;
using System.Data;

public DataLink 
{
    SqlConnection sqlConnection; 

    public DataLink 
    {
        sqlConnection = new sqlConnection("DataSource=np:\\.\pipe\LOCALDB#D6D39D8B\tsql\query;" +
                                        "Initial Catalog=Categories; " +
                                        "IntegratedSecurity=True"
                                        "TrustServerCertificate=True")
    }
}