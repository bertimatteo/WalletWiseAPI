using Microsoft.Data.SqlClient;
using System.Data;
using WalletWise.Model.UserModels;

namespace WalletWise.Repository.UserRepository
{
    public class UserRepository : IRepository<User>
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Task<long> DeleteAsync(User item) => throw new NotImplementedException();

        public Task<List<User>> GetAllAsync() => throw new NotImplementedException();

        public Task<User?> GetByIdAsync(long id) => throw new NotImplementedException();

        public async Task<User?> GetUserByUsername(string username)
        {
            User? result = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    using (var cmd = new SqlCommand(Constants.GET_USER_BY_USERNAME_SP, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@username", SqlDbType.NVarChar).Value = username;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                result = InternalReader(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error class: {nameof(UserRepository)}, method: {nameof(GetByIdAsync)}, error: {ex.Message}");
            }

            return result;
        }

        public async Task<long> InsertAsync(User item)
        {
            if (item == null)
                throw new ArgumentException();

            long result = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    using (var cmd = new SqlCommand(Constants.INSERT_USER_SP, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@username", SqlDbType.NVarChar).Value = item.Username;
                        cmd.Parameters.Add("@email", SqlDbType.NVarChar).Value = item.Email;
                        cmd.Parameters.Add("@passwordSalt", SqlDbType.NVarChar).Value = item.PasswordSalt;
                        cmd.Parameters.Add("@password", SqlDbType.NVarChar).Value = item.Password;

                        var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.BigInt);
                        returnParameter.Direction = ParameterDirection.ReturnValue;

                        await cmd.ExecuteNonQueryAsync();

                        result = Convert.ToInt64(returnParameter.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error class: {nameof(UserRepository)}, method: {nameof(InsertAsync)}, error: {ex.Message}");
            }

            return result;
        }

        public async Task<long> UpdateAsync(User item)
        {
            if (item == null)
                throw new ArgumentException();

            long result = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    using (var cmd = new SqlCommand(Constants.UPDATE_USER_SP, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@username", SqlDbType.NVarChar).Value = item.Username;
                        cmd.Parameters.Add("@passwordSalt", SqlDbType.NVarChar).Value = item.PasswordSalt;
                        cmd.Parameters.Add("@password", SqlDbType.NVarChar).Value = item.Password;

                        var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.BigInt);
                        returnParameter.Direction = ParameterDirection.ReturnValue;

                        await cmd.ExecuteNonQueryAsync();

                        result = Convert.ToInt64(returnParameter.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error class: {nameof(UserRepository)}, method: {nameof(UpdateAsync)}, error: {ex.Message}");
            }

            return result;
        }

        private User InternalReader(SqlDataReader reader)
        {
            if (reader == null)
                throw new ArgumentException();

            return new User()
            {
                Id = (long)reader["Id"],
                Username = (string)reader["Username"],
                Email = (string)reader["Email"],
                PasswordSalt = (string)reader["PasswordSalt"],
                Password = (string)reader["Password"]
            };
        }
    }
}
