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

namespace QueueServiceAdmin
{
    public partial class Form1 : Form
    {
        readonly Settings _settings;
        bool waitresponse = false;
        public string EmplName { get { return _emplName; } set { _emplName = value ?? "Петя"; } }
        private string _emplName = "Петя";

        public Form1()
        {
            InitializeComponent();
            _settings = new Settings();
            _settings.Load();
            listBox1.DisplayMember = "Fio";
            listBox1.ValueMember = "QueueId";
            listBox2.DisplayMember = "Fio";
            listBox2.ValueMember = "QueueId";
            var forma = new Form2(this);
            forma.ShowDialog();
            LoopAsync();
            this.Visible = false;
        }


        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = false;
            GetNextAsync();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = false;
            GetConcurAsync((QueueRecord)listBox2.Items[listBox2.SelectedIndex]);
        }

        private void Label4_Click(object sender, EventArgs e)
        {

        }

        private void Label3_Click(object sender, EventArgs e)
        {

        }

        private void ListBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex == -1 || waitresponse)
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
                if (!record.Competing) listBox1.Invoke(new MethodInvoker(delegate { listBox1.Items.Add(record); }));
                else listBox2.Invoke(new MethodInvoker(delegate { listBox2.Items.Add(record); }));

            if (waitresponse) return;

            if (listBox1.Items.Count > 0)
            {
                Invoke(new MethodInvoker(delegate
                {
                    button1.Enabled = true;
                }));

            }
            else
            {
                Invoke(new MethodInvoker(delegate
                {
                    button1.Enabled = false;
                }));
            }
        }


        private async void GetNextAsync()
        {
            Invoke(new MethodInvoker(delegate
            {
                waitresponse = true;
            }));
            using (var client = new MyWebClient())
            {
                var response = await client.Post(
                                remoteAddress: new Uri($"{_settings.ServerAddress}/api/queues/next"),
                                httpContent: MyWebClient.BuildJSONContent($"1"));
                if (!response.IsSuccessStatusCode) { MessageBox.Show("\r\nПроизошла ошибка, повторите попытку позже!", "Сотруднику " + EmplName); }
                else
                {
                    var queueRecord = JsonConvert.DeserializeObject<QueueRecord>(await response.Content.ReadAsStringAsync());
                    MessageBox.Show($"Вы успешно забрали клиента \"{queueRecord.Fio}\" из {(queueRecord.Competing ? label1.Text : label2.Text)}", "Сотруднику " + EmplName);
                }
            }
            Invoke(new MethodInvoker(delegate
            {
                waitresponse = false;
            }));
        }
        private async void GetConcurAsync(QueueRecord queueRecord)
        {
            Invoke(new MethodInvoker(delegate
            {
                waitresponse = true;
            }));
            using (var client = new MyWebClient())
            {
                var response = await client.Post(
                                remoteAddress: new Uri($"{_settings.ServerAddress}/api/queues/{queueRecord.RecId}"),
                                httpContent: MyWebClient.BuildJSONContent($"1"));
                if (!response.IsSuccessStatusCode) { MessageBox.Show("\r\nПроизошла ошибка, повторите попытку позже!", "Сотруднику " + EmplName); }
                else MessageBox.Show($"Вы успешно забрали клиента \"{queueRecord.Fio}\" из {(queueRecord.Competing ? label1.Text : label2.Text)}", "Сотруднику " + EmplName);
            }

            Invoke(new MethodInvoker(delegate
            {
                waitresponse = false;
            }));
        }

        private async void LoopAsync()
        {
            using (var client = new MyWebClient())
                while (true)
                {
                    var response1 = await client.GetAsync(new Uri(_settings.ServerAddress + "/api/queues"));
                    var responseContent1 = await response1.Content.ReadAsStringAsync();
                    var recordsAll = JsonConvert.DeserializeObject<List<QueueRecord>>(responseContent1);
                    await Task.Run(() => Sort(recordsAll));
                    Thread.Sleep(_settings.SleepTime * 1000);
                }
        }
    }
}
