using Microsoft.AspNetCore.SignalR.Client;
using QueueServiceAdmin.Main;
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows.Forms;

namespace QueueServiceAdmin
{
    public partial class Form1 : Form
    {
        private HubConnection _connection;
        private readonly Settings _settings;
        public string EmplName { get => _emplName; set => _emplName = value ?? "Петя"; }
        private string _emplName;
        private readonly string _headofCaptions = "Сообщение сотруднику";

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
            Form2 form = new Form2(this);
            form.ShowDialog();
            form.Dispose();
            Visible = true;
            Connect();
        }

        //забрать из неконкурирующей очереди
        private async void Button1_Click(object sender, EventArgs e)
        {
            SwitchControls(false);
            await _connection.SendAsync("GetNext", 1);
        }

        //забрать из конкурирующей очереди
        private async void Button2_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex == -1) return;
            SwitchControls(false);
            var recid = ((QueueRecord)listBox2.Items[listBox2.SelectedIndex]).RecId;
            await _connection.SendAsync("GetSelected", recid, 1);
        }

        //вызов метода происходит, если сотрудник выбирает посетителя из конкурирующего списка
        private void ListBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            button2.Enabled = listBox2.SelectedIndex != -1;
        }

        /// <summary>
        /// Метод сортирует данные, полученные с сервера, по типу очереди
        /// </summary>
        /// <param name="recordsAll">список с данными об ожидающих посетителях</param>
        private void Sort(List<QueueRecord> recordsAll)
        {
            Invoke((MethodInvoker)delegate
            {
                listBox1.Items.Clear();
                listBox2.Items.Clear();

                foreach (QueueRecord record in recordsAll)
                {
                    if (!record.Competing)
                    {
                        listBox1.Items.Add(record);
                    }
                    else
                    {
                        listBox2.Items.Add(record);
                    }
                }

                button1.Enabled = listBox1.Items.Count > 0;
            });
        }

        /// <summary>
        /// метод выполняет подключение к веб серверу
        /// </summary>
        private async void Connect()
        {
            SwitchControls(false);
            //настраиваем подключение
            _connection = new HubConnectionBuilder()//настраиваем подключение
                .WithUrl($"{_settings.ServerAddress}/queues/signalr", options =>
                {
                    options.Credentials = CredentialCache.DefaultCredentials;
                    options.Proxy = new WebProxy()
                    {
                        Credentials = CredentialCache.DefaultCredentials
                    };
                })
                .WithAutomaticReconnect()
                .Build();
            

            //определяем методы клиента SignalR
            _connection.On<bool, QueueRecord>("RequestResult", (x, y) => RequestResult(x, y));
            _connection.On<List<QueueRecord>>("QueuesUpdate", (x) => Sort(x));

            try
            {
                await _connection.StartAsync();
                await _connection.SendAsync("GetQueues", false)
                    .ContinueWith(delegate { SwitchControls(true); });
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка подключения к серверу, перезапустите приложение");
            }
        }

        /// <summary>
        /// Метод уведомляет пользователя об успехе выполнения методов сервера
        /// </summary>
        /// <param name="success"></param>
        /// <param name="record"></param>
        private void RequestResult(bool success, QueueRecord record)
        {
            if (success)
            {
                MessageBox
                    .Show(
                    text: $"{EmplName}\r\nВы успешно забрали клиента!\r\n{record.Fio}",
                    caption: _headofCaptions);
            }
            else
            {
                MessageBox
                    .Show(
                    text: EmplName + "\r\nПроизошла ошибка, повторите попытку позже!",
                    caption: _headofCaptions);
            }

            SwitchControls(true);
        }

        /// <summary>
        /// Метод переключает активность элементов управления пользовательского интерфейса
        /// </summary>
        /// <param name="enabled">true - элементы UI активны, false - элементы UI неактивны. </param>
        private void SwitchControls(bool enabled)
        {
            Invoke((MethodInvoker)delegate
            {
                button1.Enabled = enabled ? listBox1.Items.Count > 0 : false;
                button2.Enabled = enabled ? listBox2.SelectedIndex != -1 : false;
                listBox1.Enabled = enabled;
                listBox2.Enabled = enabled;
                Text = enabled ? $"Приложение для сотрудника {EmplName}" : "Ожидание...";
            });
        }
    }
}
