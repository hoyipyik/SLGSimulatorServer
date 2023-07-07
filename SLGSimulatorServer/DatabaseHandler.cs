using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;

namespace SLGSimulatorServer
{
    class DatabaseHandler
    {
        readonly string connectionString = "Server=127.0.0.1;Port=3306;Uid=root;Pwd=123123123;";

        public DatabaseHandler(string connectionString = "Server=127.0.0.1;Port=3306;Uid=root;Pwd=123123123;")
        {
            this.connectionString = connectionString;
        }

        public bool ConnectionTest()
        {
            using (MySqlConnection connection = new MySqlConnection(this.connectionString)) {
                try
                {
                    connection.Open();
                    Utils.log.Info("Connection success using " + this.connectionString);
                    connection.Close();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool CreateDatabase(string databaseName)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(this.connectionString)) {
                    connection.Open();
                    string sqlString = "CREATE DATABASE IF NOT EXISTS " + databaseName + ";"; // Changed this line
                    MySqlCommand cmd = new MySqlCommand(sqlString, connection);
                    cmd.ExecuteNonQuery();
                    connection.Close();
                    Utils.log.Info("Database " + databaseName + " created XD");
                    return true;
                }
            }
            catch
            {
                return false;
            }

        }

        public bool CreateTable(string databaseName, string tableName, KeyValuePair<string, string>[] args)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(this.connectionString)) {
                    connection.Open();
                    connection.ChangeDatabase(databaseName);
                    StringBuilder sql = new StringBuilder("CREATE TABLE IF NOT EXISTS " + tableName + " (" +
                        "id INT AUTO_INCREMENT PRIMARY KEY, ");
                    for (int i = 0; i < args.Length; i++) // Changed the loop to use a standard for loop
                    {
                        var item = args[i];
                        if (item.Value == "string")
                        {
                            sql.Append(item.Key + " VARCHAR(255),");
                        }
                        else if (item.Value == "int")
                        {
                            sql.Append(item.Key + " INT,");
                        }
                        else if (item.Value == "float" || item.Value == "number")
                        {
                            sql.Append(item.Key + " DECIMAL(10, 2),");
                        }
                        else if (item.Value == "bool")
                        {
                            sql.Append(item.Key + " BOOLEAN NOT NULL,");
                        }
                    }
                    sql.Length--; // Remove the last comma
                    sql.Append(");"); // Changed this line

                    string sqlQuery = sql.ToString();
                    Utils.log.Info(sqlQuery);
                    MySqlCommand cmd = new MySqlCommand(sqlQuery, connection);
                    cmd.ExecuteNonQuery();
                    connection.Close();
                    return true;
                }
            }
            catch (MySqlException e)
            {
                Utils.log.Info(e);
                return false;
            }
        }

        public bool ExecuteUniversalCommand(string databaseName, string sqlQuery)
        {
            if (databaseName is null)
            {
                throw new ArgumentNullException(nameof(databaseName));
            }

            try
            {
                using (MySqlConnection connection = new MySqlConnection(this.connectionString)) {
                    connection.Open();
                    MySqlCommand cmd = new MySqlCommand(sqlQuery, connection);
                    cmd.ExecuteNonQuery();
                    connection.Close();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool InsertData(string databaseName, string tableName, KeyValuePair<string, object>[] args)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(this.connectionString)) {
                    connection.Open();
                    connection.ChangeDatabase(databaseName);
                    StringBuilder sql = new StringBuilder("INSERT INTO " + tableName + " (");
                    StringBuilder sqlValues = new StringBuilder("VALUES (");
                    for (int i = 0; i < args.Length; i++) // Changed the loop to use a standard for loop
                    {
                        var item = args[i];
                        sql.Append(item.Key + ",");
                        sqlValues.Append("@" + item.Key + ",");
                    }
                    sql.Length--; // Remove the last comma
                    sqlValues.Length--; // Remove the last comma
                    sql.Append(") ");
                    sqlValues.Append(");");
                    sql.Append(sqlValues);
                    string sqlQuery = sql.ToString();
                    Utils.log.Info(sqlQuery);
                    MySqlCommand cmd = new MySqlCommand(sqlQuery, connection);
                    for (int i = 0; i < args.Length; i++) // Changed the loop to use a standard for loop
                    {
                        var item = args[i];
                        cmd.Parameters.AddWithValue("@" + item.Key, item.Value);
                    }
                    cmd.ExecuteNonQuery();
                    connection.Close();
                    return true;
                }
            }
            catch (System.Exception e)
            {
                Utils.log.Info(e);
                return false;
            }

        }

        public bool DeleteUniversal(string databaseName, string tableName, string property, string value)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(this.connectionString)) {
                    connection.Open();
                    connection.ChangeDatabase(databaseName);
                    string sqlQuery = "DELETE FROM " + tableName + " WHERE " + property + " = '" + value + "';";
                    Utils.log.Info(sqlQuery);
                    MySqlCommand cmd = new MySqlCommand(sqlQuery, connection);
                    cmd.ExecuteNonQuery();
                    connection.Close();
                    return true;
                }
            }
            catch (System.Exception e)
            {
                Utils.log.Info(e);
                return false;
            }

        }

        public bool UpgradeUniversal(string databaseName, string tableName, string conditionProperty, string conditionValue, string property, string value)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(this.connectionString)) {
                    connection.Open();
                    connection.ChangeDatabase(databaseName);
                    string sqlQuery =
                        "UPDATE " + tableName + " SET " + property + " = '"
                        + value + "' WHERE " + conditionProperty + " = '" + conditionValue + "';";
                    MySqlCommand cmd = new MySqlCommand(sqlQuery, connection);
                    cmd.ExecuteNonQuery();
                    connection.Close();
                    return true;
                }
            }
            catch (System.Exception e)
            {
                Utils.log.Info(e);
                return false;
            }
        }

        public List<Dictionary<string, object>> QueryUniversal(string databaseName, string tableName, string property, string conditionProperty, string conditionValue)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(this.connectionString)) {
                    connection.Open();
                    connection.ChangeDatabase(databaseName);
                    string sqlQuery = "SELECT " + property + " FROM " + tableName + " WHERE " + conditionProperty + " = '" + conditionValue + "';";
                    MySqlCommand cmd = new MySqlCommand(sqlQuery, connection);
                    using (MySqlDataReader reader = cmd.ExecuteReader()) {
                        List<Dictionary<string, object>> dataList = new List<Dictionary<string, object>>();
                        while (reader.Read())
                        {
                            Dictionary<string, object> data = new Dictionary<string, object>() { };
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                data.Add(reader.GetName(i), reader.GetValue(i));
                            }
                            dataList.Add(data);
                        }
                        connection.Close();
                        return dataList;
                    }
                }
            }
            catch (System.Exception e)
            {
                Utils.log.Info(e);
                return null;
            }
        }
        public List<Dictionary<string, object>> QueryAll(string databaseName, string tableName)
        {
            try
            {
                List<Dictionary<string, object>> dataList = new List<Dictionary<string, object>>();
                using (MySqlConnection connection = new MySqlConnection(this.connectionString)) {
                    connection.Open();
                    connection.ChangeDatabase(databaseName);
                    string sqlQuery = "SELECT * FROM " + tableName + ";";
                    MySqlCommand cmd = new MySqlCommand(sqlQuery, connection);
                    using (MySqlDataReader reader = cmd.ExecuteReader()) {
                        while (reader.Read())
                        {
                            Dictionary<string, object> data = new Dictionary<string, object>() { };
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                data.Add(reader.GetName(i), reader.GetValue(i));
                            }
                            dataList.Add(data);
                        }
                        connection.Close();
                        return dataList;
                    }
                }
            }
            catch (System.Exception e)
            {
                Utils.log.Info(e);
                return null;
            }
        }

        public bool UpgradeById(string databaseName, string tableName, string id, string property, string value)
        {
            return UpgradeUniversal(databaseName, tableName, "id", id, property, value);
        }

        public bool UpgradeByName(string databaseName, string tableName, string name, string property, string value)
        {
            return UpgradeUniversal(databaseName, tableName, "name", name, property, value);
        }


        public bool DeleteByName(string databaseName, string tableName, string name)
        {
            return DeleteUniversal(databaseName, tableName, "name", name);
        }

        public bool DeleteById(string databaseName, string tableName, string id)
        {
            return DeleteUniversal(databaseName, tableName, "id", id);
        }
    }
}
