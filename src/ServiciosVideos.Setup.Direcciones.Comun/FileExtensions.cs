using System.Threading.Tasks;

namespace System.IO
{
    public static class FileExtensions
    {
        public static async Task WriteAllTextAsync(this string fileName, string content)
        {
            using (var writer = File.CreateText(fileName))
            {
                await writer.WriteAsync(content);
            }
        }

        public static async Task<string> ReadAllTextAsync(this string fileName)
        {
            using (var reader = File.OpenText(fileName))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}
