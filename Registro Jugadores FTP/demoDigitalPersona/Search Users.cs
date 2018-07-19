using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace demoDigitalPersona
{
    public partial class Search_Users : Form
    {
        public Search_Users()
        {
            InitializeComponent();
        }
          ClassConnectDataBase search ;

    
        public string id ="0";
        public string  name = "";
        public string lastname="";
        public string nickname="";

        public string height = "", weight = "", skillful_leg = "", position = "", birthday = "";

        private void Search_Users_Load(object sender, EventArgs e)
        {
         search= new ClassConnectDataBase();
           
        }

        

        private void dataGridView1_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                id = dataGridView1[0, e.RowIndex].Value.ToString();
                name = dataGridView1[1, e.RowIndex].Value.ToString();
                lastname = dataGridView1[2, e.RowIndex].Value.ToString();
                nickname = dataGridView1[3, e.RowIndex].Value.ToString();

                height = dataGridView1[4, e.RowIndex].Value.ToString();
                weight = dataGridView1[5, e.RowIndex].Value.ToString();
                skillful_leg = dataGridView1[6, e.RowIndex].Value.ToString();
                position = dataGridView1[7, e.RowIndex].Value.ToString();
                birthday = dataGridView1[8, e.RowIndex].Value.ToString();

                this.Close();
              

            }
            catch {
               
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            search = new ClassConnectDataBase();
            dataGridView1.DataSource = search.search_Users(textBox1.Text);
        }
    }
}
