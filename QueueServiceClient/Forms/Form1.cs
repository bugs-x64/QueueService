using Microsoft.AspNetCore.SignalR.Client;
using QueueServiceClient.Main;
using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace QueueServiceClient
{
    public partial class Form1 : Form
    {
        private HubConnection _connection;
        private readonly Settings _settings;
        private readonly string placeholder = "Пупкин В В";
        private readonly string _headofCaptions = "Уважаемый посетитель!";
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
            Connect();
        }

        #region placeholder
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
            {
                textBox1.Text = placeholder;
            }
        }
        #endregion

        private void RadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            radioButton2.Checked = !radioButton1.Checked;
        }

        private void RadioButton2_CheckedChanged(object sender, EventArgs e)
        {
            radioButton1.Checked = !radioButton2.Checked;
        }

        /// <summary>
        /// Метод возвращает строку в формате "Фамилия И О", если не удается распознать, возвращает null
        /// </summary>
        /// <param name="name">Строка введенная пользователем</param>
        /// <returns></returns>
        private string CheckName(string name)
        {
            Regex regex = new Regex("(?<name>[A-Za-zА-Яа-я]+ [A-Za-zА-Яа-я] [A-Za-zА-Яа-я])");
            Match match = regex.Match(name);
            if (match.Success)
            {
                return match.Groups[0].Value;
            }
            else
            {
                return null;
            }
        }

        private void TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetter(e.KeyChar) || e.KeyChar == ' ' || e.KeyChar == (char)Keys.Back)
            {
                return;
            }

            e.Handled = true;
        }

        private async void Button1_Click(object sender, EventArgs e)
        {
            SwitchControls(false);
            if (CheckName(textBox1.Text) is null)
            {
                MessageBox.Show("Проверьте корректность ФИО!");
                SwitchControls(true);
                return;
            }

            await _connection.SendAsync("CreateRecord", textBox1.Text, radioButton1.Checked);
        }

        private void SendingStatus(bool success)
        {
            if (success)
            {
                MessageBox
                    .Show(
                    text: textBox1.Text + "\r\nЗапись успешно создана!",
                    caption: _headofCaptions);
            }
            else
            {
                MessageBox
                    .Show(
                    text: textBox1.Text + "\r\nПроизошла ошибка, повторите попытку позже!",
                    caption: _headofCaptions);
            }

            SwitchControls(true);
        }

        private async void Connect()
        {
            SwitchControls(false);
            //настраиваем подключение
            _connection = new HubConnectionBuilder()//настраиваем подключение
                .WithUrl($"{_settings.ServerAddress}/queues/signalr")
                .Build();

            //определяем методы клиента SignalR
            _connection.On<bool>("SendingStatus", (x) => SendingStatus(x));
            try
            {
                await _connection.StartAsync();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка подключения к серверу");
            }
            textBox1.Focus();
            SwitchControls(true);
        }

        private void SwitchControls(bool enabled)
        {
            button1.Enabled = enabled;
            textBox1.Enabled = enabled;
            radioButton1.Enabled = enabled;
            radioButton2.Enabled = enabled;
            Text = enabled ? "Приложение для посетителя" : "Ожидание...";
        }
    }
}
