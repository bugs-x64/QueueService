using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using QueueServiceAdmin.Main;

namespace QueueServiceAdmin
{
    public partial class Form1 : Form
    {
        private readonly Settings _settings;
        private bool _waiting;
        public string EmplName { get { return _emplName; } set { _emplName = value ?? "Петя"; } }
        private string _emplName;

        public Form1()
        {
            InitializeComponent();

            //config form
            listBox1.DisplayMember = "Fio";
            listBox1.ValueMember = "QueueId";
            listBox2.DisplayMember = "Fio";
            listBox2.ValueMember = "QueueId";

            //load settings
            _settings = new Settings();
            _settings.LoadXML();

        }


        private void Form1_Load(object sender, EventArgs e)
        {
            Visible = false;
            var form = new Form2(this);
            form.ShowDialog();
            form.Dispose();
            Visible = true;
        }

        //забрать из неконкурирующей очереди
        private void Button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = false;
            _waiting = true;
            QueueWebMethods.GetNextAsync(_settings.ServerAddress,EmplName);
        }

        //забрать из конкурирующей очереди
        private void Button2_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = false;
            QueueWebMethods.GetConcurAsync(_settings.ServerAddress, EmplName, (QueueRecord)listBox2.Items[listBox2.SelectedIndex]);
        }

        //метод срабатывает, если сотрудник выбирает клиента из конкурирующего списка
        private void ListBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex == -1 || _waiting)
            {
                button2.Enabled = false;
                return;
            }
            button2.Enabled = true;
        }

        void Sort(List<QueueRecord> recordsAll)
        {
            Invoke(new MethodInvoker(delegate
            {
                listBox1.Items.Clear();
                listBox2.Items.Clear();
            }));
            foreach (var record in recordsAll)
                if (!record.Competing)
                    listBox1.Invoke(new MethodInvoker(delegate { listBox1.Items.Add(record); }));
                else
                    listBox2.Invoke(new MethodInvoker(delegate { listBox2.Items.Add(record); }));

            if (_waiting) return;

            Invoke(new MethodInvoker(delegate
            {
                button1.Enabled = listBox1.Items.Count > 0;
            }));
        }


        //private async void LoopAsync()
        //{
        //    using (var client = new MyWebClient())
        //        while (true)
        //        {
        //            var response1 = await client.GetAsync(new Uri(_settings.ServerAddress + "/api/queues"));
        //            var responseContent1 = await response1.Content.ReadAsStringAsync();
        //            var recordsAll = JsonConvert.DeserializeObject<List<QueueRecord>>(responseContent1);
        //            await Task.Run(() => Sort(recordsAll));
        //            Thread.Sleep(_settings.SleepTime * 1000);
        //        }
        //}
    }
}
