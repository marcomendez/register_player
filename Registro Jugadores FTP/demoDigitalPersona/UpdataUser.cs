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
    public partial class UpdataUser : RegisterUser
    {
        public UpdataUser()
        {
            InitializeComponent();
        }

        protected override void registerUser()
        {
            MemoryStream fingerdata = new MemoryStream();
            base.Enroller.Template.Serialize(fingerdata);
            fingerdata.Position = 0;
            BinaryReader br = new BinaryReader(fingerdata);
            Byte[] bytes = br.ReadBytes((Int32)fingerdata.Length);
            ClassConnectDataBase c = new ClassConnectDataBase();

            string result = c.update(int.Parse(textBox3.Text), txt_name.Text, txt_lastname.Text, teamName, txt_carnet.Text + " " + ciudadaCarnet, txt_phono.Text, "", "",
                "", positionName, txt_date.Text, nameCode, arrayTemplate[0], arrayTemplate[1]);
            SetStatus("Datos Actualizados Correctamente");
            Message(result);
        }
        private void UpdataUser_Load(object sender, EventArgs e)
        {

        }

        private void UpdataUser_FormClosed(object sender, FormClosedEventArgs e)
        {
            Stop();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {

                Search_Users search = new Search_Users();
                search.ShowDialog();
                textBox3.Text = search.id;
                txt_name.Text = search.name;
                txt_lastname.Text = search.lastname;
                combo_team.Text = search.nickname;

                txt_carnet.Text = search.height;

                txt_position.Text = search.position;
                txt_date.Text = search.birthday;
            }
            catch
            {

            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ClassConnectDataBase delete = new ClassConnectDataBase();
            MessageBox.Show(delete.delete_user(textBox3.Text), "Eleminar Jugador");


        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {

                Search_Users search = new Search_Users();
                search.ShowDialog();
                textBox3.Text = search.id;
                txt_name.Text = search.name;
                txt_lastname.Text = search.lastname;
                combo_team.Text = search.nickname;

                combo_division.Text = search.height;

                txt_position.Text = search.position;
                txt_date.Text = search.birthday;
            }
            catch
            {

            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Esta Seguro que Quiere Eliminar a este Jugador ", "AFC - ELIMINAR JUGADOR", MessageBoxButtons.YesNoCancel);
            if (result == DialogResult.Yes)
            {
                ClassConnectDataBase delete = new ClassConnectDataBase();
                MessageBox.Show(delete.delete_user(textBox3.Text), "AFC - JUGADOR ELIMINADO", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
