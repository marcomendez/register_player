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
            string aux ="server="+ ConstactsCreadentials.serverName +
                        ";database=" + ConstactsCreadentials.database + 
                        ";uid=" + ConstactsCreadentials.userName + 
                        ";pwd=" + ConstactsCreadentials.pass +
                        ";";
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
        public string update(int id, string name, string lastname, string nickname,string ci, string phono, string height, string weight, string skillful, string position, string birthday, string patchphoto, byte[] fingerOne, byte[] fingerTwo)
        {
            try
            {
                beginTransaction();
               
                MySqlCommand cmd;
                string cmdString = "UPDATE " + "players" +
                    " SET name=@name,last_name=@lastname,nickname=@nickname,ci=@ci,phono=@phono,height=@height,weight=@weight,skillful_leg=@skillful " +
                    ",position=@position,birthday=@birthday,image=@image,fingerOne=@fingerOne,fingerTwo=@fingerTwo " +
                    " WHERE id= @id";

                cmd = new MySqlCommand(cmdString, myConnection);
                cmd.Parameters.Add("@id", MySqlDbType.Int16);
                cmd.Parameters.Add("@name", MySqlDbType.VarChar, 45);
                cmd.Parameters.Add("@lastname", MySqlDbType.VarChar, 45);
                cmd.Parameters.Add("@nickname", MySqlDbType.VarChar, 40);
                cmd.Parameters.Add("@ci", MySqlDbType.VarChar, 20);
                cmd.Parameters.Add("@phono", MySqlDbType.VarChar, 30);
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
                cmd.Parameters["@ci"].Value = ci;
                cmd.Parameters["@phono"].Value =phono;
                cmd.Parameters["@height"].Value = height;
                cmd.Parameters["@weight"].Value = weight;
                cmd.Parameters["@skillful"].Value = skillful;
                cmd.Parameters["@position"].Value = position;
                cmd.Parameters["@birthday"].Value = DateTime.Parse(birthday);
                cmd.Parameters["@image"].Value = patchphoto;
                cmd.Parameters["@fingerOne"].Value = fingerOne;
                cmd.Parameters["@fingerTwo"].Value = fingerTwo;
                cmd.ExecuteNonQuery();
                ftp ftpClient = new ftp(@"ftp://sidbol.com", ConstactsCreadentials.userName, ConstactsCreadentials.pass);

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
            return "***** DATOS DEL JUGADOR ACTUALIZADO CORRECTAMENTE *****";
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
        public string insert(string name, string lastname, string nickname,string ci,
            string phono, string height, string weight, string skillful, string position,
            string birthday, string patchphoto, byte[] fingerOne, byte[] fingerTwo, string idDivision, string idteam)
        {
            try
            {
                beginTransaction();
                
                MySqlCommand cmd;
                string cmdString = "INSERT INTO players(name,last_name,nickname,ci,phono,height,weight,skillful_leg,position,birthday,image,fingerOne,fingerTwo)values(@name,@lastname,@nickname,@ci,@phono,@height,@weight,@skillful,@position,@birthday,@image,@fingerOne,@fingerTwo)";
                cmd = new MySqlCommand(cmdString, myConnection);
                cmd.Parameters.Add("@name", MySqlDbType.VarChar, 45);
                cmd.Parameters.Add("@lastname", MySqlDbType.VarChar, 45);
                cmd.Parameters.Add("@nickname", MySqlDbType.VarChar, 40);
                cmd.Parameters.Add("@ci", MySqlDbType.VarChar, 20);
                cmd.Parameters.Add("@phono", MySqlDbType.VarChar, 30);
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
                cmd.Parameters["@ci"].Value = ci;
                cmd.Parameters["@phono"].Value = phono;
                cmd.Parameters["@height"].Value = height;
                cmd.Parameters["@weight"].Value = weight;
                cmd.Parameters["@skillful"].Value = skillful;
                cmd.Parameters["@position"].Value = position;
                cmd.Parameters["@birthday"].Value = DateTime.Parse(birthday);
                cmd.Parameters["@image"].Value = patchphoto;
                cmd.Parameters["@fingerOne"].Value = fingerOne;
                cmd.Parameters["@fingerTwo"].Value = fingerTwo;
                cmd.ExecuteNonQuery();
                int idPlayer = Convert.ToInt32( cmd.LastInsertedId);




                /* Create Object Instance */
                ftp ftpClient = new ftp(@"ftp://sidbol.com", ConstactsCreadentials.userName, ConstactsCreadentials.pass);

                /* Upload a File */
                ftpClient.upload("public_html/afc/app/webroot/img/players/sm_" + patchphoto +".jpg", Application.StartupPath + "\\Images\\sm_"+ patchphoto+".jpg");
                myTrans.Commit();



                DataTable tabla = this.getDivisionTeamsID(idDivision, idteam);
                string idDivisionQuery = tabla.Rows[0]["id"].ToString();


                tabla = this.getChampionsId(idDivision);
                string idchampionShip = tabla.Rows[0]["championship_id"].ToString();

                insertDivisionTeamsPLayers(idPlayer.ToString(), idDivisionQuery, idchampionShip);


            }
            catch (Exception e)
            {
                myTrans.Rollback();
                return e.Message + "*************** Ha ocurrido un error , porfavor contacte con SIDBOL ********************";

            }
            finally
            {
                myConnection.Close();

            }
            return "***** JUGADOR REGISTRADO CORRECTAMENTE *****";
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

        public DataTable getDivision()
        {
            MySqlCommand command;
            MySqlDataAdapter da;
            string query = String.Format("select *from divisions");
            command = new MySqlCommand(query, myConnection);
            da = new MySqlDataAdapter(command);
            DataTable table = new DataTable();
            da.Fill(table);

            da.Dispose();
            myConnection.Close();
            return table;
        }

        public DataTable getTeam()
        {
            MySqlCommand command;
            MySqlDataAdapter da;
            string query = String.Format("select *from teams");
            command = new MySqlCommand(query, myConnection);
            da = new MySqlDataAdapter(command);
            DataTable table = new DataTable();
            da.Fill(table);

            da.Dispose();
            myConnection.Close();
            return table;
        }


        public DataTable getDivisionTeamsID(string idDivision, string idTeam)
        {

            MySqlCommand command;
            MySqlDataAdapter da;
            string query = String.Format("select *from divisions_teams where division_id ={0} and team_id = {1}", idDivision, idTeam);
            command = new MySqlCommand(query, myConnection);
            da = new MySqlDataAdapter(command);
            DataTable table = new DataTable();
            da.Fill(table);

            da.Dispose();
            myConnection.Close();
            return table;
            
        }

        public DataTable getChampionsId(string idDivision)
        {

            MySqlCommand command;
            MySqlDataAdapter da;
            string query = String.Format("select *from divisions where id = {0}", idDivision);
            command = new MySqlCommand(query, myConnection);
            da = new MySqlDataAdapter(command);
            DataTable table = new DataTable();
            da.Fill(table);

            da.Dispose();
            myConnection.Close();
            return table;

        }




        public string insertDivisionTeamsPLayers(string idplayer,string idDivisin, string idChampion)
        {
            try
            {
                beginTransaction();

                MySqlCommand cmd;
                string cmdString = "INSERT INTO divisions_teams_players(player_id,divisions_team_id,championship_id)values(@idplayer,@iddivision,@idchampion)";
                cmd = new MySqlCommand(cmdString, myConnection);
                cmd.Parameters.Add("@idplayer", MySqlDbType.Int16);
                cmd.Parameters.Add("@iddivision", MySqlDbType.Int16);
                cmd.Parameters.Add("@idchampion", MySqlDbType.Int16);
                cmd.Parameters["@idplayer"].Value = idplayer;
                cmd.Parameters["@iddivision"].Value = idDivisin;
                cmd.Parameters["@idchampion"].Value = idChampion;
     
                cmd.ExecuteNonQuery();
                myTrans.Commit();
            }
            catch (Exception e)
            {
                myTrans.Rollback();
                return e.Message + "*************** Ha ocurrido un error , porfavor contacte con SIDBOL ********************";

            }
            finally
            {
                myConnection.Close();

            }

            return "OK";
        }
    }
}