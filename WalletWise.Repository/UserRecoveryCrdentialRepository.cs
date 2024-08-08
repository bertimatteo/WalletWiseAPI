using Microsoft.Data.SqlClient;
using System.Data;
using WalletWise.Model.User;

namespace WalletWise.Repository
{
    public class UserRecoveryCrdentialRepository : IRepository<UserCredentialRecovery>
    {
        private readonly string _connectionString;

        public UserRecoveryCrdentialRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Task<long> DeleteAsync(UserCredentialRecovery item) => throw new NotImplementedException();

        public Task<List<UserCredentialRecovery>> GetAllAsync() => throw new NotImplementedException();

        public Task<UserCredentialRecovery?> GetByIdAsync(long id) => throw new NotImplementedException();

        public async Task<UserCredentialRecovery?> UserRecoveryCrdentialByUsername(long id)
        {
            UserCredentialRecovery? result = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    using (var cmd = new SqlCommand(Constants.GET_USER_CREDENTIAL_RECOVERY_BY_USER_ID_SP, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@userId", SqlDbType.NVarChar).Value = id;

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
                Console.WriteLine($"Error class: {nameof(UserRecoveryCrdentialRepository)}, method: {nameof(UserRecoveryCrdentialByUsername)}, error: {ex.Message}");
            }

            return result;
        }

        public async Task<long> InsertAsync(UserCredentialRecovery item)
        {
            if (item == null)
                throw new ArgumentException();

            long result = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    using (var cmd = new SqlCommand(Constants.INSERT_USER_CREDENTIAL_RECOVERY_SP, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@userId",   SqlDbType.BigInt).Value   = item.User.Id;
                        cmd.Parameters.Add("@question", SqlDbType.NVarChar).Value = item.Question;
                        cmd.Parameters.Add("@response", SqlDbType.NVarChar).Value = item.Response;

                        var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.BigInt);
                        returnParameter.Direction = ParameterDirection.ReturnValue;

                        await cmd.ExecuteNonQueryAsync();

                        result = Convert.ToInt64(returnParameter.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error class: {nameof(UserRecoveryCrdentialRepository)}, method: {nameof(InsertAsync)}, error: {ex.Message}");
            }

            return result;
        }

        public async Task<long> UpdateAsync(UserCredentialRecovery item)
        {
            if (item == null)
                throw new ArgumentException();

            long result = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    using (var cmd = new SqlCommand(Constants.UPDATE_USER_CREDENTIAL_RECOVERY_SP, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@userId",   SqlDbType.BigInt).Value   = item.User.Id;
                        cmd.Parameters.Add("@question", SqlDbType.NVarChar).Value = item.Question;
                        cmd.Parameters.Add("@response", SqlDbType.NVarChar).Value = item.Response;

                        var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.BigInt);
                        returnParameter.Direction = ParameterDirection.ReturnValue;

                        await cmd.ExecuteNonQueryAsync();

                        result = Convert.ToInt64(returnParameter.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error class: {nameof(UserRecoveryCrdentialRepository)}, method: {nameof(UpdateAsync)}, error: {ex.Message}");
            }

            return result;
        }

        private UserCredentialRecovery InternalReader(SqlDataReader reader)
        {
            if (reader == null)
                throw new ArgumentException();

            return new UserCredentialRecovery()
            {
                Id       = (long)reader["Id"],
                User     = new User() 
                {
                    Id = (long)reader["UserId"]
                },
                Question = (string)reader["Question"],
                Response = (string)reader["Response"],
            };
        }
    }
}
