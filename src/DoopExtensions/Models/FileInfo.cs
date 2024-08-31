namespace DoopExtensions.Models
{
    public class FileInfo
    {
        public FileInfo(string fileName)
        {
            FileName = fileName;
        }

        public string FileName { get; set; }
        public string? InitialFilePath { get; set; }
        public string? FinalFilePath { get; set; }
    }
}