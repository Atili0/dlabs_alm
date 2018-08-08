using Microsoft.Xrm.Sdk;

namespace Deloitte.Labs
{
    public interface IDataExporter
    {
        EntityCollection GetData(ObjectExporter pExport);
    }
}