using Oracle.ManagedDataAccess.Client;

namespace TsrmWebApi.Helper
{
    public static class OracleParameterExtensions
    {
        public static OracleParameter ConfigureArray<T>(this OracleParameter param, IList<T> list)
        {
            param.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
            param.Size = list.Count;
            param.Value = list.ToArray();

            // Optional: add null value handling for VARCHAR2
            if (typeof(T) == typeof(string))
            {
                param.ArrayBindSize = list.Select(x => 4000).ToArray(); // max length
            }

            return param;
        }
    }

}
