from neo4j import GraphDatabase
import os
import json
import traceback

uri = "bolt://localhost:7687"
user = "neo4j"
password = "lumia1020"
_driver = GraphDatabase.driver(uri, auth=(user, password))

path = 'C:\\Users\\khang\\neo4j_doc\\'

def build_rel_query(src, rel, dest):
    query = "match (n{objectEntity:'"+src+"'}), (v{objectEntity:'"+dest+"'}) create (n)-[:"+rel+"]->(v)"
    return query

def run_query(query):
    with _driver.session() as session:
        session.write_transaction(run, query)


def run(tx, query):
    tx.run(query)


def dashProcess(_j, f_name):
    try:
        #query = "create(n{objectEntity:'" + f_name + "'})"
        #run_query(query)
        #print("Created ", f_name)
        de = _j['definitions']
        for x in de:
            for k in x.keys():
                if type(x[k]) == list:
                    for sub in x[k]:
                        run_query(build_rel_query(f_name, k, sub))

    except:
        traceback.print_exc()


def normalProcess(_j, f_name):
    try:
        #query = "create(n{objectEntity:'" + f_name + "'})"
        #run_query(query)
        #print("Created ", f_name)
        de = _j['results']
        for x in de:
            for k in x.keys():
                if type(x[k]) == list:
                    for sub in x[k]:
                        run_query(build_rel_query(f_name, k, sub))
    except:
        traceback.print_exc()


files = []
# r=root, d=directories, f = files
for r, d, f in os.walk(path):
    for file in f:
        if '.json' in file:
            try:
                f = open(os.path.join(r, file))
                _json = f.read()
                if str(file)[0] == '_':
                    dashProcess(json.loads(_json), str(file)[1:-5])
                else:
                    normalProcess(json.loads(_json), str(file[0:-5]))

                f.close()
            except:
                continue
