using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Frontend.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _appDataDirectory;
        private readonly ILogger _logger;
        public FileStorageService(ILogger<FileStorageService> logger) {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _appDataDirectory = Path.Combine(appDataPath, "savedPuzzleGames");
            if (!Directory.Exists(_appDataDirectory)) Directory.CreateDirectory(_appDataDirectory);
            _logger = logger;
        }
        public async Task<T?> LoadAsync<T>(string fileName)
        {
            var filePath = Path.Combine(_appDataDirectory, fileName);
            if (!File.Exists(filePath))
            {
                _logger.LogInformation($"{fileName} file path does not exist.");
                return default;
            }
            var json = await File.ReadAllTextAsync(filePath);
            if (json == null) { return default; }
            return JsonSerializer.Deserialize<T>(json);
        }

        public async Task SaveAsync<T>(string fileName, T data)
        {
            if(data == null)
            {
                return;
            }
            try
            {
                var filePath = Path.Combine(_appDataDirectory, fileName);
                var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                if (Directory.Exists(_appDataDirectory))
                {
                    _logger.LogInformation($"directory exists: {Directory.Exists(_appDataDirectory)}");
                    await File.WriteAllTextAsync(filePath, json);
                    _logger.LogInformation($"Saved to: {filePath}");
                }
                else
                {
                    _logger.LogCritical($"Filepath {fileName} does not exists!");
                }

            }
            catch (Exception ex) {
                _logger.LogCritical($"Failed to save: {data} to filepath {fileName}");
            }
        }
        public async Task DeleteAsync(string fileName)
        {
            var filePath = Path.Combine(_appDataDirectory, fileName);
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    _logger.LogInformation($"File deleted: {fileName}");
                }
                else
                {
                    _logger.LogCritical($"Attempted to delete non-existing faile name: {fileName}");
                }
            }catch(Exception ex) {
                _logger.LogCritical($"Failed to delete file {fileName}");
            }
        }
    }
}
