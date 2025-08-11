namespace LibraryMgmt.Repository.Interfaces
{
    public interface IRepository<T> where T : class
    {
        ICollection<T> GetAll();
     
        bool Save();
    }
}
