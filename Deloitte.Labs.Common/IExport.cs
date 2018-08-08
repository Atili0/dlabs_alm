namespace Deloitte.Labs
{
    public interface IExport
    {
        void ExportToUncompressed();
        void CreateDataFile();
        void ExportTocompressed();
    }
}