using System;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using TwainLib;
using System.Configuration;
using Google.Cloud.Storage.V1;
using Google.Apis.Auth.OAuth2;
using GoogleCloudSamples.Services;

namespace Escaner_Twain
{
    public partial class frmEscaner : System.Windows.Forms.Form, IMessageFilter {
    

        #region Declaración variables
        private string sFileName;
        private string sErrorMsg;
        private string sWSResponse;

        private bool bMsgFilter;
        private bool bImageReady;
        private bool bImageScanned;
        private Twain tw;
        private int iPicNumber = 0;
        private Single valZoom = 1;


        BITMAPINFOHEADER BmpInfoHeader;

        #endregion

        public frmEscaner(string sImgName="")
        {
            InitializeComponent();
            this.sFileName = sImgName;
            this.sErrorMsg = String.Empty;
            this.sWSResponse = String.Empty;
            this.bImageReady = false;
            tw = new Twain();
            tw.Init(this.Handle);
        }

        /// <summary>
        /// Obtiene el nombre del archivo de la imagen
        /// </summary>
        /// <returns></returns>
        public string getWSResponse()
        {
            return this.sWSResponse;
        }

        /// <summary>
        /// Permite conocer si se terminó el escaneo
        /// </summary>
        /// <returns></returns>
        public bool FinishedScanner()
        {
            return this.bImageReady;
        }

        /// <summary>
        /// Devuelve una cadena que contiene el último mensaje de error 
        /// obtenido desde el Servicio Web
        /// </summary>
        /// <returns></returns>
        public string getErrorMsg()
        {
            return this.sErrorMsg;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                tw.Finish();
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Carga una imagen desde el escaner y la coloca en el formulario
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoadImage_Click(object sender, EventArgs e)
        {

            try
            {

                if (!bMsgFilter)
                {
                    this.Enabled = false;
                    bMsgFilter = true;
                    Application.AddMessageFilter(this);
                }

                tw.Acquire();
            }
            catch (Exception ex) { }
        }

        /// <summary>
        /// Función para cargar un archcivo directamente a la nube
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string jsonPath = ConfigurationManager.AppSettings["jsonPath"].ToString();
                string projectId = ConfigurationManager.AppSettings["projectId"].ToString();
                string bucketName = ConfigurationManager.AppSettings["bucketName"].ToString();
                string filesPath = ConfigurationManager.AppSettings["filesPath"].ToString();
                

                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Archivos PDF|*.pdf";
                if (ofd.ShowDialog() == DialogResult.OK)
                {

                    string fileName = ofd.FileName;
                    /**
                    Image img = Image.FromFile(fileName);
                    this.activarElementosForm(img);
                */
                    this.UploadFile(bucketName, fileName);
                    MessageBox.Show("Archivo almacenado", "Listo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
            }

        }

        private void UploadFile(string bucketName, string localPath, string objectName = null)
        {
            string jsonPath = ConfigurationManager.AppSettings["jsonPath"].ToString();
            var credential = GoogleCredential.FromFile(jsonPath);

            // Instantiates a client.
            StorageClient storageClient = StorageClient.Create(credential);


            using (var f = File.OpenRead(localPath))
            {
                objectName = objectName ?? Path.GetFileName(localPath);
                storageClient.UploadObject(bucketName, objectName, "application/pdf", f);
                Console.WriteLine($"Uploaded {objectName}.");
            }
            GC.Collect();
        }

        /// <summary>
        /// Almacena la imagen escaneada en el servidor, por medio de un Servicio Web
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSaveImage_Click(object sender, EventArgs e)
        {
            try
            {
                string jsonPath = ConfigurationManager.AppSettings["jsonPath"].ToString();
                string projectId = ConfigurationManager.AppSettings["projectId"].ToString();
                string bucketName = ConfigurationManager.AppSettings["bucketName"].ToString();
                string filesPath = ConfigurationManager.AppSettings["filesPath"].ToString();
                string imgName = DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".jpeg";

                string fullNameImg = filesPath + imgName;

                Image img = pictureBoxEscaner.Image;

                img.Save(@fullNameImg, ImageFormat.Jpeg);

                ImageUploader imgUploader = new ImageUploader(bucketName);

                string url = string.Empty;
                var task = imgUploader.UploadImageAsync(fullNameImg);
                url = await task;

                // this.UploadFile(bucketName, fullNameImg);

                // Bitmap bmp = new Bitmap(pictureBoxEscaner.Image);

                //string imgSerializada = this.SerializarImagen(bmp);
                //byte[] imgZip = this.Zip(imgSerializada);


                this.bImageReady = true;
                this.sErrorMsg = String.Empty;
                MessageBox.Show("Imagen almacenada: " + url, "Imagen guardada", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // this.Close();
            }
        }

        /// <summary>
        /// Método auxiliar para la compresión de cadenas
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        private void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }

        /// <summary>
        /// Función para comprimir cadenas
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private byte[] Zip(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    //msi.CopyTo(gs);
                    CopyTo(msi, gs);
                }

                return mso.ToArray();
            }
        }

        /// <summary>
        /// Permite convertir una imagen Bitmap en String de Base 64
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        private string SerializarImagen(Bitmap bmp)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                return Convert.ToBase64String(stream.GetBuffer());
            }
        }

        /// <summary>
        /// Cambia el dispositivo desde donde se realizará el escaneo de imagenes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void elegirDispositivoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tw.Select();
        }

        /// <summary>
        /// Cierra el formulario
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Función para cambiar la bandera del formulario, indicando que el escaneo finalizó
        /// </summary>
        private void EndingScan()
        {
            if (bMsgFilter)
            {
                Application.RemoveMessageFilter(this);
                bMsgFilter = false;
                this.Enabled = true;
                this.Activate();
            }
        }
        
        /// <summary>
        /// Filtros del formulario para detectar los cambios de estado del escaneo
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        bool IMessageFilter.PreFilterMessage(ref Message m)
        {
            TwainCommand cmd = tw.PassMessage(ref m);
            if (cmd == TwainCommand.Not)
            {
                return false;
            }

            switch (cmd)
            {
                case TwainCommand.CloseRequest:
                    {
                        EndingScan();
                        tw.CloseSrc();
                        break;
                    }
                case TwainCommand.CloseOk:
                    {
                        EndingScan();
                        tw.CloseSrc();
                        break;
                    }
                case TwainCommand.DeviceEvent:
                    {
                        break;
                    }
                case TwainCommand.TransferReady:
                    {
                        this.setImageFromScanner();
                        break;
                    }
                default:
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Permite colocar en el formulario la imagen enviada desde el escanner
        /// </summary>
        private void setImageFromScanner()
        {
            IntPtr ptrDibhand;
            IntPtr ptrBmp;
            IntPtr ptrBmpPixelInfo;
            
            ArrayList pics = tw.TransferPictures();
            EndingScan();
            tw.CloseSrc();
            iPicNumber++;

            for (int i = 0; i < pics.Count; i++)
            {
                IntPtr imgPtr = (IntPtr)pics[i];
                int picnum = i + 1;

                ptrDibhand = imgPtr;
                ptrBmp = GlobalLock(ptrDibhand);
                ptrBmpPixelInfo = GetPixelInfo(ptrBmp);

                BmpInfoHeader.biWidth.ToString();
                BmpInfoHeader.biHeight.ToString();
                BmpInfoHeader.biBitCount.ToString();

                IntPtr img = IntPtr.Zero;
                // Crea el bitmap
                Bitmap bmp = WithHBitmap(ptrBmp);

                Image targetImg = this.ResizeImage(720, bmp);
                targetImg = this.CreateGrayScaleBitmap((Bitmap)targetImg);

                this.activarElementosForm(targetImg);
            }
        }

        /// <summary>
        /// Funcion parcial para activar elementose en el formulario
        /// </summary>
        /// <param name="targetImg"></param>
        private void activarElementosForm(Image targetImg)
        {
            this.pictureBoxEscaner.Image = targetImg;
            this.btnSaveImage.Visible = true;
            this.bImageScanned = true;

            this.Panel1.HorizontalScroll.Maximum = 0;
            this.Panel1.AutoScroll = false;
            this.Panel1.VerticalScroll.Visible = false;
            this.Panel1.AutoScroll = true;

            this.Panel1.AutoScrollMinSize = pictureBoxEscaner.Image.Size;
            this.buttonZoomMas.Visible = true;
            this.buttonZoomMenos.Visible = true;
            this.label1.Visible = true;
        }

        /// <summary>
        /// Función que permite aplicar un filtro de Escala de grises a una imagen Bitmap
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private Bitmap CreateGrayScaleBitmap(Bitmap source)
        {
            int widht = source.Width;
            int height = source.Height;
            Color actualColor, newColor;
            Bitmap target = new Bitmap(widht, height);
            
            // Recorre los pixeles de la imagen
            for (int x = 0; x < widht; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    actualColor = source.GetPixel(x, y);
                    newColor = Color.FromArgb(actualColor.R, actualColor.R, actualColor.R);
                    target.SetPixel(x, y, newColor);
                }
            }
            return target;
        }

        /// <summary>
        /// Obtiene una imagen a partir de un IntPtr (Utilizado por Twain)
        /// </summary>
        /// <param name="dibPtr"></param>
        /// <returns></returns>
        private Bitmap WithHBitmap(IntPtr dibPtr)
        {
            Type bmiTyp = typeof(BITMAPINFOHEADER);
            BITMAPINFOHEADER bmi = (BITMAPINFOHEADER)Marshal.PtrToStructure(dibPtr, bmiTyp);
            if (bmi.biSizeImage == 0)
                bmi.biSizeImage = ((((bmi.biWidth * bmi.biBitCount) + 31) & ~31) >> 3) * Math.Abs(bmi.biHeight);
            if ((bmi.biClrUsed == 0) && (bmi.biBitCount < 16))
                bmi.biClrUsed = 1 << bmi.biBitCount;

            IntPtr pixPtr = new IntPtr((int)dibPtr + bmi.biSize + (bmi.biClrUsed * 4));		// pointer to pixels

            IntPtr img = IntPtr.Zero;
            int st = GdipCreateBitmapFromGdiDib(dibPtr, pixPtr, ref img);
            if ((st != 0) || (img == IntPtr.Zero))
                throw new ArgumentException("Invalid bitmap for GDI+", "IntPtr dibPtr");

            IntPtr hbitmap;
            st = GdipCreateHBITMAPFromBitmap(img, out hbitmap, 0);
            if ((st != 0) || (hbitmap == IntPtr.Zero))
            {
                GdipDisposeImage(img);
                throw new ArgumentException("can't get HBITMAP with GDI+", "IntPtr dibPtr");
            }

            Bitmap tmp = Image.FromHbitmap(hbitmap);			// 'tmp' is wired to hbitmap (unfortunately)
            Bitmap result = new Bitmap(tmp);					// 'result' is a copy (stand-alone)
            tmp.Dispose(); tmp = null;
            bool ok = DeleteObject(hbitmap); hbitmap = IntPtr.Zero;
            st = GdipDisposeImage(img); img = IntPtr.Zero;
            return result;
        }

        /// <summary>
        /// Redimensiona una imagen mantiniendo la relación
        /// </summary>
        /// <param name="newWidthSize"></param>
        /// <param name="originalImage"></param>
        /// <returns></returns>
        private Image ResizeImage(int newWidthSize, Image originalImage)
        {

            if (originalImage.Width <= newWidthSize)
            {
                return originalImage;

            }

            int newHeight = originalImage.Height * newWidthSize / originalImage.Width;

            return originalImage.GetThumbnailImage(newWidthSize, newHeight, null, IntPtr.Zero);
        }

        /// <summary>
        /// Metodo auxiliar para Twain para la obtención de propiedades de una imagen en memoria
        /// </summary>
        /// <param name="bmpptr"></param>
        /// <returns></returns>
        private IntPtr GetPixelInfo(IntPtr bmpptr)
        {
            Rectangle rectBmp;
            BmpInfoHeader = new BITMAPINFOHEADER();
            Marshal.PtrToStructure(bmpptr, BmpInfoHeader);

            rectBmp = new Rectangle(0, 0, 0, 0);
            rectBmp.X = rectBmp.Y = 0;
            rectBmp.Width = BmpInfoHeader.biWidth;
            rectBmp.Height = BmpInfoHeader.biHeight;

            if (BmpInfoHeader.biSizeImage == 0)
                BmpInfoHeader.biSizeImage = ((((BmpInfoHeader.biWidth * BmpInfoHeader.biBitCount) + 31) & ~31) >> 3) * BmpInfoHeader.biHeight;

            int p = BmpInfoHeader.biClrUsed;
            if ((p == 0) && (BmpInfoHeader.biBitCount <= 8))
                p = 1 << BmpInfoHeader.biBitCount;
            p = (p * 4) + BmpInfoHeader.biSize + (int)bmpptr;
            return (IntPtr)p;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        internal class BITMAPINFOHEADER
        {
            public int biSize;
            public int biWidth;
            public int biHeight;
            public short biPlanes;
            public short biBitCount;
            public int biCompression;
            public int biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public int biClrUsed;
            public int biClrImportant;
        }

        #region twain imports
        [DllImport("gdi32.dll", ExactSpelling = true)]
        internal static extern int SetDIBitsToDevice(IntPtr hdc, int xdst, int ydst, int width, int height, int xsrc, int ysrc, int start,
            int lines, IntPtr bitsptr, IntPtr bmiptr, int color);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        internal static extern IntPtr GlobalLock(IntPtr handle);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        internal static extern IntPtr GlobalFree(IntPtr handle);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern void OutputDebugString(string outstr);

        [DllImport("gdiplus.dll", ExactSpelling = true)]
        internal static extern int GdipCreateBitmapFromGdiDib(IntPtr bminfo, IntPtr pixdat, ref IntPtr image);

        [DllImport("gdiplus.dll", ExactSpelling = true)]
        private static extern int GdipCreateHBITMAPFromBitmap(IntPtr image, out IntPtr hbitmap, int bkg);

        [DllImport("gdiplus.dll", ExactSpelling = true)]
        private static extern int GdipDisposeImage(IntPtr image);

        [DllImport("gdi32.dll", ExactSpelling = true)]
        private static extern bool DeleteObject(IntPtr obj);

        #endregion

        private void frmEscaner_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;
        }

        /// <summary>
        /// Permite disminuir el zoon del picturebox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonZoomMenos_Click(object sender, EventArgs e)
        {
            this.pictureBoxEscaner.Height -= 150;
            this.pictureBoxEscaner.Width -= 150;
        }

        /// <summary>
        /// Permite aumentar el zoom del picturebox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonZoomMas_Click(object sender, EventArgs e)
        {
            this.pictureBoxEscaner.Height += 150;
            this.pictureBoxEscaner.Width += 150;
        }

    }

}
