using Neo4j.Driver.V1;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Neo4J_Demonstration
{
   
    public partial class Form1 : Form
    {
        Neo4JHelper helper;
        public Form1()
        {
            InitializeComponent();
        }

       
        private void chkShortestPath_CheckedChanged(object sender, EventArgs e)
        {
            if (chkShortestPath.Checked == true)
            {
                cbRelations.Enabled = false;
            }
            else
                cbRelations.Enabled = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            helper = new Neo4JHelper();
            btnQuery.Enabled = false;
            foreach(String re in helper.getAllRelationship())
            {
                cbRelations.Items.Add(re);
            }
            cbRelations.SelectedIndex = 0;
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            String relation = "All";

            try { cbRelations.SelectedItem.ToString(); } catch {}
            String src = txtSrcNode.Text;
            String des = txtDestNode.Text;

            if(chkShortestPath.Checked==true)
            {
                ResultWrapper result = helper.findSHortestPath(src, des);
                txtQueryStatement.Text = result.query;
                ShowResult(result.list, 1);
                return;
            }

            if(des=="")
            {
                ResultWrapper results = helper.getGraphWithoutDest(src, relation);
                txtQueryStatement.Text = results.query;
                ShowResult(results.list, 0);
                return;
            }
            if (des != "")
            {
                ResultWrapper results = helper.getGraphWithDest(src, relation, des);
                txtQueryStatement.Text = results.query;
                ShowResult(results.list, 0);
                return;
            }
            
            
        }


        public void ShowResult(List<IRecord> records, int type)
        {
            txtResult.Text = "";

            if (type == 1)
            {
                LinkedList<Int32> linked = new LinkedList<int>();
                foreach (IRecord r in records)
                {
                    var p = r[0];
                    String path = Newtonsoft.Json.JsonConvert.SerializeObject(p).ToString();
                    foreach (var n in fastJSON.JSON.ToDynamic(path).Relationships)
                    {
                        if (linked.Count == 0)
                        {
                            linked.AddFirst((int)n.StartNodeId);
                            //linked.AddAfter(n.StartNodeId, n.Id);
                            linked.AddLast((int)n.EndNodeId);
                            continue;
                        }

                        int s = (int)n.StartNodeId;
                        int e = (int)n.EndNodeId;

                        if (linked.Contains(s))
                        {
                            if (linked.First.Value == s)
                            {
                                linked.AddFirst((int)e);
                            }
                            else
                            {
                                if (linked.Last.Value == s)
                                    linked.AddLast((int)e);
                                        }
                        }
                        else
                        {
                            if (linked.Contains(e))
                            {
                                if (linked.First.Value == e)
                                {
                                    linked.AddFirst((int)s);
                                }
                                else
                                {
                                    if (linked.Last.Value == e)
                                    {
                                        linked.AddLast((int)s);
                                    }
                                }
                            }
                        }

                        //Console.Write(n.StartNodeId);
                        //Console.WriteLine(n.EndNodeId);

                    }

                    int previous = -1;
                    foreach(int a in linked)
                    {
                        if (previous == -1)
                        {
                            previous = a;
                            continue;
                        }
                        else
                        {
                            txtResult.Text += helper.getNodeName(previous);
                            txtResult.Text += "--" + helper.getRelName(previous, a) + "--";
                            
                            previous = a;
                        }
                    }
                    txtResult.Text += helper.getNodeName(previous);

                }

                return;
            }

            foreach (IRecord r in records)
            {
               
                var srcN = r[0];
                var relR = r[1];
                var destN = r[2];
                String s = Newtonsoft.Json.JsonConvert.SerializeObject(srcN).ToString();

                var srcJ = fastJSON.JSON.ToDynamic(s);
                var relJ = fastJSON.JSON.ToDynamic(Newtonsoft.Json.JsonConvert.SerializeObject(relR));
                var destJ = fastJSON.JSON.ToDynamic(Newtonsoft.Json.JsonConvert.SerializeObject(destN));

                txtResult.Text += ("\r\n"+srcJ.Properties.objectEntity +" > "+ relJ.Type +" > " + destJ.Properties.objectEntity+ "\r\n");

           
            }
        }

        private void txtSrcNode_TextChanged(object sender, EventArgs e)
        {
            if(txtSrcNode.Text=="")
            {
                btnQuery.Enabled = false;

            }
            else
                btnQuery.Enabled = true;
        }

        private void btnExeQ_Click(object sender, EventArgs e)
        {

        }
    }
}
