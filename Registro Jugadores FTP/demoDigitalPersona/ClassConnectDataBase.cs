using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace demoDigitalPersona
{
    class ClassConnectDataBase
    {

        public MySqlConnection myConnection;

        private FileStream fs;
        private BinaryReader br;
        private byte[] imageData;
        MySqlTransaction myTrans;
        public ClassConnectDataBase()
        {
            string aux = "server=sidbol.com;database=sidbol_db;uid=sidbol;pwd=1G3!Nx}$!W]f;";

            myConnection = new MySqlConnection(aux);
        }
        /// <summary>
        /// Begin the transaction services
        /// </summary>
        private void beginTransaction()
        {
            myConnection.Open();

            MySqlCommand myCommand = new MySqlCommand();
            myCommand.Connection = myConnection;


            // Start a local transaction
            myTrans = myConnection.BeginTransaction(IsolationLevel.ReadCommitted);
            // Assign transaction object for a pending local transaction
            myCommand.Transaction = myTrans;
        }

        /// <summary>
        /// yhis method do a select all by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataTable getDatawithTwoFinger(string id)
        {

            MySqlCommand command;
            MySqlDataAdapter da;
            string query = "Select * from players WHERE  id=" + id;
            command = new MySqlCommand(query, myConnection);
            da = new MySqlDataAdapter(command);
            DataTable table = new DataTable();
            da.Fill(table);

            da.Dispose();
            return table;
        }

        /// <summary>
        /// this method do a select with parameters
        /// id, fingerOne, fingerTwo
        /// </summary>
        /// <returns></returns>
        public DataTable getDataWithTwoFingers(string name)
        {

            MySqlCommand command;
            MySqlDataAdapter da;

            string query = "Select id,Name,fingerOne,fingerTwo from players  where Name LIKE'" + name + "%'";
            command = new MySqlCommand(query, myConnection);
            da = new MySqlDataAdapter(command);
            DataTable table = new DataTable();
            da.Fill(table);

            da.Dispose();
            return table;
        }
        /// <summary>
        /// This method Update a User registered
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="finger"></param>
        /// <param name="photo"></param>
        internal string update(int id, string name, string lastname, string nickname, string height, string weight, string skillful, string position, string birthday, string patchphoto, byte[] fingerOne, byte[] fingerTwo)
        {
            try
            {
                beginTransaction();
               
                MySqlCommand cmd;
                string cmdString = "UPDATE " + "players" +
                    " SET name=@name,last_name=@lastname,nickname=@nickname,height=@height,weight=@weight,skillful_leg=@skillful " +
                    ",position=@position,birthday=@birthday,image=@image,fingerOne=@fingerOne,fingerTwo=@fingerTwo " +
                    " WHERE id= @id";

                cmd = new MySqlCommand(cmdString, myConnection);
                cmd.Parameters.Add("@id", MySqlDbType.Int16);
                cmd.Parameters.Add("@name", MySqlDbType.VarChar, 45);
                cmd.Parameters.Add("@lastname", MySqlDbType.VarChar, 45);
                cmd.Parameters.Add("@nickname", MySqlDbType.VarChar, 40);
                cmd.Parameters.Add("@height", MySqlDbType.VarChar, 10);
                cmd.Parameters.Add("@weight", MySqlDbType.VarChar, 10);
                cmd.Parameters.Add("@skillful", MySqlDbType.VarChar, 40);
                cmd.Parameters.Add("@position", MySqlDbType.VarChar, 40);
                cmd.Parameters.Add("@birthday", MySqlDbType.Date);
                cmd.Parameters.Add("@image", MySqlDbType.Text);
                cmd.Parameters.Add("@fingerOne", MySqlDbType.LongBlob);
                cmd.Parameters.Add("@fingerTwo", MySqlDbType.LongBlob);
                cmd.Parameters["@id"].Value = id;
                cmd.Parameters["@name"].Value = name;
                cmd.Parameters["@lastname"].Value = lastname;
                cmd.Parameters["@nickname"].Value = nickname;
                cmd.Parameters["@height"].Value = height;
                cmd.Parameters["@weight"].Value = weight;
                cmd.Parameters["@skillful"].Value = skillful;
                cmd.Parameters["@position"].Value = position;
                cmd.Parameters["@birthday"].Value = DateTime.Parse(birthday);
                cmd.Parameters["@image"].Value = patchphoto;
                cmd.Parameters["@fingerOne"].Value = fingerOne;
                cmd.Parameters["@fingerTwo"].Value = fingerTwo;
                cmd.ExecuteNonQuery();
                ftp ftpClient = new ftp(@"ftp://sidbol.com", "sidbol", "1G3!Nx}$!W]f");

                /* Upload a File */
                ftpClient.upload("public_html/afc/app/webroot/img/players/sm_" + patchphoto + ".jpg", Application.StartupPath + "\\Images\\sm_" + patchphoto + ".jpg");
                myTrans.Commit();
            }
            catch (Exception e)
            {
                myTrans.Rollback();
                return "***************** There is problem, please try again ***********************";
            }
            finally
            {
                myConnection.Close();
            }
            return "***************** DATOS DEL JUGADOR ACTUALIZADO CORRECTAMENTE ***********************";
        }
        /// <summary>
        /// this method do an insert to register user
        /// </summary>
        /// <param name="name"></param>
        /// <param name="lastname"></param>
        /// <param name="nickname"></param>
        /// <param name="height"></param>
        /// <param name="weight"></param>
        /// <param name="skillful"></param>
        /// <param name="position"></param>
        /// <param name="birthday"></param>
        /// <param name="patchphoto"></param>
        /// <param name="fingerOne"></param>
        /// <param name="fingerTwo"></param>
        /// <returns></returns>
        public string insert(string name, string lastname, string nickname, string height, string weight, string skillful, string position, string birthday, string patchphoto, byte[] fingerOne, byte[] fingerTwo)
        {
            try
            {
                beginTransaction();
                
                MySqlCommand cmd;
                string cmdString = "INSERT INTO players(name,last_name,nickname,height,weight,skillful_leg,position,birthday,image,fingerOne,fingerTwo)values(@name,@lastname,@nickname,@height,@weight,@skillful,@position,@birthday,@image,@fingerOne,@fingerTwo)";
                cmd = new MySqlCommand(cmdString, myConnection);
                cmd.Parameters.Add("@name", MySqlDbType.VarChar, 45);
                cmd.Parameters.Add("@lastname", MySqlDbType.VarChar, 45);
                cmd.Parameters.Add("@nickname", MySqlDbType.VarChar, 40);
                cmd.Parameters.Add("@height", MySqlDbType.VarChar, 10);
                cmd.Parameters.Add("@weight", MySqlDbType.VarChar, 10);
                cmd.Parameters.Add("@skillful", MySqlDbType.VarChar, 40);
                cmd.Parameters.Add("@position", MySqlDbType.VarChar, 40);
                cmd.Parameters.Add("@birthday", MySqlDbType.Date);
                cmd.Parameters.Add("@image", MySqlDbType.Text);
                cmd.Parameters.Add("@fingerOne", MySqlDbType.Blob);
                cmd.Parameters.Add("@fingerTwo", MySqlDbType.Blob);
                cmd.Parameters["@name"].Value = name;
                cmd.Parameters["@lastname"].Value = lastname;
                cmd.Parameters["@nickname"].Value = nickname;
                cmd.Parameters["@height"].Value = height;
                cmd.Parameters["@weight"].Value = weight;
                cmd.Parameters["@skillful"].Value = skillful;
                cmd.Parameters["@position"].Value = position;
                cmd.Parameters["@birthday"].Value = DateTime.Parse(birthday);
                cmd.Parameters["@image"].Value = patchphoto;
                cmd.Parameters["@fingerOne"].Value = fingerOne;
                cmd.Parameters["@fingerTwo"].Value = fingerTwo;
                cmd.ExecuteNonQuery();
                /* Create Object Instance */
                ftp ftpClient = new ftp(@"ftp://sidbol.com", "sidbol", "1G3!Nx}$!W]f");

                /* Upload a File */
                ftpClient.upload("public_html/afc/app/webroot/img/players/sm_" + patchphoto +".jpg", Application.StartupPath + "\\Images\\sm_"+ patchphoto+".jpg");
                myTrans.Commit();

            }
            catch (Exception e)
            {
                myTrans.Rollback();
                return e.Message + "*************** There is a error, please try again ********************";

            }
            finally
            {
                myConnection.Close();

            }
            return "*************** JUGAR REGISTRADO CORRECTAMENTE ********************";
        }
 



      
        public DataTable search_Users()
        {
            MySqlCommand command;
            MySqlDataAdapter da;
            string query = "select id,name,last_name,nickname,height,weight,skillful_leg,position,birthday from players ";

            command = new MySqlCommand(query, myConnection);
            da = new MySqlDataAdapter(command);
            DataTable table = new DataTable();
            da.Fill(table);

            da.Dispose();
            myConnection.Close();
            return table;
        }

        public DataTable search_Users(string name)
        {
            MySqlCommand command;
            MySqlDataAdapter da;
            //     string query =String.Format( "select id,name,last_name,nickname,height,weight,skillful_leg,position,birthday from players  where id between  {0} and {1}",17,20);
            string query = "select id,name,last_name,nickname,height,weight,skillful_leg,position,birthday from players  where Name LIKE'" + name + "%'";

            command = new MySqlCommand(query, myConnection);
            da = new MySqlDataAdapter(command);
            DataTable table = new DataTable();
            da.Fill(table);

            da.Dispose();
            myConnection.Close();
            return table;
        }

        public DataTable search_UsersWITHRANDON(int start, int end)
        {
            MySqlCommand command;
            MySqlDataAdapter da;
            string query = String.Format("Select id,fingerOne,fingerTwo from players  where id between  {0} and {1}", start, end);
            command = new MySqlCommand(query, myConnection);
            da = new MySqlDataAdapter(command);
            DataTable table = new DataTable();
            da.Fill(table);

            da.Dispose();
            myConnection.Close();
            return table;
        }


        public string delete_user(string user_id)
        {
            try
            {
                myConnection.Open();
                string cmdString = "delete  from players where id= " + user_id;
                MySqlCommand cmd = new MySqlCommand(cmdString, myConnection);
                cmd.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                return e.Message;
            }
            finally
            {
                myConnection.Close();

            }
            return "*********************** User Deleted ***************************";


        }
    }
}
