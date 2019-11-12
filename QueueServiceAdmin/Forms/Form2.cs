using System;
using System.Windows.Forms;

namespace QueueServiceAdmin
{
    public partial class Form2 : Form
    {
        private readonly Form1 _form;
        public Form2(Form1 form)
        {
            InitializeComponent();
            _form = form;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            _form.EmplName = textBox1.Text;
            _form.Text += " " + textBox1.Text;
            Close();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}
