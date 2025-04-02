using System;
using Microsoft.Data.SqlClient;


public class Connection
{

    private string Base;
    private string Server;
    private string User;
    private string Password;
    private static Connection Con = null;

    private Connection()
    {
        this.Server = "val\\MSSQL2022";
        this.Base = "bd_wallet";
        this.User = "project_master";
        this.Password = "1234";
    }

    public SqlConnection createConnection()
    {

        SqlConnection Chain = new SqlConnection();

        try
        {
            Chain.ConnectionString = "Server=" + this.Server +
                                     "; Database=" + this.Base +
                                     "; User Id=" + this.User +
                                     "; Password=" + this.Password +
                                     "; TrustServerCertificate=True";
        }
        catch (Exception ex)
        {
            Chain = null;
            throw ex;
        }

        return Chain;
    }

    public static Connection createInstance()
    {

        if (Con == null)
        {
            Con = new Connection();
        }

        return Con;
    }

}
