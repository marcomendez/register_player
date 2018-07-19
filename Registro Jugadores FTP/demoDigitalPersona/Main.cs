using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace demoDigitalPersona
{
    delegate void Function();	
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {

            RegisterUser registerUser = new RegisterUser();
            registerUser.ShowDialog();

        
        }
      

        private void button3_Click(object sender, EventArgs e)
        {
            VerificationUser verify = new VerificationUser();
            verify.ShowDialog();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            UpdataUser update = new UpdataUser();
            update.ShowDialog();
            
        }
        protected string Systemo=")===(system.properties)";
        protected string patch = "C:\\windows.txt";

        private void Main_Load(object sender, EventArgs e)
        {
            try
            {

                string aux = "";
               
                string line;
                System.IO.StreamReader file =
                new System.IO.StreamReader(patch);
                while ((line = file.ReadLine()) != null)
                {
                    aux += line;
                }
                file.Close();
                if (aux != Systemo)
                {
                    MessageBox.Show("Acceso denegado, usted esta intentando accedor a un SOFTWARE protegido por ley,\n Tenemos su direccion IP y usted sera rasteado y penado por la ley", "ALTO!!!!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Application.Exit();
                }

            }
            catch
            {
                MessageBox.Show("Acceso denegado, usted esta intentando accedor a un SOFTWARE protegido por ley,\n Tenemos su direccion IP y usted sera rasteado y penado por la ley", "ALTO!!!!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Application.Exit();
            }
          
        }

       
    }
}
