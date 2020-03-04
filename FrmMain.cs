using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using RestSharp;

namespace Moded_Xss
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void Btn_Execute_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(txt_link.Text))
            {
                xssFound = 0;
                btn_Execute.Enabled = false;

                if (File.Exists("Xss.txt"))
                {
                    var xssPayloads = File.ReadAllLines("Xss.txt").ToList();
                    foreach (var i in xssPayloads)
                    {
                        Thread t = new Thread(() => { 
                        
                        Task.Factory.StartNew(() => { xssVulnerableTest(txt_link.Text, i); }).Wait();
                        });
                        t.IsBackground = true;
                        t.Start();
                    }
                }
                else
                {
                    MessageBox.Show("file Xss.txt is not found !", "Warning !", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
        int xssFound = 0;

        private void xssVulnerableTest(String Url, String Payload)
        {
            try
            {
                Task.Delay(1000);

                var u = Url + Payload;
                var client = new RestClient(u);
                var req = new RestRequest(Method.GET);

                lbl_status.Invoke((MethodInvoker)delegate { lbl_status.ForeColor = Color.Coral; lbl_status.Text = "Status: Testing " + u; });

                var resp = client.Get(req);

                if (resp.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    xssFound++;
                    listView1.Invoke((MethodInvoker)delegate { listView1.Items.Add(new ListViewItem(new String[] { u, Payload })); });
                }
            }
            catch { }
        }
    }
}
