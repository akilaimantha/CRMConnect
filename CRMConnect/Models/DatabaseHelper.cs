using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace CRMConnect.Models;

public class DatabaseHelper
{
    private static string connectionString = "User Id=system;Password=Oracle123;Data Source=localhost:1521/XE;";
    
    public static DataTable ExecuteQuery(string query, OracleParameter[]? parameters = null)
    {
        using var conn = new OracleConnection(connectionString);
        conn.Open();
        using var cmd = new OracleCommand(query, conn);
        if (parameters != null)
            cmd.Parameters.AddRange(parameters);
        
        using var adapter = new OracleDataAdapter(cmd);
        var dt = new DataTable();
        adapter.Fill(dt);
        return dt;
    }
    
    public static int ExecuteNonQuery(string query, OracleParameter[]? parameters = null)
    {
        using var conn = new OracleConnection(connectionString);
        conn.Open();
        using var cmd = new OracleCommand(query, conn);
        if (parameters != null)
            cmd.Parameters.AddRange(parameters);
        return cmd.ExecuteNonQuery();
    }
}