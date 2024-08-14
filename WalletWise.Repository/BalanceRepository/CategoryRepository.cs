using Microsoft.Data.SqlClient;
using System.Data;
using WalletWise.Model.BalanceModels;
using WalletWise.Model.UserModels;

namespace WalletWise.Repository.BalanceRepository
{
    public class CategoryRepository : IRepository<Category>
    {
        private readonly string _connectionString;

        public CategoryRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<long> DeleteAsync(Category item)
        {
            if (item == null)
                throw new ArgumentException();

            long result = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    using (var cmd = new SqlCommand(Constants.UPDATE_CATEGORY_SP, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("id ", SqlDbType.BigInt).Value = item.Id;
                        cmd.Parameters.Add("@description", SqlDbType.NVarChar).Value = item.Description;
                        cmd.Parameters.Add("@icon", SqlDbType.NVarChar).Value = item.Icon;
                        cmd.Parameters.Add("@colorBackground", SqlDbType.NVarChar).Value = item.ColorBackground;
                        cmd.Parameters.Add("@isDeleted ", SqlDbType.Bit).Value = true;

                        var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.BigInt);
                        returnParameter.Direction = ParameterDirection.ReturnValue;

                        await cmd.ExecuteNonQueryAsync();

                        result = Convert.ToInt64(returnParameter.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error class: {nameof(CategoryRepository)}, method: {nameof(DeleteAsync)}, error: {ex.Message}");
            }

            return result;
        }

        public Task<List<Category>> GetAllAsync() => throw new NotImplementedException();

        public async Task<List<Category>> GetAllAsync(long userId, CategoryType? type = null, bool isDeleted = false)
        {
            List<Category> results = new List<Category>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    using (var cmd = new SqlCommand(Constants.GET_CATEGORIES_BY_USER_SP, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@userId",    SqlDbType.BigInt).Value   = userId;
                        cmd.Parameters.Add("@type",      SqlDbType.SmallInt).Value = type != null ? type : null;
                        cmd.Parameters.Add("@isDeleted", SqlDbType.Bit).Value      = isDeleted;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var item = InternalReader(reader);
                                if (item != null)
                                    results.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error class: {nameof(CategoryRepository)}, method: {nameof(GetAllAsync)}, error: {ex.Message}");
            }

            return results;
        }

        public Task<Category?> GetByIdAsync(long id) => throw new NotImplementedException();

        public async Task<long> InsertAsync(Category item)
        {
            if (item == null)
                throw new ArgumentException();

            long result = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    using (var cmd = new SqlCommand(Constants.INSERT_CATEGORY_SP, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("userId ",          SqlDbType.BigInt).Value   = item.User.Id;
                        cmd.Parameters.Add("@description",     SqlDbType.NVarChar).Value = item.Description;
                        cmd.Parameters.Add("@type",            SqlDbType.SmallInt).Value = item.CategoryType;
                        cmd.Parameters.Add("@icon",            SqlDbType.NVarChar).Value = item.Icon;
                        cmd.Parameters.Add("@colorBackground", SqlDbType.NVarChar).Value = item.ColorBackground;
                        cmd.Parameters.Add("@isDeleted ",      SqlDbType.Bit).Value      = item.IsDeleted;

                        var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.BigInt);
                        returnParameter.Direction = ParameterDirection.ReturnValue;

                        await cmd.ExecuteNonQueryAsync();

                        result = Convert.ToInt64(returnParameter.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error class: {nameof(CategoryRepository)}, method: {nameof(InsertAsync)}, error: {ex.Message}");
            }

            return result;
        }

        public async Task<long> UpdateAsync(Category item)
        {
            if (item == null)
                throw new ArgumentException();

            long result = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    using (var cmd = new SqlCommand(Constants.UPDATE_CATEGORY_SP, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("id ",              SqlDbType.BigInt).Value   = item.Id;
                        cmd.Parameters.Add("@description",     SqlDbType.NVarChar).Value = item.Description;
                        cmd.Parameters.Add("@icon",            SqlDbType.NVarChar).Value = item.Icon;
                        cmd.Parameters.Add("@colorBackground", SqlDbType.NVarChar).Value = item.ColorBackground;
                        cmd.Parameters.Add("@isDeleted ",      SqlDbType.Bit).Value      = item.IsDeleted;

                        var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.BigInt);
                        returnParameter.Direction = ParameterDirection.ReturnValue;

                        await cmd.ExecuteNonQueryAsync();

                        result = Convert.ToInt64(returnParameter.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error class: {nameof(CategoryRepository)}, method: {nameof(UpdateAsync)}, error: {ex.Message}");
            }

            return result;
        }

        private Category InternalReader(SqlDataReader reader)
        {
            if (reader == null)
                throw new ArgumentException();

            return new Category()
            {
                Id              = (long)reader["Id"],
                User            = new User() { Id = (long)reader["UserId"] },
                Description     = (string)reader["Description"],
                CategoryType    = (CategoryType)((Int16)reader["Type"]),
                Icon            = (string)reader["Icon"],
                ColorBackground = (string)reader["ColorBackground"],
                IsDeleted       = (bool)reader["IsDeleted"]
            };
        }
    }
}
