using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Config;

namespace VintageAuth
{
public class DBhandler {
   public static bool InitDB() {
      SQLiteConnection conn = CreateConnection();
      if (conn == null) {
         return false;
      }
      CreateTable();
      CreateTokenTable();
      return true;
   }

    public static SQLiteConnection CreateConnection()
      {

         SQLiteConnection sqlite_conn;
         // Create a new database connection:
         string db_path = Path.Combine(GamePaths.DataPath, VintageAuth.vaConfig.database_name);
         Console.WriteLine($"VintageAuth: Got database path: {db_path}");
         sqlite_conn = new SQLiteConnection($"Data Source={db_path};Version=3;New=True;Compress=True;");
         // Open the connection:
         try
         {
            sqlite_conn.Open();
         }
         catch (Exception ex)
         {
            Console.WriteLine("VintageAuth: Failed to create database!");
            return null;
         }
         return sqlite_conn;
      }

      static void CreateTable()
      {
         SQLiteConnection conn = CreateConnection();
         if (conn == null) {
            return;
         }
         using (SQLiteCommand command = new SQLiteCommand(conn))
         {
            command.CommandText = "CREATE TABLE IF NOT EXISTS DATA (id INTEGER PRIMARY KEY AUTOINCREMENT, uuid TEXT, name TEXT, active BOOLEAN, hash TEXT, salt TEXT, role TEXT);";
            command.ExecuteNonQuery();
         }
         conn.Close();
      }

      static void CreateTokenTable()
      {
         SQLiteConnection conn = CreateConnection();
         if (conn == null)
         {
            return;
         }
         using (SQLiteCommand command = new SQLiteCommand(conn))
         {
            command.CommandText = "CREATE TABLE IF NOT EXISTS TOKEN (id INTEGER PRIMARY KEY AUTOINCREMENT, value TEXT UNIQUE);";
            command.ExecuteNonQuery();
         }
         conn.Close();
      }

      public static User GetByUsername(string username)
      {
         SQLiteConnection conn = CreateConnection();
         if (conn == null) {
            return null;
         }
         User res;
         using (SQLiteCommand command = new SQLiteCommand(conn))
         {
            command.CommandText = "SELECT * FROM DATA WHERE name = @name LIMIT 1;";
            command.Parameters.AddWithValue("@name", username);

            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                  if (reader.Read())
                  {
                     int id = reader.GetInt32(0);
                     string uuid = reader.GetString(1);
                     string name = reader.GetString(2);
                     bool active = reader.GetBoolean(3);
                     string hash = reader.GetString(4);
                     string salt = reader.GetString(5);
                     string role = reader.GetString(6);
                     res = new User(id, uuid, name, active, hash, salt, role);
                  }
                  else
                  {
                     res = null;
                  }
            }
         }
         conn.Close();
         return res;
      }

      public static User GetByUuid(string uuid)
      {
         SQLiteConnection conn = CreateConnection();
         if (conn == null) {
            return null;
         }
         User res;
         using (SQLiteCommand command = new SQLiteCommand(conn))
         {
            command.CommandText = "SELECT * FROM DATA WHERE uuid = @uuid LIMIT 1;";
            command.Parameters.AddWithValue("@uuid", uuid);

            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                  if (reader.Read())
                  {
                     int id = reader.GetInt32(0);
                     string uuid_ = reader.GetString(1);
                     string name = reader.GetString(2);
                     bool active = reader.GetBoolean(3);
                     string hash = reader.GetString(4);
                     string salt = reader.GetString(5);
                     string role = reader.GetString(6);
                     res = new User(id, uuid, name, active, hash, salt, role);
                  }
                  else
                  {
                     res = null;
                  }
            }
         }
         conn.Close();
         return res;
      }

      public static bool insertUser(string username, string uuid, string password, bool active, string role) {
         SQLiteConnection conn = CreateConnection();
         if (conn == null)
         {
            return false;
         }
         string salt = PasswordUtil.getSalt();
         string hash = PasswordUtil.getHash(password, salt);
         using (SQLiteCommand command = new SQLiteCommand(conn))
         {
            command.CommandText = "INSERT INTO DATA (uuid, name, active, hash, salt, role) VALUES (@uuid, @name, @active, @hash, @salt, @role)";
            command.Parameters.AddWithValue("@uuid", uuid);
            command.Parameters.AddWithValue("@name", username);
            command.Parameters.AddWithValue("@active", active);
            command.Parameters.AddWithValue("@hash", hash);
            command.Parameters.AddWithValue("@salt", salt);
            command.Parameters.AddWithValue("@role", role);
            int rowsAffected = command.ExecuteNonQuery();
            conn.Close();
            return rowsAffected == 1;
         }
      }

      public static bool updatePassword(string username, string newpassword)
      {
         SQLiteConnection conn = CreateConnection();
         if (conn == null)
         {
            return false;
         }
         
         string salt = PasswordUtil.getSalt();
         string hash = PasswordUtil.getHash(newpassword, salt);
         
         using (SQLiteCommand command = new SQLiteCommand(conn))
         {
            command.CommandText = "UPDATE DATA SET hash = @hash, salt = @salt WHERE name = @username";
            command.Parameters.AddWithValue("@hash", hash);
            command.Parameters.AddWithValue("@salt", salt);
            command.Parameters.AddWithValue("@username", username);
            
            int rowsAffected = command.ExecuteNonQuery();
            conn.Close();
            return rowsAffected == 1;
         }
      }

      public static bool deleteAccount(string username)
      {
         SQLiteConnection conn = CreateConnection();
         if (conn == null)
         {
            return false;
         }
         using (SQLiteCommand command = new SQLiteCommand(conn))
         {
            command.CommandText = "DELETE FROM DATA WHERE name = @name";
            command.Parameters.AddWithValue("@name", username);
            int rowsAffected = command.ExecuteNonQuery();
            conn.Close();
            return rowsAffected == 1;
         }
      }

      public static bool changeRole(string username, string role) {
      SQLiteConnection conn = CreateConnection();
      if (conn == null) {
         return false;
      }
      using (SQLiteCommand command = new SQLiteCommand(conn)) {
         command.CommandText = "UPDATE DATA SET role=@role WHERE name=@name";
         command.Parameters.AddWithValue("@name", username);
         command.Parameters.AddWithValue("@role", role);
         int rowsAffected = command.ExecuteNonQuery();
         conn.Close();
         return rowsAffected == 1;
      }
   }

      public static bool passwordValid(string username, string password) {
         User usr = GetByUsername(username);
         if (usr == null) {
            return false;
         }
         return usr.Hash.Equals(PasswordUtil.getHash(password, usr.Salt));
      }

      private static string GenerateRandomToken(int length)
      {
         const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
         StringBuilder sb = new StringBuilder();
         Random random = new Random();
         for (int i = 0; i < length; i++)
         {
            sb.Append(validChars[random.Next(validChars.Length)]);
         }
         return sb.ToString();
      }
   
      public static string addToken()
      {
         string token = GenerateRandomToken(16);
         SQLiteConnection conn = CreateConnection();
         if (conn == null)
         {
            return null;
         }
         using (SQLiteCommand command = new SQLiteCommand(conn))
         {
            command.CommandText = "INSERT INTO TOKEN (value) VALUES (@value);";
            command.Parameters.AddWithValue("@value", token);
            command.ExecuteNonQuery();
         }
         conn.Close();
         return token;
      }

      public static bool removeToken(string token)
      {
         SQLiteConnection conn = CreateConnection();
         if (conn == null)
         {
            return false;
         }
         using (SQLiteCommand command = new SQLiteCommand(conn))
         {
            command.CommandText = "DELETE FROM TOKEN WHERE value=@value;";
            command.Parameters.AddWithValue("@value", token);
            int rowsAffected = command.ExecuteNonQuery();
            conn.Close();
            return rowsAffected == 1;
         }
      }

      public static bool isValidToken(string token)
      {
         SQLiteConnection conn = CreateConnection();
         if (conn == null)
         {
            return false;
         }

         using (SQLiteCommand command = new SQLiteCommand(conn))
         {
            command.CommandText = "SELECT COUNT(*) FROM TOKEN WHERE value = @token;";
            command.Parameters.AddWithValue("@token", token);
            int count = Convert.ToInt32(command.ExecuteScalar());
            conn.Close();

            return count == 1;
         }
      }

      public static bool deactivateAccount(string username) {
         SQLiteConnection conn = CreateConnection();
         if (conn == null) {
            return false;
         }
         using (SQLiteCommand command = new SQLiteCommand(conn)) {
            command.CommandText = "UPDATE DATA SET active = 0 WHERE name = @name";
            command.Parameters.AddWithValue("@name", username);
            int rowsAffected = command.ExecuteNonQuery();
            conn.Close();
            return rowsAffected == 1;
         }
      }

      public static bool activateAccount(string username) {
         SQLiteConnection conn = CreateConnection();
         if (conn == null) {
            return false;
         }
         using (SQLiteCommand command = new SQLiteCommand(conn)) {
            command.CommandText = "UPDATE DATA SET active = 1 WHERE name = @name";
            command.Parameters.AddWithValue("@name", username);
            int rowsAffected = command.ExecuteNonQuery();
            conn.Close();
            return rowsAffected == 1;
         }
      }
   }
}