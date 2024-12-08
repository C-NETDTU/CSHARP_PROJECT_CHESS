using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Services
{
    public interface IFileStorageService
    {
        Task SaveAsync<T>(string fileName, T data);
        Task<T?> LoadAsync<T>(string fileName);
        Task DeleteAsync(string fileName);
    }
}
