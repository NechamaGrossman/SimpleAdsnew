using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace SimpleAdsNew.Data
{
    public class UserDb
    {
        string _connectionString;
        public UserDb(string ConnectionString)
        {
            _connectionString = ConnectionString;
        }
        public void AddUser(User user, string password)
        {
            string hash = BCrypt.Net.BCrypt.HashPassword(password);
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO UserInfo (Name, Email, PasswordHash) " +
                                  "VALUES (@name, @email, @hash) SELECT SCOPE_IDENTITY()";
                cmd.Parameters.AddWithValue("@name", user.Name);
                cmd.Parameters.AddWithValue("@email", user.Email);
                cmd.Parameters.AddWithValue("@hash", hash);
                connection.Open();
                user.Id = (int)(decimal)cmd.ExecuteScalar();
            }
        }
        public User Login(string Email, string password)
        {
            User user = GetByEmail(Email);
            if (user == null)
            {
                return null;
            }
            bool isValidPassword = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (isValidPassword)
            {
                return user;
            }
            return null;
        }
        public User GetByEmail(string Email)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"select * from userInfo 
                                    where userInfo.Email =@email";
                cmd.Parameters.AddWithValue("@email", Email);
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                User user = new User();
                if (!reader.Read())
                {
                    return null;
                }
                return new User
                {
                    Email = (string)reader["email"],
                    Name = (string)reader["Name"],
                    PasswordHash = (string)reader["PasswordHash"],
                    Id = (int)reader["id"]
                };
            }
        }
    }
}
