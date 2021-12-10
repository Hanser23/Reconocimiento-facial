using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Data.OleDb;
using System.Speech.Synthesis;
using System.Media;
using System.Runtime.InteropServices;

namespace Reconocimiento_facial
{
    public partial class Frm_Reconocimiento : Form
    {
        public int altura, ancho;
        public string[] Labels;
        public string[] LabelsCed;
        DataBase Data = new DataBase();
        int CN= 0;
        SoundPlayer media = new SoundPlayer();
        SpeechSynthesizer vos = new SpeechSynthesizer();
        Image<Bgr, Byte> currentFrame;
        Capture grabber;
        HaarCascade face;
        HaarCascade eye;
        MCvFont font = new MCvFont(FONT.CV_FONT_HERSHEY_TRIPLEX, 0.4d, 0.4d);
        Image<Gray, byte> result, TrainedFace = null;
        Image<Gray, byte> resultCed, TrainedFaceCD = null;
        Image<Gray, byte> resultFN, TrainedFaceFN = null;
        Image<Gray, byte> resultCarrera, TrainedFaceSG = null;
        Image<Gray, byte> resultCorreo, TrainedFaceAL = null;


        Image<Gray, byte> gray = null;
        List<Image<Gray, byte>> trainingImages = new List<Image<Gray, byte>>();
        List<string> labels = new List<string>();
        List<string> labelsCed = new List<string>();
        List<string> labelsFecha = new List<string>();
        List<string> labelsCarrera = new List<string>();
        List<string> labelscorreo = new List<string>();


        List<string> NamePersons = new List<string>();
        List<string> MatriculaPersons = new List<string>();
        List<string> Fecha_NaciPersons = new List<string>();
        List<string> CarreraPersons = new List<string>();
        List<string> correoPersons = new List<string>();
        int ContTrain, NumLabels, t;
        string name, Labelsinfo, names = null;
        string Matricula, Matriculasinfo, Matriculas = null;
        string Fecha_Nacimiento, Fecha_Nacimientoinfo, Fecha_Nacimientos = null;
        string Carrera, Carrerainfo, Carrerass = null;

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void lblNumeroDetect_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            FRPrincipal f = new FRPrincipal();
            this.Close();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        string correo, Correoinfo, Correos = null;

        private void pictureBox2_Click_1(object sender, EventArgs e)
        {
            FRPrincipal f = new FRPrincipal();
            f.Show();
            this.Close();
        }

        private void imageBoxFrameGrabber_Click(object sender, EventArgs e)
        {

        }

        public Frm_Reconocimiento()
        {
            InitializeComponent();
            altura = this.Height; ancho = this.ancho;
            face = new HaarCascade("haarcascade_frontalface_default.xml");
            try
            {
                Data.ObtenerBytesImagen();
                string[] Labels = Data.Nombre;
                string[] LabelsCed = Data.Matricula;
                string[] LabelsFecha = Data.Fecha_Naci;
                string[] LabelsCarrera = Data.Carrera;
                string[] Labelscorreo = Data.Correo;
                NumLabels = Data.TotalUser;
                ContTrain = NumLabels;
                string LoadFaces;

                for (int tf = 0; tf < NumLabels; tf++)
                {
                    CN= tf;
                    Bitmap bmp = new Bitmap(Data.ConvertByteToImg(CN));
                    trainingImages.Add(new Image<Gray, byte>(bmp));
                    labels.Add(Labels[tf]);
                    labelsCed.Add(LabelsCed[tf]);
                    labelsFecha.Add(LabelsFecha[tf]);
                    labelsCarrera.Add(LabelsCarrera[tf]);
                    labelscorreo.Add(Labelscorreo[tf]);
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e + "No hay ningun rosto registrado).", "Cargar rostros", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }


        private void Reconocer()
        {
            try
            {
                grabber = new Capture();
                grabber.QueryFrame();
                Application.Idle += new EventHandler(FrameGrabber);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void FrameGrabber(object sender, EventArgs e)
        {
            lblNumeroDetect.Text = "0";
            NamePersons.Add("");
            MatriculaPersons.Add("");
            Fecha_NaciPersons.Add("");
            CarreraPersons.Add("");
            correoPersons.Add("");


            try
            {
                currentFrame = grabber.QueryFrame().Resize(320, 240, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                gray = currentFrame.Convert<Gray, Byte>();

                MCvAvgComp[][] facesDetected = gray.DetectHaarCascade(face, 1.2, 10, Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(20, 20));

                foreach (MCvAvgComp f in facesDetected[0])
                {
                    t = t + 1;
                    result = currentFrame.Copy(f.rect).Convert<Gray, byte>().Resize(100, 100, INTER.CV_INTER_CUBIC);
                    resultCed = currentFrame.Copy(f.rect).Convert<Gray, byte>().Resize(100, 100, INTER.CV_INTER_CUBIC);
                    resultFN = currentFrame.Copy(f.rect).Convert<Gray, byte>().Resize(100, 100, INTER.CV_INTER_CUBIC);
                    resultCarrera = currentFrame.Copy(f.rect).Convert<Gray, byte>().Resize(100, 100, INTER.CV_INTER_CUBIC);
                    resultCorreo = currentFrame.Copy(f.rect).Convert<Gray, byte>().Resize(100, 100, INTER.CV_INTER_CUBIC);
                    currentFrame.Draw(f.rect, new Bgr(Color.LightGreen), 1);

                    if (trainingImages.ToArray().Length != 0)
                    {
                        MCvTermCriteria termCrit = new MCvTermCriteria(ContTrain, 0.001);

                        EigenObjectRecognizer recognizer = new EigenObjectRecognizer(trainingImages.ToArray(), labels.ToArray(), ref termCrit);
                        EigenObjectRecognizer recognizerCED = new EigenObjectRecognizer(trainingImages.ToArray(), labelsCed.ToArray(), ref termCrit);
                        EigenObjectRecognizer recognizerFN = new EigenObjectRecognizer(trainingImages.ToArray(), labelsFecha.ToArray(), ref termCrit);
                        EigenObjectRecognizer recognizerSG = new EigenObjectRecognizer(trainingImages.ToArray(), labelsCarrera.ToArray(), ref termCrit);
                        EigenObjectRecognizer recognizerAL = new EigenObjectRecognizer(trainingImages.ToArray(), labelscorreo.ToArray(), ref termCrit);
                        var fa = new Image<Gray, byte>[trainingImages.Count]; 

                        name = recognizer.Recognize(result);
                        Matricula = recognizerCED.Recognize(resultCed);
                        Fecha_Nacimiento = recognizerFN.Recognize(resultFN);
                        Carrera = recognizerSG.Recognize(resultCarrera);
                        correo = recognizerAL.Recognize(resultCorreo);
                        currentFrame.Draw(name, ref font, new Point(f.rect.X - 2, f.rect.Y - 2), new Bgr(Color.YellowGreen));
                        if (name == "")
                        {
                            currentFrame.Draw("Usuario Desconocido", ref font, new Point(f.rect.X - 2, f.rect.Y - 2), new Bgr(Color.YellowGreen));
                        }
                    }

                    NamePersons[t - 1] = name;
                    MatriculaPersons[t - 1] = Matricula;
                    Fecha_NaciPersons[t - 1] = Fecha_Nacimiento;
                    CarreraPersons[t - 1] = Carrera;
                    correoPersons[t - 1] = correo;
                    NamePersons.Add("");
                    MatriculaPersons.Add("");
                    Fecha_NaciPersons.Add("");
                    CarreraPersons.Add("");
                    correoPersons.Add("");
                    lblNumeroDetect.Text = facesDetected[0].Length.ToString();
                    lblNadie.Text = name;
                    lblCedResultado.Text = Matricula;
                    lblFNResultado.Text = Fecha_Nacimiento;
                    lblCarreraResultado.Text = Carrera;
                    lblCorreoResultado.Text = correo;
                }
                t = 0;

                for (int nnn = 0; nnn < facesDetected[0].Length; nnn++)
                {
                    names = names + NamePersons[nnn] + ", ";
                    Matriculas = Matriculas + MatriculaPersons[nnn] + ", ";
                    Carrerass = Carrerass + CarreraPersons[nnn] + ", ";
                    Correos = Correos + correoPersons[nnn] + ", ";
                    Fecha_Nacimientos = Fecha_Nacimientos + Fecha_NaciPersons[nnn] + ", ";
                }

                imageBoxFrameGrabber.Image = currentFrame;
                name = "";
                Matricula = "";
                Fecha_Nacimiento = "";
                Carrera = "";
                correo = "";

                NamePersons.Clear();
                MatriculaPersons.Clear();
                Fecha_NaciPersons.Clear();
                CarreraPersons.Clear();
                correoPersons.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void Frm_Reconocimiento_Load(object sender, EventArgs e)
        {
            Reconocer();
 
        }

        private void btn_Salir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_mini_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void btn_Registrar_Click(object sender, EventArgs e)
        {
            try
            {
                Desconectar();
                FRRegitrar re = new FRRegitrar();
                Bg fil = new Bg();
                fil.Show();
                re.ShowDialog();
                fil.Hide();
            }
            catch
            {

            }

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }
        private void Desconectar()
        {
            Application.Idle -= new EventHandler(FrameGrabber);
            grabber.Dispose();
            imageBoxFrameGrabber.ImageLocation = "img/1.jpg";
            lblNadie.Text = string.Empty;
            lblNumeroDetect.Text = string.Empty;
            
        }
    }
}
