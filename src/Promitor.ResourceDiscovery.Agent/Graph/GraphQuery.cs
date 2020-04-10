using System.Linq;
using System.Text;

namespace Promitor.ResourceDiscovery.Agent.Graph
{
    public class GraphQuery
    {
        private readonly StringBuilder _queryBuilder;
        public GraphQuery(string resourceType)
        {
            _queryBuilder = new StringBuilder();
            _queryBuilder.AppendLine("Resources");
            _queryBuilder.AppendLine($"| where type == '{resourceType}'");
        }

        public static GraphQuery ForResourceType(string resourceType)
        {
            return new GraphQuery(resourceType);
        }

        public GraphQuery Project(params string[] fields)
        {
            _queryBuilder.Append("| project ");
            for (int fieldCount = 0; fieldCount < fields.Length-1; fieldCount++)
            {
                _queryBuilder.Append($"{fields[fieldCount]}, ");
            }


            _queryBuilder.AppendLine($"{fields.Last()}");

            return this;
        }

        public GraphQuery LimitTo(int limit)
        {
            _queryBuilder.AppendLine($"| limit {limit}");
            return this;
        }

        public string Build()
        {
            return _queryBuilder.ToString();
        }
    }
}