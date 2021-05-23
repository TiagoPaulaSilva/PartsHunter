using System;
using System.Windows.Forms;

namespace PartsHunter
{
    public partial class Form2 : Form
    {
        public string Category { get; set; }

        public string Drawer { get; set; }

        public string Description { get; set; }

        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            dataGridView.Rows[0].Cells[0].Value = Category;
            dataGridView.Rows[0].Cells[1].Value = Drawer;
            dataGridView.Rows[0].Cells[2].Value = Description;

            dataGridView.Columns[0].ReadOnly = false;
            dataGridView.Columns[1].ReadOnly = false;
            dataGridView.Columns[2].ReadOnly = false;

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Category = dataGridView.Rows[0].Cells[0].Value?.ToString();
            Drawer = dataGridView.Rows[0].Cells[1].Value?.ToString();
            Description = dataGridView.Rows[0].Cells[2].Value?.ToString();

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
