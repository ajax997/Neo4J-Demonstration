using Neo4j.Driver.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neo4J_Demonstration
{
    public class Neo4JHelper
    {
        private IDriver _driver;
        string uri = "bolt://localhost:7687"; string user = "neo4j";
        string password = "lumia1020";
        public Neo4JHelper()
        {
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
        }
        public List<String> getAllRelationship()
        {
            List<String> rel = new List<string>();
            using (var session = _driver.Session())
            {
                using (var tx = session.BeginTransaction())
                {
                    IStatementResult results = tx.Run(
                    @"match(n)-[r]-(v) return distinct type(r)");
                    foreach (IRecord result in results)
                    {
                        rel.Add(result[0].As<String>());
                    }
                }
            }
            return rel;
        }

        public ResultWrapper getGraphWithoutDest(String src, String rl)
        {
            List<IRecord> list = new List<IRecord>();
            String query = "";
            using (var session = _driver.Session())
            {
                using (var tx = session.BeginTransaction())
                {
                    if (rl == "All")
                    {
                        query = (@"match(n {objectEntity: '" + src + "'" + "})-[r]-(v) return n,r,v");

                    }
                    else
                    {
                        query = (@"match(n {objectEntity: '" + src + "'" + "})-[r:" + rl + "]-(v) return n,r,v");

                    }
                    IStatementResult results = tx.Run(query);
                    foreach (IRecord result in results)
                    {
                        list.Add(result);
                    }
                    return new ResultWrapper(list, query);

                }
            }
        }
    
        internal String getNodeName(int num)
        {
            String query = @"match(s) where id(s)="+num.ToString() +" return s.objectEntity";

            
            using (var session = _driver.Session())
            {
                using (var tx = session.BeginTransaction())
                {

                    IStatementResult results = tx.Run(query);
                    foreach (IRecord result in results)
                    {
                        return result[0].As<String>();
                    }
                    
                }
            }
            return "";
     
        }
        internal String getRelName(int src, int dest)
        {
            String query = @"match(s)-[r]-(v) where id(s)="+src+" and id(v)="+dest+" return type(r)";


            using (var session = _driver.Session())
            {
                using (var tx = session.BeginTransaction())
                {

                    IStatementResult results = tx.Run(query);
                    foreach (IRecord result in results)
                    {
                        return result[0].As<String>();
                    }

                }
            }
            return "";
        }

        internal ResultWrapper findSHortestPath(string src, string des)
        {
            String query = "MATCH (n:ObjectEntity {objectEntity:'"+src+"'}),(v:ObjectEntity { objectEntity: '"+des+"' }), p = shortestPath((n)-[*]-(v)) RETURN p";

            List<IRecord> list = new List<IRecord>();
            using (var session = _driver.Session())
            {
                using (var tx = session.BeginTransaction())
                {
                    
                    IStatementResult results = tx.Run(query);
                    foreach (IRecord result in results)
                    {
                        list.Add(result);
                    }
                    return new ResultWrapper(list, query);
                }
            }

        }

        internal ResultWrapper getGraphWithDest(string src, string rl, string des)
        {
            List<IRecord> list = new List<IRecord>();
            String query = "";
            using (var session = _driver.Session())
            {
                using (var tx = session.BeginTransaction())
                {
                    if (rl == "All")
                    {
                        query = @"match(n {objectEntity: '" + src + "'" + "})-[r]-(v {objectEntity: '" + des + "'" + "}) return n,r,v";
                    }
                    else
                    {
                        query = @"match(n {objectEntity: '" + src + "'" + "})-[r:" + rl + "]-(v {objectEntity: '" + des + "'" + "}) return n,r,v";
                    }
                    IStatementResult results = tx.Run(query);
                    foreach (IRecord result in results)
                    {
                        list.Add(result);
                    }
                    return new ResultWrapper(list, query);


                }
            }
        }
    }
}
