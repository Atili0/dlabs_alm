namespace Deloitte.Labs
{
    using System;

    public class Export : IExport
    {
        private ObjectExporter _export;


        public Export(ObjectExporter pExport)
        {
            _export = pExport;
        }

        public void ExportToUncompressed()
        {
            string _fullpath;

            CreateDataFile();
        }

        public void ExportTocompressed()
        {
            CreateDataFile();

            Common.Compress(String.Format("{0}.zip",_export.FullPath));
            System.IO.File.Delete(_export.FullPath);

        }

        public void CreateDataFile()
        {
            var _json = string.Empty;
            var _fullpath = string.Empty;

            IDataExporter _dataexporter = new DataExporter();

            var _retrieved = _dataexporter.GetData(this._export);
            var _entitySerializer = new EntitySerializer();

            foreach (var retrievedEntity in _retrieved.Entities)
            {
                _json = _entitySerializer.SerializeObject(retrievedEntity);

                if (!String.IsNullOrEmpty(_export.Path))
                    _export.CompletePath = Common.CreateFolder(_export.Path, _export.Entity);
                else _export.CompletePath = Common.CreateFolder(_export.Entity);

                _export.FullPath = Common.CreateFile(_export);
                _export.index++;
            }

        }

    }
}
