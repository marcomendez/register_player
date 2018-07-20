using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Security.Cryptography;



namespace demoDigitalPersona
{
    public partial class RegisterUser : Form, DPFP.Capture.EventHandler
    {
        public delegate void OnTemplateEventHandler(DPFP.Template template);
        private DPFP.Capture.Capture Capturer;
        protected DPFP.Processing.Enrollment Enroller;
        string bandera = "";
        protected List<Byte[]> arrayTemplate = new List<byte[]>();

        string plain, hash;
        byte[] temp;

        SHA1 sha = null;
        StringBuilder sb = null;
        Bitmap bmp1;

        public RegisterUser()
        {
            InitializeComponent();
        }

        protected void Init()
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

        private void load_all()
        {

            Init();
            Enroller = new DPFP.Processing.Enrollment();			// Create an enrollment.
            UpdateStatus();
            Start();
            bandera = "";
        }
        private void DrawPicture(Bitmap bitmap)
        {
            this.Invoke(new Function(delegate()
            {
                Picture.Image = new Bitmap(bitmap, Picture.Size);	// fit the image into the picture box
            }));
        }
        protected Bitmap ConvertSampleToBitmap(DPFP.Sample Sample)
        {
            DPFP.Capture.SampleConversion Convertor = new DPFP.Capture.SampleConversion();	// Create a sample convertor.
            Bitmap bitmap = null;												            // TODO: the size doesn't matter
            Convertor.ConvertToPicture(Sample, ref bitmap);									// TODO: return bitmap as a result
            return bitmap;
        }

        private void Processos(DPFP.Sample Sample)
        {
            // Draw fingerprint sample image.
            DrawPicture(ConvertSampleToBitmap(Sample));
        }
        protected void Process(DPFP.Sample Sample)
        {
            try
            {
                this.Processos(Sample);

                // Process the sample and create a feature set for the enrollment purpose.
                DPFP.FeatureSet features = ExtractFeatures(Sample, DPFP.Processing.DataPurpose.Enrollment);

                // Check quality of the sample and add to enroller if it's good
                if (features != null) try
                    {
                        // MakeReport("The fingerprint feature set was created.");
                        Enroller.AddFeatures(features);		// Add feature set to template.
                    }
                    finally
                    {
                        UpdateStatus();
                        // Check if template has been created.
                        switch (Enroller.TemplateStatus)
                        {
                            case DPFP.Processing.Enrollment.Status.Ready:	// report success and stop capturing
                                SetPrompt("Click Close, and then click Fingerprint Verification.");
                                bandera = "ok";
                                Stop();

                                break;

                            case DPFP.Processing.Enrollment.Status.Failed:	// report failure and restart capturing
                                Enroller.Clear();
                                Stop();
                                UpdateStatus();
                                Start();
                                break;
                        }
                    }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Las huellas no coinciden");
            }
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
        protected void SetPrompt(string prompt)
        {
            this.Invoke(new Function(delegate()
            {
                //Prompt.Text = prompt;
            }));
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
        private void UpdateStatus()
        {
            // Show number of samples needed.
            SetStatus(String.Format("Fingerprint samples needed: {0}", Enroller.FeaturesNeeded));
        }

        protected void SetStatus(string status)
        {
            this.Invoke(new Function(delegate()
            {
                StatusLine.Text = status;
            }));
        }

        protected void Message(string message)
        {
            this.Invoke(new Function(delegate()
            {
                MessageBox.Show(message, "REGISTRO", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }));
        }


        protected void Enabled_ButtonOne(Boolean value)
        {
            this.Invoke(new Function(delegate()
            {
                if (value == false)
                    button2.BackColor = Color.Black;
                else
                    button2.BackColor = Color.Green;
                button2.Enabled = value;
            }));
        }


        protected void Enabled_ButtonTwo(Boolean value)
        {
            this.Invoke(new Function(delegate()
            {
                if (value == false)
                    button3.BackColor = Color.Black;
                else
                    button3.BackColor = Color.Green;
                button3.Enabled = value;
            }));
        }

        #region EventHandler Members:

        public void OnComplete(object Capture, string ReaderSerialNumber, DPFP.Sample Sample)
        {
            SetPrompt("Scan the same fingerprint again.");
            Process(Sample);
            if (bandera == "ok")
            {
                MemoryStream fingerdata = new MemoryStream();
                Enroller.Template.Serialize(fingerdata);
                fingerdata.Position = 0;
                BinaryReader br = new BinaryReader(fingerdata);
                Byte[] bytes = br.ReadBytes((Int32)fingerdata.Length);

                arrayTemplate.Add(bytes);

                Enabled_ButtonOne(false);
                Enabled_ButtonTwo(true);
                if (arrayTemplate.Count == 2)
                {
                    registerUser();
                    Enabled_ButtonOne(false);
                    Enabled_ButtonTwo(false);
                }

            }

        }
        protected virtual void registerUser()
        {
            ClassConnectDataBase crud = new ClassConnectDataBase();
            string result = crud.insert(txt_name.Text, txt_lastname.Text, txt_club.Text,txt_carnet.Text +" " + combo_carnet.Text,txt_phono.Text ,"", "",
               "",txt_position.Text, txt_date.Text, nameCode, arrayTemplate[0], arrayTemplate[1]);
            SetStatus("REGISTRO REALIZADO CORRECTAMENTE");
            Message(result);
        }

        public void OnFingerGone(object Capture, string ReaderSerialNumber)
        {

        }

        public void OnFingerTouch(object Capture, string ReaderSerialNumber)
        {

        }

        public void OnReaderConnect(object Capture, string ReaderSerialNumber)
        {
            // MakeReport("The fingerprint reader was connected.");
        }

        public void OnReaderDisconnect(object Capture, string ReaderSerialNumber)
        {

        }

        public void OnSampleQuality(object Capture, string ReaderSerialNumber, DPFP.Capture.CaptureFeedback CaptureFeedback)
        {

        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {

            string patch1 = "";
            pictureBox1.Image = searchPhoto(ref patch1);
            textBox2.Text = patch1;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }
        public Image searchPhoto(ref string patch)
        {
            Bitmap img = null;
            OpenFileDialog open = new OpenFileDialog();
            DialogResult resul = open.ShowDialog();
            if (resul == DialogResult.OK)
            {
                patch = open.FileName;
                img = (Bitmap)Image.FromFile(open.FileName);
            }
            return img;


        }

        private string hashCode(string fullname)
        {

            plain = fullname;

            sha = new SHA1CryptoServiceProvider();
            // This is one implementation of the abstract class SHA1.
            temp = sha.ComputeHash(Encoding.UTF8.GetBytes(plain));


            //storing hashed vale into byte data type
            sb = new StringBuilder();
            for (int i = 0; i < temp.Length; i++)
            {
                sb.Append(temp[i].ToString("x2"));
            }

            hash = sb.ToString();
            return hash;
        }
        public string nameCode;
        private void button1_Click_1(object sender, EventArgs e)
        {
            string patch1 = "";
            pictureBox1.Image = searchPhoto(ref patch1);
            textBox2.Text = patch1;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

            nameCode = hashCode(txt_name.Text + " " + txt_lastname.Text);

            VaryQualityLevel(textBox2.Text, nameCode + ".jpg");


        }

        private void VaryQualityLevel(string imagePath, string name)
        {
            // Get a bitmap.
            bmp1 = new Bitmap(imagePath);
            System.Drawing.Imaging.ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);

            // Create an Encoder object based on the GUID 
            // for the Quality parameter category.
            System.Drawing.Imaging.Encoder myEncoder =
                System.Drawing.Imaging.Encoder.Quality;

            EncoderParameters myEncoderParameters = new EncoderParameters(1);

            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 50L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            bmp1.Save(Application.StartupPath + "\\Images\\sm_" + name, jgpEncoder, myEncoderParameters);


            bmp1.Dispose();


        }
        private ImageCodecInfo GetEncoder(ImageFormat format)
        {

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
        private void RegisterUser_FormClosed(object sender, FormClosedEventArgs e)
        {
            Stop();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            load_all();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            load_all();
        }

        private void RegisterUser_Load(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            arrayTemplate = new List<byte[]>();
            arrayTemplate.Add(null);
            arrayTemplate.Add(null);
            registerUser();
        }

    }

}
