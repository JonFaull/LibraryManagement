namespace LibraryMgmt.Repository.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<ICollection<T>> GetAllAsync();
     
        Task<bool> SaveAsync();
    }
}
