using Neo4j.Driver.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neo4J_Demonstration
{
    public class ResultWrapper
    {
        public List<IRecord> list { get; set; }
        public String query { get; set; }
        public ResultWrapper(List<IRecord> list, String query)
        {
            this.list = list;
            this.query = query;
        }
    }
}
