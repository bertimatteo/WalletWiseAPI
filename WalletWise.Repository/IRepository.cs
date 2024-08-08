namespace WalletWise.Repository
{
    public interface IRepository<T>
    {
        public Task<List<T>> GetAllAsync();
        public Task<T?> GetByIdAsync(long id);
        public Task<long> InsertAsync(T item);
        public Task<long> UpdateAsync(T item);
        public Task<long> DeleteAsync(T item);
    }
}
