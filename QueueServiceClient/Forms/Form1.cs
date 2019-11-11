using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using QueueServiceClient.Main;

namespace QueueServiceClient
{
    public partial class Form1 : Form
    {
        private readonly Settings _settings;
        private readonly string placeholder = "Пупкин В В";
        public Form1()
        {
            InitializeComponent();
            _settings = new Settings();
            _settings.Load();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = placeholder;
            textBox1.GotFocus += RemoveText;
            textBox1.LostFocus += AddText;
        }


        public void RemoveText(object sender, EventArgs e)
        {
            if (textBox1.Text == placeholder)
            {
                textBox1.Text = "";
            }
        }

        public void AddText(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
                textBox1.Text = placeholder;
        }

        private void RadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (!radioButton1.Checked) return;
            else radioButton2.Checked = false;
        }

        private void RadioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (!radioButton2.Checked) return;
            else radioButton1.Checked = false;
        }

        //Метод проверки фио
        private string CheckName(string name)
        {
            var regex = new Regex("(?<name>[A-Za-zА-Яа-я]+ [A-Za-zА-Яа-я] [A-Za-zА-Яа-я])");
            var match = regex.Match(name);
            if (match.Success) return match.Groups[0].Value;
            else return "null";
        }

        private void TextBox1_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetter(e.KeyChar) || e.KeyChar == ' '|| e.KeyChar == (char)Keys.Back) return;
            e.Handled = true;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            if (CheckName(textBox1.Text) == "null")
            {
                MessageBox.Show("Проверьте корректность ФИО!");
                button1.Enabled = true;
                return;
            }

            SendDataAsync(textBox1.Text, radioButton1.Checked);
        }
        private async void SendDataAsync(string fio, bool iscompeting)
        {
            var client = new MyWebClient();
            var response = await client.Post(
                remoteAddress: new Uri($"{_settings.ServerAddress}/api/queues/?c={iscompeting}"),
                httpContent: MyWebClient.BuildJSONContent($"\"{fio}\""));
            if (response.IsSuccessStatusCode) MessageBox.Show(textBox1.Text + "\r\nЗапись успешно создана!", "Клиенту");
            else MessageBox.Show(textBox1.Text + "\r\nПроизошла ошибка, повторите попытку позже!", "Клиенту");
            this.Enabled = true;
            client.Dispose();
        }
    }
}
