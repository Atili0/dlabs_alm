namespace Deloitte.Labs
{
    using System;
    using Microsoft.Xrm.Sdk.Query;
    using Microsoft.Crm.Sdk.Messages;
    using Microsoft.Xrm.Sdk;

    public class DataExporter : IDataExporter
    {
        public EntityCollection GetData(ObjectExporter pExport)
        {
            EntityCollection _entityCollection;

            if (string.IsNullOrEmpty(pExport.Fetch))
            {
                QueryExpression _query = new QueryExpression(pExport.Entity);
                _entityCollection = pExport.ServiceClient.RetrieveMultiple(_query);
            }
            else
            {
                FetchXmlToQueryExpressionRequest _fetchXmlToQueryExpression = new FetchXmlToQueryExpressionRequest()
                {
                    FetchXml = pExport.Fetch
                };
                FetchXmlToQueryExpressionResponse _fetchXmlToQueryExpressionResponse =
                    (FetchXmlToQueryExpressionResponse)pExport.ServiceClient.Execute(_fetchXmlToQueryExpression);

                _entityCollection = pExport.ServiceClient.RetrieveMultiple(_fetchXmlToQueryExpressionResponse.Query);
            }

            return _entityCollection;

        }
    }
}
