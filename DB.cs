using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reconocimiento_facial
{
    public class DataBase
    {
        private OleDbConnection conn;
        public string[] Nombre;
        public string[] Matricula;
        public string[] Carrera;
        public string[] Correo;
        public string[] Fecha_Naci;
        private byte[] rostro;
        public List<byte[]> Rostro = new List<byte[]>();
        public int TotalUser;
        public DataBase()
        {
            conn = new OleDbConnection("Provider = Microsoft.Jet.OLEDB.4.0; Data Source = BD_Rostro.mdb");
            conn.Open();
        }
        public bool GuardarImagen(string Nombre, string Matricula, string Carrera, string Correo, string Fecha_Nacimiento, byte[] abImagen)
        {
            conn.Open();
            OleDbCommand comm = new OleDbCommand("INSERT INTO TablaR (Nombre,Matricula,Carrera,Correo,Fecha_Nacimiento,Rostros) VALUES ('" + Nombre + "','" + Matricula + "','" + Carrera + "','" + Correo + "','" + Fecha_Nacimiento + "',?)", conn);
            OleDbParameter parImagen = new OleDbParameter("@Rostro", OleDbType.VarBinary, abImagen.Length);
            parImagen.Value = abImagen;
            comm.Parameters.Add(parImagen);
            int iResultado = comm.ExecuteNonQuery();
            conn.Close();
            return Convert.ToBoolean(iResultado);
        }

        public DataTable ObtenerBytesImagen()
        {
            string sql = "SELECT IdImage,Nombre,Matricula,Carrera,Correo,Fecha_Nacimiento,Rostros FROM TablaR";
            OleDbDataAdapter adaptador = new OleDbDataAdapter(sql, conn);
            DataTable dt = new DataTable();
            adaptador.Fill(dt);
            int cont = dt.Rows.Count;
            Nombre = new string[cont];
            Matricula = new string[cont];
            Carrera = new string[cont];
            Correo = new string[cont];
            Fecha_Naci = new string[cont];

            for (int i = 0; i < cont; i++)
            {
                Nombre[i] = dt.Rows[i]["Nombre"].ToString();
                Matricula[i] = dt.Rows[i]["Matricula"].ToString();
                Carrera[i] = dt.Rows[i]["Carrera"].ToString();
                Correo[i] = dt.Rows[i]["Correo"].ToString();
                Fecha_Naci[i] = dt.Rows[i]["Fecha_Nacimiento"].ToString();
                rostro = (byte[])dt.Rows[i]["Rostros"];
                Rostro.Add(rostro);
            }
            TotalUser = dt.Rows.Count;
            conn.Close();
            return dt;
        }

        public void ConvertImgToBinary(string Nombre, string Matricula, string Carrera, string Correo, string Fecha_Nacimiento, Image Img)
        {
            Bitmap bmp = new Bitmap(Img);
            MemoryStream MyStream = new MemoryStream();
            bmp.Save(MyStream, System.Drawing.Imaging.ImageFormat.Bmp);

            byte[] abImagen = MyStream.ToArray();
            GuardarImagen(Nombre, Matricula, Carrera, Correo, Fecha_Nacimiento, abImagen);
        }

        public Image ConvertByteToImg(int con)
        {
            Image FetImg;
            byte[] img = Rostro[con];
            MemoryStream ms = new MemoryStream(img);
            FetImg = Image.FromStream(ms);
            ms.Close();
            return FetImg;

        }
    }
}
