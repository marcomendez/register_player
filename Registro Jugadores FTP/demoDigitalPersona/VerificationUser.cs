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
    public partial class VerificationUser : Form, DPFP.Capture.EventHandler
    {
        public VerificationUser()
        {
            InitializeComponent();
        }
        #region globalVariables
        string name = "", last_name = "", nickname = "", height = "", weight = "", skillful_leg = "", position = "", birthday = "";

        private DPFP.Capture.Capture Capturer;
        private Dictionary<string, DPFP.Template[]> ListTemplate = new Dictionary<string, DPFP.Template[]>();
        private DPFP.Verification.Verification Verificator;
        #endregion



        private void Verification_FormClosed(object sender, FormClosedEventArgs e)
        {
            Stop();
        }

        protected void Start()
        {
            if (null != Capturer)
            {
                try
                {
                    Capturer.StartCapture();
                    SetPrompt("Using the fingerprint reader, scan your fingerprint.");
                }
                catch
                {
                    SetPrompt("Can't initiate capture!");
                }
            }
        }

        protected void Stop()
        {
            if (null != Capturer)
            {
                try
                {
                    Capturer.StopCapture();
                }
                catch
                {
                    SetPrompt("Can't terminate capture!");
                }
            }
        }


        #region base

        protected void baseInit()
        {
            try
            {
                Capturer = new DPFP.Capture.Capture();				// Create a capture operation.

                if (null != Capturer)
                    Capturer.EventHandler = this;					// Subscribe for capturing events.
                else
                    SetPrompt("Can't initiate capture operation!");
            }
            catch
            {
                MessageBox.Show("Can't initiate capture operation!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        protected void baseProcess(DPFP.Sample Sample)
        {
            // Draw fingerprint sample image.
            DrawPicture(ConvertSampleToBitmap(Sample));
        }
        protected Bitmap ConvertSampleToBitmap(DPFP.Sample Sample)
        {
            DPFP.Capture.SampleConversion Convertor = new DPFP.Capture.SampleConversion();	// Create a sample convertor.
            Bitmap bitmap = null;												            // TODO: the size doesn't matter
            Convertor.ConvertToPicture(Sample, ref bitmap);									// TODO: return bitmap as a result
            return bitmap;
        }
        protected void Message(string message)
        {
            this.Invoke(new Function(delegate()
            {
                MessageBox.Show(message);

            }));
        }

        protected void btn()
        {
            this.Invoke(new Function(delegate()
            {
                button1.Text = "Buscar por Nombre";
                button2.Text = "Buscar Automatico";
                button3.Text = "Cargar Todo";

            }));
        }

        protected void SetPrompt(string prompt)
        {
            this.Invoke(new Function(delegate()
            {
                //Prompt.Text = prompt;
            }));
        }
        protected void UserName(string name, string last_name, string nickname, string height, string weight, string skillful_leg, string position, string birthday)
        {
            this.Invoke(new Function(delegate()
            {
                try
                {
                    txt_name.Text = name;
                    txt_lastname.Text = last_name;
                    txt_nickname.Text = nickname;
                    txt_height.Text = height;

                    txt_position.Text = position;

                    txt_date.Text = birthday;
                }
                catch { }
            }));
        }
        protected void MakeReport(string message)
        {
            this.Invoke(new Function(delegate()
            {
                //StatusText.AppendText(message + "\r\n");
            }));
        }

        private void DrawPicture(Bitmap bitmap)
        {
            this.Invoke(new Function(delegate()
            {
                Picture.Image = new Bitmap(bitmap, Picture.Size);	// fit the image into the picture box
            }));
        }


        #endregion

        #region original

        public void Verify()
        {
            ClassConnectDataBase classBio = new ClassConnectDataBase();

            DataTable table = classBio.getDataWithTwoFingers("");

            int i = table.Rows.Count;
            for (int items = 0; items < i; items++)
            {
                byte[] fingerBufferOne = (byte[])table.Rows[items]["fingerOne"];
                byte[] fingerBufferTwo = (byte[])table.Rows[items]["fingerTwo"];
                DPFP.Template[] arrayTemplate = new DPFP.Template[2];
                arrayTemplate[0] = getTemplate(fingerBufferOne);
                arrayTemplate[1] = getTemplate(fingerBufferTwo);
                ListTemplate.Add(table.Rows[items]["Id"].ToString(), arrayTemplate);
            }

        }
        private DPFP.Template getTemplate(byte[] fingerBuffer)
        {
            MemoryStream ms = new MemoryStream(fingerBuffer);
            DPFP.Template template = new DPFP.Template(ms);
            return template;
        }
        protected void Init()
        {
            baseInit();
            Verificator = new DPFP.Verification.Verification();		// Create a fingerprint template verificator
            UpdateStatus(0);
        }

        protected void Process(DPFP.Sample Sample)
        {
            baseProcess(Sample);
            while (ListTemplate.Count != 0)
            {


                foreach (string keysTemplate in ListTemplate.Keys)
                {
                    Verificator = new DPFP.Verification.Verification();
                    DPFP.FeatureSet features = ExtractFeatures(Sample, DPFP.Processing.DataPurpose.Verification);
                    if (features != null)
                    {
                        DPFP.Verification.Verification.Result result = new DPFP.Verification.Verification.Result();
                        DPFP.Template[] arrayTemplate = ListTemplate[keysTemplate];
                        Verificator.Verify(features, arrayTemplate[0], ref result);
                        UpdateStatus(result.FARAchieved);
                        if (result.Verified)
                        {
                            getUerData(keysTemplate);

                            return;
                        }
                        Verificator.Verify(features, arrayTemplate[1], ref result);
                        UpdateStatus(result.FARAchieved);
                        if (result.Verified)
                        {
                            getUerData(keysTemplate);

                            return;
                        }

                    }

                }
                searchWithoutName();

            }
            if (ListTemplate.Count == 0)
            {
                MakeReport("la Huella  no existe");
                Message("Jugador no encontrado en : " + (start).ToString() + " : " + ListTemplate.Count.ToString());
                UserName(" ", " ", " ", " ", " ", " ", " ", " ");
                btn();
            }



        }
        private void getUerData(string keysTemplate)
        {
            try
            {
                MakeReport("la Huella existe");
                ClassConnectDataBase classBiometric = new ClassConnectDataBase();
                DataTable table = classBiometric.getDatawithTwoFinger(keysTemplate);


                name = table.Rows[0]["Name"].ToString();
                last_name = table.Rows[0]["last_name"].ToString();
                nickname = table.Rows[0]["nickname"].ToString();
                height = table.Rows[0]["height"].ToString();
                weight = table.Rows[0]["weight"].ToString();
                skillful_leg = table.Rows[0]["skillful_leg"].ToString();
                position = table.Rows[0]["position"].ToString();
                birthday = table.Rows[0]["birthday"].ToString();


                ftp ftpClient = new ftp(@"ftp://sidbol.com", ConstactsCreadentials.userName, ConstactsCreadentials.pass);

                /* Upload a File */
                ftpClient.download("public_html/afc/app/webroot/img/players/sm_" + table.Rows[0]["image"].ToString() + ".jpg", Application.StartupPath + "\\Images\\sm_" + table.Rows[0]["image"].ToString() + ".jpg");

                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox1.Image = Image.FromFile(Application.StartupPath + "\\Images\\sm_" + table.Rows[0]["image"].ToString() + ".jpg");

                UserName(name, last_name, nickname, height, weight, skillful_leg, position, birthday);
                btn();

            }
            catch
            {
                //Message("*************** There is a Problem, please try again ******************");
            }
        }
        protected DPFP.FeatureSet ExtractFeatures(DPFP.Sample Sample, DPFP.Processing.DataPurpose Purpose)
        {
            DPFP.Processing.FeatureExtraction Extractor = new DPFP.Processing.FeatureExtraction();	// Create a feature extractor
            DPFP.Capture.CaptureFeedback feedback = DPFP.Capture.CaptureFeedback.None;
            DPFP.FeatureSet features = new DPFP.FeatureSet();
            Extractor.CreateFeatureSet(Sample, Purpose, ref feedback, ref features);			// TODO: return features as a result?
            if (feedback == DPFP.Capture.CaptureFeedback.Good)
                return features;
            else
                return null;
        }
        private void UpdateStatus(int FAR)
        {
            // Show "False accept rate" value
            SetStatus(String.Format("False Accept Rate (FAR) = {0}", FAR));
        }
        protected void SetStatus(string status)
        {
            this.Invoke(new Function(delegate()
            {
                // StatusLine.Text = status;
            }));
        }






        #endregion


        #region implementation finger

        #region EventHandler Members:

        public void OnComplete(object Capture, string ReaderSerialNumber, DPFP.Sample Sample)
        {
            MakeReport("The fingerprint sample was captured.");
            SetPrompt("Scan the same fingerprint again.");
            Process(Sample);


        }


        public void OnFingerGone(object Capture, string ReaderSerialNumber)
        {
            MakeReport("The finger was removed from the fingerprint reader.");
        }

        public void OnFingerTouch(object Capture, string ReaderSerialNumber)
        {
            MakeReport("The fingerprint reader was touched.");
        }

        public void OnReaderConnect(object Capture, string ReaderSerialNumber)
        {
            MakeReport("The fingerprint reader was connected.");
        }

        public void OnReaderDisconnect(object Capture, string ReaderSerialNumber)
        {
            MakeReport("The fingerprint reader was disconnected.");
        }

        public void OnSampleQuality(object Capture, string ReaderSerialNumber, DPFP.Capture.CaptureFeedback CaptureFeedback)
        {
            if (CaptureFeedback == DPFP.Capture.CaptureFeedback.Good)
                MakeReport("The quality of the fingerprint sample is good.");
            else
                MakeReport("The quality of the fingerprint sample is poor.");
        }
        #endregion


        #endregion

        private void VerificationUser_Load(object sender, EventArgs e)
        {
            try
            {
                search = new ClassConnectDataBase();
                Init();
                Start();
            }
            catch
            {
                MessageBox.Show("ERROR", "Iniciando lector de huella");
            }

            //Verify();
            //Init();
            //Start();

        }
        ClassConnectDataBase search;
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {


                DataTable table = search.getDataWithTwoFingers(textBox1.Text);
                dataGridView1.Rows.Clear();
                ListTemplate.Clear();

                int i = table.Rows.Count;
                for (int items = 0; items < i; items++)
                {
                    if (!Convert.IsDBNull(table.Rows[items]["fingerOne"]))
                    {
                        byte[] fingerBufferOne = (byte[])table.Rows[items]["fingerOne"];
                        byte[] fingerBufferTwo = (byte[])table.Rows[items]["fingerTwo"];
                        DPFP.Template[] arrayTemplate = new DPFP.Template[2];
                        arrayTemplate[0] = getTemplate(fingerBufferOne);
                        arrayTemplate[1] = getTemplate(fingerBufferTwo);
                        ListTemplate.Add(table.Rows[items]["Id"].ToString(), arrayTemplate);
                        dataGridView1.Rows.Add(table.Rows[items]["Name"].ToString());
                    }

                }

            }
            catch
            {
                MessageBox.Show("ERROR", "cargado jugadores");
            }
        }

        public void searchWithoutName()
        {
            int end = start + 100;

            DataTable table = search.search_UsersWITHRANDON(start, end);
            start = end;
            ListTemplate.Clear();
            int i = table.Rows.Count;
            for (int items = 0; items < i; items++)
            {

                byte[] fingerBufferOne = (byte[])table.Rows[items]["fingerOne"];
                byte[] fingerBufferTwo = (byte[])table.Rows[items]["fingerTwo"];
                DPFP.Template[] arrayTemplate = new DPFP.Template[2];
                arrayTemplate[0] = getTemplate(fingerBufferOne);
                arrayTemplate[1] = getTemplate(fingerBufferTwo);
                ListTemplate.Add(table.Rows[items]["Id"].ToString(), arrayTemplate);

            }
        }

        int start = 1000;
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {

                searchWithoutName();
                start = 0;
                button2.Text = "buscando";
            }
            catch
            {
                MessageBox.Show("ERROR", "cargado jugadores");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DataTable table = search.getDataWithTwoFingers("");
            //dataGridView1.Rows.Clear();
            ListTemplate.Clear();

            int i = table.Rows.Count;
            for (int items = 0; items < i; items++)
            {

                byte[] fingerBufferOne = (byte[])table.Rows[items]["fingerOne"];
                byte[] fingerBufferTwo = (byte[])table.Rows[items]["fingerTwo"];
                DPFP.Template[] arrayTemplate = new DPFP.Template[2];
                arrayTemplate[0] = getTemplate(fingerBufferOne);
                arrayTemplate[1] = getTemplate(fingerBufferTwo);
                ListTemplate.Add(table.Rows[items]["Id"].ToString(), arrayTemplate);

            }
            button3.Text = "buscando";

        }


    }
}