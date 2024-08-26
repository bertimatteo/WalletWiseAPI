using Microsoft.Data.SqlClient;
using System.Data;
using WalletWise.Model.BalanceModels;
using WalletWise.Model.UserModels;

namespace WalletWise.Repository.BalanceRepository
{
    public class ItemRepository : IRepository<Item>
    {
        private readonly string _connectionString;

        public ItemRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<long> DeleteAsync(Item item)
        {
            if (item == null)
                throw new ArgumentException();

            long result = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    using (var cmd = new SqlCommand(Constants.DELETE_ITEM_SP, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("id ", SqlDbType.BigInt).Value = item.Id;

                        var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.BigInt);
                        returnParameter.Direction = ParameterDirection.ReturnValue;

                        await cmd.ExecuteNonQueryAsync();

                        result = Convert.ToInt64(returnParameter.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error class: {nameof(ItemRepository)}, method: {nameof(DeleteAsync)}, error: {ex.Message}");
            }

            return result;
        }

        public async Task<List<Item>> GetAllAsync() => await GetAllAsync(null, null, null, null);

        public async Task<List<Item>> GetAllAsync(long? userId = null, long? categoryId = null, DateTime? startDate = null, DateTime? endDate = null) 
        {
            List<Item> results = new List<Item>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    using (var cmd = new SqlCommand(Constants.GET_ITEMS_SP, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@userId",     SqlDbType.BigInt).Value   = userId;
                        cmd.Parameters.Add("@categoryId", SqlDbType.BigInt).Value   = categoryId;
                        cmd.Parameters.Add("@startDate",  SqlDbType.DateTime).Value = startDate;
                        cmd.Parameters.Add("@endDate",    SqlDbType.DateTime).Value = endDate;

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
                Console.WriteLine($"Error class: {nameof(ItemRepository)}, method: {nameof(GetAllAsync)}, error: {ex.Message}");
            }

            return results;
        }

        public async Task<Item?> GetByIdAsync(long id)
        {
            Item? results = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    using (var cmd = new SqlCommand(Constants.GET_ITEM_BY_ID_SP, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@id", SqlDbType.BigInt).Value = id;
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var item = InternalReader(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error class: {nameof(ItemRepository)}, method: {nameof(GetByIdAsync)}, error: {ex.Message}");
            }

            return results;
        }

        public async Task<long> InsertAsync(Item item)
        {
            if (item == null)
                throw new ArgumentException();

            long result = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    using (var cmd = new SqlCommand(Constants.INSERT_ITEM_SP, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@userId ",     SqlDbType.BigInt).Value   = item.User.Id;
                        cmd.Parameters.Add("@categoryId",  SqlDbType.BigInt).Value   = item.Category.Id;
                        cmd.Parameters.Add("@description", SqlDbType.NVarChar).Value = item.Description;
                        cmd.Parameters.Add("@amount",      SqlDbType.Float).Value    = item.Amount;
                        cmd.Parameters.Add("@date",        SqlDbType.DateTime).Value = item.Date;

                        var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.BigInt);
                        returnParameter.Direction = ParameterDirection.ReturnValue;

                        await cmd.ExecuteNonQueryAsync();

                        result = Convert.ToInt64(returnParameter.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error class: {nameof(ItemRepository)}, method: {nameof(InsertAsync)}, error: {ex.Message}");
            }

            return result;
        }

        public async Task<long> UpdateAsync(Item item)
        {
            if (item == null)
                throw new ArgumentException();

            long result = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    using (var cmd = new SqlCommand(Constants.UPDATE_ITEM_SP, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@id ",         SqlDbType.BigInt).Value   = item.Id;
                        cmd.Parameters.Add("@description", SqlDbType.NVarChar).Value = item.Description;
                        cmd.Parameters.Add("@amount",      SqlDbType.Float).Value    = item.Amount;
                        cmd.Parameters.Add("@date",        SqlDbType.DateTime).Value = item.Date;

                        var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.BigInt);
                        returnParameter.Direction = ParameterDirection.ReturnValue;

                        await cmd.ExecuteNonQueryAsync();

                        result = Convert.ToInt64(returnParameter.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error class: {nameof(ItemRepository)}, method: {nameof(InsertAsync)}, error: {ex.Message}");
            }

            return result;
        }

        private Item InternalReader(SqlDataReader reader)
        {
            if (reader == null)
                throw new ArgumentException();

            return new Item()
            {
                Id          = (long)reader["Id"],
                User        = new User() { Id = (long)reader["UserId"] },
                Category    = new Category() { Id = (long)reader["CategoryId"] },
                Description = (string)reader["Description"],
                Amount      = (double)reader["Amount"],
                Date        = (DateTime)reader["Date"]
            };
        }
    }
}
