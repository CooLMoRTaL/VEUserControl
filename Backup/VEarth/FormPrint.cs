/* Created by SharpDevelop.
 * Developed by: Juergen Thomas, Berlin (Germany)
 * email adress: post(at)vs-polis.de
 *
 * Provided for NET 1.1 and 2.0
 * ----------------------------
 * To adapt the code for the used version, change the #define instructions.
 *
 * Project: general tool to print or save a form or any other control
 * 
 * License: you can use this code 'as is' without any warrenty.
 * 
 * Description
 * -----------
 * http://www.codeproject.com/csharp/FormPrintPackage.asp   (English)
 * http://www.mycsharp.de/wbb2/thread.php?threadid=29704    (deutsch)
 * 
 * History
 * -------
 * 12/17/2006
 * First version
 *
 * 12/19/2006
 * 1. added:   clearing resources (Bitmap, Graphics)
 * 2. added:   printing as portrait or landscape
 * 3. added:   another method Execute() to decide printing either portrait or landscape
 *
 * 12/23/2006
 * 1. changed: a shorter way to copy the control's graphics to the bitmap
 * 2. comment  changed: at present, no adaptation to NET 1.1
 *
 * 12/25/2006
 * 1. added:   copy the control's graphics to the bitmap with NET 1.1
 * 2. changed: variables and methods are grouped newly,
 *             but with no other features
 * 3. comment  changed: adaptation to NET 1.1
 * 4. commend  added: known problems
 *
 * 01/13/2007
 * 1. NET 1.1  changed: form can be printed including title and menu bar
 * 2. changed: public method Execute() splitted to Print() resp. Save()
 * 3. changed: Print() allows several parameters: PrinterName, Margins, Orientation
 * 4. changed: printing can compress to papersize with margins if necessary
 * 5. changed: most of the parameters can be omitted
 * 6. changed: printer memory problem managed by converting
 *             to PixelFormat.Format8bppIndexed and GrayScaled bitmap
 *             instead of colored PixelFormat.Canonical
 * 7. changed: Execute() uses progress label while printing a large control
 *			   (more than 40000 pixels, e.g. 200x200)
 * 8. changed: private method Execute() splitted to StartPrinting() resp. StartSaving()
 * 
 * 01/22/2007
 * 1. changed: use #define to change NET version
 * 2. changed: converting to PixelFormat.Format8bppIndexed now managed 
 *             by enum ColorDepthReduction
 *
 * 03/22/2007
 * RichTextBox and WebBrowser cannot be copied by Control.DrawToBitmap(); therefore
 * we need DllImport("gdi32.dll") both, for NET 1.1 and NET 2.0
 * this leeds to the following changes:
 * 1. changed  using System.Runtime.InteropServices; for NET 2.0, too
 * 2. renamed  GetBitmap() for NET 1.1 to GetBitmapGdi()
 * 			   FormPrint calls GetBitmap() and leeds to GetBitmapGdi()
 * 3. changed  GetBitmap() for NET 2.0 isn't used in all situations:
 * 			   does Control contain any RichTextBox or WebBrowser Control?
 * 			   true  => FormPrint calls GetBitmapGdi()
 * 			   false => FormPrint calls GetBitmap() using DrawToBitmap()
 * 
 *  */

#define NET_V20
#undef  NET_V11

using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Printing;
using System.Drawing.Imaging;
using System.Collections;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;

namespace JThomas.Tools
{
    
	/// <summary>
	/// Description of FormPrint.
	/// </summary>
	/// FormPrint is a simple class to print a winform or any winform control, or save
	/// it into a file as bitmap format.
	public sealed class FormPrint
	{

	    #region Public: Enumeration ColorDepthReduction
		/// <summary>
		/// ColorDepthReduction decides if pixelformat has to be changed
		/// </summary>
		public enum ColorDepthReduction {
		    /// <summary>
		    /// ColorDepthReduction.None - PixelFormat.Canconical is used
		    /// </summary>
		    None,
		    /// <summary>
		    /// ColorDepthReduction.Colored8bpp - change bitmap 
		    /// to PixelFormat.Format8bppIndexed using colors
		    /// </summary>
		    Colored8bpp,
		    /// <summary>
		    /// ColorDepthReduction.Grayscaled8bpp - change bitmap 
		    /// to PixelFormat.Format8bppIndexed using grayscales
		    /// </summary>
		    Grayscaled8bpp
		};
		
	    #endregion    
	    
		#region Private: Variables, Constructor, Printer's Event, additional methods
		/// <summary>
		/// The FormPrint instance cls is used to execute printing resp. saving.
		/// </summary>
		private static FormPrint cls;

		/// <summary>
		/// The bitmap bmp contains the copy of the control's graphics.
		/// </summary>
		/// Note: bmp cannot be local to start methods because of doc_PrintPage
		private Control ctrl;
		private Bitmap bmp;

		/// <summary>
		/// Parameters to be used to print or save
		/// </summary>
		private bool Printing;
		//	formatings, convertings
		private ImageFormat formatImage;
		private ColorDepthReduction format8bitPixel;
		//  additional parameters for printing
		private string PrinterName;
		private Margins PrinterMargins;
		private bool PrinterPortrait;
		//  additional parameter for saving
		private string SaveFileName;

		/// <summary>
		/// Constructor without parameter is not used
		/// </summary>
		private FormPrint()
		{
		}

		/// <summary>
		/// Constructor to set all parameters
		/// </summary>
		/// how to use parameters, take a look at description of Start() method
		private FormPrint(Control Ctrl, bool bPrint,
				//	formatings, convertings
				ImageFormat fImage, ColorDepthReduction f8bitPixel, 
				//  additional parameters for printing
				string sPrinterName, Margins aMargins, bool bPortrait,
				//  additional parameter for saving
				string sFileName)
		{
			//  call the bitmap in the propriate way
			//		changed 03/22/2007
			// 	NET 1.1 
			//	=>	GetBitmap in 1.1 Version 
			//		calls GetBitmapGdi
			//	NET 2.0
			//	=>	standard version
			//		GetBitmap in 2.0 Version 
			//	=>  RichTextBox or WebBrowser
			//		GetBitmapGdi
			ctrl = Ctrl;
			#if NET_V20
			if (Find_RTF_or_Web(Ctrl))
			    bmp = GetBitmapGdi(Ctrl);
			else
				bmp = GetBitmap(Ctrl);
        	#elif NET_V11        
        	bmp = GetBitmap(Ctrl);
        	#endif

			//  set all other parameters
			Printing = bPrint;
			//	formatings, convertings
			formatImage = fImage;
			format8bitPixel = f8bitPixel;
			//  additional parameters for printing
			PrinterName = sPrinterName;
			PrinterMargins = aMargins;
			PrinterPortrait = bPortrait;
			//  additional parameter for saving
			SaveFileName = sFileName;
		}

		/// <summary>
		/// doc_PrintPage connects the control's bitmap to the printer's graphics
		/// </summary>
		private void doc_PrintPage(System.Object sender,
								   System.Drawing.Printing.PrintPageEventArgs e)
		{
			e.Graphics.DrawImage(bmp, 0, 0);
		}

		/// <summary>
		/// GetThumbnail() compresses the bitmap if necessary
		/// </summary>
		/// <param name="newWidth">the (lower) width of the required bitmap</param>
		/// <param name="newHeight">the (lower) height of the required bitmap</param>
		private void GetThumbnail(int newWidth, int newHeight)
		{
			Bitmap Result = new Bitmap(newWidth, newHeight);
			Graphics g = Graphics.FromImage(Result);
			g.FillRectangle(new SolidBrush(Color.White), 0,0, Result.Width, Result.Height);

			float factor = Math.Max((float)bmp.Width /(float)newWidth,
									(float)bmp.Height/(float)newHeight);
			g.DrawImage(bmp, 0,0, (float)bmp.Width/factor, (float)bmp.Height/factor);
			g.Dispose();

			//  change the formprint bitmap
			bmp.Dispose();
			bmp = Result;

			return;
		}

		private bool Find_RTF_or_Web(Control ctrl) {
			if (ctrl.Controls.Count == 0)
				return (ctrl is RichTextBox) || (ctrl is WebBrowserBase);
			else {
				bool Result = false;
				foreach(Control ctl in ctrl.Controls) {
					Result = Find_RTF_or_Web(ctl);
					if (Result)
						break;
				}
				return Result;
			}
		}
		#endregion //  Private: Variables, Constructor, Printer's Event, additional methods

		#region Copy the control's graphics 
        #if NET_V20		
        //  NET 2.0 - Copy the control's graphics directly to the bitmap
		private Bitmap GetBitmap(Control Ctrl) {
			//  prepare the bitmap
			Graphics grCtrl = Ctrl.CreateGraphics();
			Bitmap Result = new Bitmap(Ctrl.Width, Ctrl.Height, grCtrl);
			//  copy the control's graphics – not available with NET 1.1
			Ctrl.DrawToBitmap( Result, new Rectangle(0, 0, Ctrl.Width, Ctrl.Height));
			//  that's all work for the bmp using NET 2.0 instructions
			return Result;
		}

        #elif NET_V11        
		//  NET 1.1 - copy the control's graphics using Windows GDI
		private Bitmap GetBitmap(Control Ctrl) {
			return GetBitmapGdi(Ctrl);
		}
        #endif
		
		[DllImport("gdi32.dll")]
		private static extern long BitBlt(
			IntPtr hdcDest,
			int xDest,
			int yDest,
			int nWidth,
			int nHeight,
			IntPtr hdcSource,
			int xSrc,
			int ySrc,
			Int32 dwRop);

		const int SRCCOPY=13369376;

		private Bitmap GetBitmapGdi(Control Ctrl) {

			//  create the source graphics and the required bitmap
			Graphics grCtrl = Graphics.FromHwnd(Ctrl.Handle);
			Bitmap Result = new Bitmap(Ctrl.Width, Ctrl.Height, grCtrl);

			//  we need handles to copy graphics
			IntPtr   hdcCtrl = grCtrl.GetHdc();
			Graphics grDest  = Graphics.FromImage(Result);
			IntPtr   hdcDest = grDest.GetHdc();

			//	copy the image from source hdcCtrl to destination hdcDest
			//  using BitBlt (bit block transfer) of Windows GDI
			//  note:	a form is normally printed without title or menu bar;
			//      	therefore, one has to move the graphics
			int SourceX, SourceY;
			if (Ctrl is Form) {
				SourceX = Ctrl.ClientSize.Width  - Ctrl.Width  + 4;
				SourceY = Ctrl.ClientSize.Height - Ctrl.Height + 4;
			} else {
				SourceX = 0;
				SourceY = 0;
			}
			BitBlt(hdcDest, 0, 0, Ctrl.Width, Ctrl.Height, hdcCtrl, SourceX, SourceY, SRCCOPY);

			//	free the used objects
			grCtrl.ReleaseHdc(hdcCtrl);
			grDest.ReleaseHdc(hdcDest);

			//  that's all work for the bmp using NET 1.1 instructions
			return Result;
		}
        
		#endregion

		#region Converts bitmap format to 8bpp
		/// this region is adapted from robert.schmitz.stolberg@googlemail.com
		/// take a look at http://www.mycsharp.de/wbb2/thread.php?threadid=29667

		Color ConvertColorTo8Bpp(Color col)
		{
			int r = col.R - col.R % 51;
			int g = col.G - col.G % 51;
			int b = col.B - col.B % 51;

			return Color.FromArgb(r, b, g);
		}

		Color ConvertColorTo8Bpp(Color col, int correction)
		{
			int r = (col.R + correction) - (col.R + correction) % 51;
			int g = (col.G + correction) - (col.G + correction) % 51;
			int b = (col.B + correction) - (col.B + correction) % 51;

			return Color.FromArgb(r, b, g);
		}

		private void ConvertBitmapTo8Bpp()
		{
			ConvertBitmapTo8Bpp(0);
		}

		public void ConvertBitmapTo8Bpp(int correction)
		{
			//  this method is the only one that sometimes needs a long time;
			//  therefore Juergen Thomas added a progress form as standard,
			//  but shows it only for larger bitmaps
			using(ProgressForm progress = new ProgressForm(ctrl)) {
				if (bmp.Width * bmp.Height >= 40000)
					progress.Show();
				//  variables used instead of timer
				DateTime now;
				TimeSpan diff;

				Bitmap Result = new Bitmap( bmp.Width, bmp.Height,
					PixelFormat.Format8bppIndexed );

				BitmapData ResultData
					= Result.LockBits(
						new Rectangle(0, 0, bmp.Width, bmp.Height),
						ImageLockMode.ReadWrite,
						PixelFormat.Format8bppIndexed);

				int tmpStride = ResultData.Stride;
				IntPtr ptr = ResultData.Scan0;

				Hashtable ColorReduction = new Hashtable();
				byte offset = 39;
				for (int r = 0; r < 256; r += 51)
					for (int g = 0; g < 256; g += 51)
						for (int b = 0; b < 256; b += 51)
						{
							ColorReduction[Color.FromArgb(r, b, g)] = ++offset;
						}

				int tmpBytes = bmp.Height * tmpStride;
				byte[] tmpBitmapData = new byte[tmpBytes];

				now = DateTime.Now;
				for (int x = 0; x < bmp.Width; x++)
				{
					//  check if progress should be actualized (after 600 msec)
					diff = DateTime.Now - now;
					if (progress.Visible && (diff.Ticks > 6000000)) {
						progress.Progressing(x);
						now = DateTime.Now;
					}

					for (int y = 0; y < bmp.Height; y++)
					{
						int offset2 = (y * tmpStride) + x;
						//  get a suitable 8 bit color to the current 24 bit pixel
						Color col = ConvertColorTo8Bpp(bmp.GetPixel(x,y), correction);
						//  set this color as color resp. grayscale
						if (format8bitPixel == ColorDepthReduction.Colored8bpp)
							tmpBitmapData[offset2]
								= (byte)( col.R*0.3 + col.G*0.59 + col.B*0.11 );
						else
							tmpBitmapData[offset2]
								= (byte)ColorReduction[col];
					}
				}

				System.Runtime.InteropServices.Marshal.Copy(tmpBitmapData, 0, ptr, tmpBytes);

				Result.UnlockBits(ResultData);

				progress.Hide();

				//  change the formprint bitmap
				bmp.Dispose();
				bmp = Result;
			}
		}

		#endregion  //  Converts bitmap format to 8bpp

		#region Additional ProgressForm.
		/// <summary>
		/// ProgressForm shows progressing during ConvertTo8bppFormat for larger controls
		/// </summary>
		/// Note: this is a minimalized version created by a designer
		private class ProgressForm : System.Windows.Forms.Form
		{
			/// <summary>
			/// Required designer variables.
			/// </summary>
			private System.ComponentModel.Container components = null;
			/// label that shows the progressing
			private System.Windows.Forms.Label showValue;
			/// maximum value = 100%
			private int maximum = 100;

			/// standard constructor is not used
			private ProgressForm()
			{
			}

			/// <summary>
			/// Constructor using control's position and width
			/// </summary>
			public ProgressForm(Control Ctrl)
			{
				//
				//  used by Windows form designer
				//
				InitializeComponent(Ctrl);
				maximum = Ctrl.Width;
				//
			}

			/// <summary>
			/// clean up resources
			/// </summary>
			protected override void Dispose(bool disposing)
			{
				if (disposing)
				{
					if (components != null)
					{
						components.Dispose();
					}
				}
				base.Dispose(disposing);
			}

			#region Code developed by Windows form designer
			/// <summary>
			/// Necessary method to support the designer
			/// </summary>
			private void InitializeComponent(Control Ctrl)
			{
				this.showValue = new System.Windows.Forms.Label();
				this.SuspendLayout();
				//
				// showValue
				//
				this.showValue.Dock = System.Windows.Forms.DockStyle.Fill;
				this.showValue.Location = new System.Drawing.Point(0, 0);
				this.showValue.Name = "showValue";
				this.showValue.Size = new System.Drawing.Size(140, 22);
				this.showValue.TabIndex = 0;
				this.showValue.Text = "0 %";
				this.showValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
				//
				// ProgressForm
				//
				this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
				this.ClientSize = new System.Drawing.Size(140, 22);
				this.ControlBox = false;
				this.Controls.Add(this.showValue);
				this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;

				//
				//  check manual position
				Point pt = Ctrl.PointToScreen(new Point(0,0));
				this.Location = new System.Drawing.Point(
					pt.X + (Ctrl.Width  - this.Width )/2,
					pt.Y + (Ctrl.Height - this.Height)/3 );
				this.MaximizeBox = false;
				this.MinimizeBox = false;
				this.Name = "ProgressForm";
				this.ShowInTaskbar = false;
				this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
				this.TopMost = true;
				this.ResumeLayout(false);
			}
			#endregion

			public void Progressing(int Position) {
				showValue.Text = String.Format("Convert Bitmap {0} %", (int)((Position * 100)/maximum));
				showValue.Refresh();
			}
		}
		#endregion

		#region Private: Start, StartPrinting, StartSaving
		/// <summary>
		/// Start() combines Print() and Save() to do all of the work
		/// </summary>
		/// <param name="Ctrl">Control (form or other) that is to be printed or saved</param>
		/// <param name="bPrint">Desides whether to print (true) or save (false)</param>
		/// <param name="fImage">System.Drawing.Imaging.ImageFormat by which the bitmap
		/// has to be saved (printing uses png format as standard)</param>
		/// <param name="f8bitPixel">decides how to convert bitmap to 8 bit ColorDepth</param>
		/// <param name="sPrinterName">the name of the printer that is to be used</param>
		/// <param name="aMargins">the page margins that are to be used</param>
		/// <param name="bPortrait">Printing as portrait (true) or landscape (false)</param>
		/// <param name="sFileName">String with the complete filename to which the bitmap
		/// is to be saved (used only if bPrint == true)</param>
		private static void Start(Control Ctrl, bool bPrint,
				//	formatings
				ImageFormat fImage, ColorDepthReduction f8bitPixel, 
				//  additional parameters for printing
				string sPrinterName, Margins aMargins, bool bPortrait,
				//  additional parameter for saving
				string sFileName
			)
		{

			//  add required standard parameters if necessary
			if (fImage == null)
				fImage = ImageFormat.Png;
			if (bPrint) {
				if (sPrinterName == null)
					sPrinterName = "";
				//  orientation and margins are set to the printer standard automatically
			} else {
				#region NET 2.0 a shorter way to check string.IsNullOrEmpty(sFileName)
				//  NET 2.0 - you can use next 'if'-query as follows:
				//      if (String.IsNullOrEmpty(sFileName))
				//      pay attention: String.IsNullOrEmpty() is new in NET 2.0
				//  NET 1.1 - you must use next 'if'-query as follows:
				//      if ((sFileName == null) || (sFileName == ""))
				#endregion
				//  no filename => user's folder, control's name, imageformat as extension
				if ((sFileName == null) || (sFileName == "")) {
					sFileName = System.IO.Path.Combine(
						Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
						Ctrl.Name + "." + fImage.ToString() );
				}
			}

			//  check possible errors, otherwise execute
			if (Ctrl == null) 	//  reserved for further checks
			{
				if (Ctrl == null)
					MessageBox.Show("Control is not defined", "FormPrint.Start");
				else
					MessageBox.Show("Any other error", "FormPrint.Start");
			} else {
				//  create the private formprint class cls
				cls = new FormPrint(Ctrl, bPrint, fImage, f8bitPixel, 
					sPrinterName, aMargins,  bPortrait, sFileName);
				//  execute the required printing resp. saving method
				try {
					if (bPrint) {
						cls.StartPrinting();
					} else {
						cls.StartSaving();
					}
				} finally {
					//  free resources
					cls.bmp.Dispose();
					cls.bmp = null;
					//  in some situations, the next command is useful
					Ctrl.Refresh();
				}
			}
		}

		/// <summary>
		/// StartPrinting() executes to print the control's bitmap
		/// </summary>
		private void StartPrinting() {
			//  create a document; the event handler PrintPage
			//  will connect it with the bitmap
			using(PrintDocument doc = new PrintDocument()){
				try {
					//  use the required printer
					if (this.PrinterName != "")
						doc.PrinterSettings.PrinterName = this.PrinterName;
					bool bPrinterValid = doc.PrinterSettings.IsValid;

					//  use the required page settings
					doc.DefaultPageSettings.Landscape = !PrinterPortrait;
					if (PrinterMargins != null)
						doc.DefaultPageSettings.Margins = PrinterMargins;
					doc.PrintPage += new PrintPageEventHandler(doc_PrintPage);

					//  if margins are set, then check if bitmap has to be compressed
					if (PrinterMargins != null) {
						doc.OriginAtMargins = true;
						PageSettings stg = doc.DefaultPageSettings;

						//  compare PaperSize and bmp size:
						//  maximum horizontal and vertical
						float maxX = ( stg.PaperSize.Width
									 - stg.Margins.Left
									 - stg.Margins.Right );
						float maxY = ( stg.PaperSize.Height
									 - stg.Margins.Top
									 - stg.Margins.Bottom );
						float bmpX = maxX * 96 / 100;
						float bmpY = maxY * 96 / 100;
						//	landscape => exchange bmpX and bmpY
						if (stg.Landscape) {
							{
							float tmp = bmpX;
							bmpX = bmpY;
							bmpY = tmp;
							}
						}
						//  check if bmp size is larger than PaperSize
						if ( (bmp.Width > bmpX) || (bmp.Height > bmpY) ) {
							GetThumbnail((int)bmpX, (int)bmpY);
						}
					}
					//  convert to 8-bit-pixel or grayscale to avoid printer problems
					if (format8bitPixel != ColorDepthReduction.None)    {
						ConvertBitmapTo8Bpp(15);
					}

					//  now print
					doc.DocumentName = ctrl.Name;
					doc.Print();
				} catch (InvalidPrinterException e) {
					MessageBox.Show(e.Message, "FormPrint.StartPrinting - InvalidPrinterException");
				} catch (Exception e) {
					MessageBox.Show(e.Message, "FormPrint.StartPrinting - Standard-Exception");
				} finally {
					doc.PrintPage -= new PrintPageEventHandler(doc_PrintPage);
				}
			}
		}

		/// <summary>
		/// StartSaving() executes to save the control's bitmap
		/// </summary>
		private void StartSaving() {
			try {
				//  save the bitmap into the required file
				bmp.Save(SaveFileName, formatImage);
			} catch (System.IO.IOException e) {
				MessageBox.Show("Bitmap could not be saved:\n" +
								SaveFileName + "\n" +
								e.Message,
								"FormPrint.StartSaving - IOException");
			} catch (Exception e) {
				MessageBox.Show(e.Message, "FormPrint.StartSaving - Standard-Exception");
			}
		}

		#endregion  //  Private: Start, StartPrinting, StartSaving

		#region Public: printing methods
		/// <summary>
		/// Print: using all possible parameters
		/// </summary>
		/// <param name="Ctrl">Control (form or other) that is to be printed</param>
		/// <param name="f8bitPixel">decides how to convert bitmap to 8 bit ColorDepth</param>
		/// <param name="sPrinterName">the name of the printer that has to be used</param>
		/// <param name="aMargins">the page margins that are to be used</param>
		/// <param name="bPortrait">Printing as portrait (true) or landscape (false)</param>
		public static void Print(Control Ctrl, ColorDepthReduction f8bitPixel, 
				string sPrinterName, Margins aMargins, bool bPortrait) {
			Start(Ctrl, true, null, f8bitPixel, 
		          sPrinterName, aMargins, bPortrait, null);
			return;
		}

		/// <summary>
		/// Print: png format with variable printer and settings
		/// </summary>
		/// <param name="Ctrl">Control (form or other) that is to be printed</param>
		/// <param name="sPrinterName">the name of the printer that has to be used</param>
		/// <param name="aMargins">the page margins that are to be used</param>
		/// <param name="bPortrait">Printing as portrait (true) or landscape (false)</param>
		public static void Print(Control Ctrl,
				string sPrinterName, Margins aMargins, bool bPortrait) {
			Start(Ctrl, true, null, ColorDepthReduction.None, 
		          sPrinterName, aMargins, bPortrait, null);
			return;
		}

		/// <summary>
		/// Print: 8 bit pixels greyscaling with variable printer
		/// </summary>
		/// <param name="Ctrl">Control (form or other) that is to be printed</param>
		/// <param name="f8bitPixel">decides how to convert bitmap to 8 bit ColorDepth</param>
		/// <param name="sPrinterName">the name of the printer that has to be used
		/// including its margins</param>
		/// <param name="bPortrait">Printing as portrait (true) or landscape (false)</param>
		public static void Print(Control Ctrl, ColorDepthReduction f8bitPixel, 
				string sPrinterName, bool bPortrait) {
			Start(Ctrl, true, null, f8bitPixel, 
		          sPrinterName, null, bPortrait, null);
			return;
		}

		/// <summary>
		/// Print: png format with variable printer
		/// </summary>
		/// <param name="Ctrl">Control (form or other) that is to be printed</param>
		/// <param name="sPrinterName">the name of the printer that has to be used
		/// including its margins</param>
		/// <param name="bPortrait">Printing as portrait (true) or landscape (false)</param>
		public static void Print(Control Ctrl, string sPrinterName, bool bPortrait) {
			Start(Ctrl, true, null, ColorDepthReduction.None, 
		          sPrinterName, null, bPortrait, null);
			return;
		}

		/// <summary>
		/// Print: standard printer with variable parameters
		/// </summary>
		/// <param name="Ctrl">Control (form or other) that is to be printed</param>
		/// <param name="f8bitPixel">decides how to convert bitmap to 8 bit ColorDepth</param>
		/// <param name="aMargins">the page margins that are to be used</param>
		/// <param name="bPortrait">Printing as portrait (true) or landscape (false)</param>
		public static void Print(Control Ctrl, ColorDepthReduction f8bitPixel, 
				Margins aMargins, bool bPortrait) {
			Start(Ctrl, true, null, f8bitPixel, null, aMargins, bPortrait, null);
			return;
		}

		/// <summary>
		/// Print: 8 bit pixels greyscaling as landscape
		/// </summary>
		/// <param name="Ctrl">Control (form or other) that is to be printed</param>
		/// <param name="f8bitPixel">decides how to convert bitmap to 8 bit ColorDepth</param>
		/// <param name="bPortrait">Printing as portrait (true) or landscape (false)</param>
		public static void Print(Control Ctrl, ColorDepthReduction f8bitPixel, 
				bool bPortrait) {
			Start(Ctrl, true, null, f8bitPixel, null, null, bPortrait, null);
			return;
		}

		/// <summary>
		/// Print: 8 bit pixels as portrait with variable margins
		/// </summary>
		/// <param name="Ctrl">Control (form or other) that is to be printed</param>
		/// <param name="f8bitPixel">decides how to convert bitmap to 8 bit ColorDepth</param>
		/// <param name="aMargins">the page margins that are to be used</param>
		public static void Print(Control Ctrl, ColorDepthReduction f8bitPixel, 
				Margins aMargins) {
			Start(Ctrl, true, null, f8bitPixel, null, aMargins, true, null);
			return;
		}

		/// <summary>
		/// Print: png format with variable page settings
		/// </summary>
		/// <param name="Ctrl">Control (form or other) that is to be printed</param>
		/// <param name="aMargins">the page margins that are to be used</param>
		/// <param name="bPortrait">Printing as portrait (true) or landscape (false)</param>
		public static void Print(Control Ctrl, Margins aMargins, bool bPortrait) {
			Start(Ctrl, true, null, ColorDepthReduction.None, 
		          null, aMargins, bPortrait, null);
			return;
		}

		/// <summary>
		/// Print: standards with variable margins
		/// </summary>
		/// <param name="Ctrl">Control (form or other) that is to be printed</param>
		/// <param name="aMargins">the page margins that are to be used</param>
		public static void Print(Control Ctrl, Margins aMargins) {
			Start(Ctrl, true, null, ColorDepthReduction.None, null, aMargins, true, null);
			return;
		}

		/// <summary>
		/// Print: standards with variable orientation
		/// </summary>
		/// <param name="Ctrl">Control (form or other) that is to be printed</param>
		/// <param name="bPortrait">Printing as portrait (true) or landscape (false)</param>
		public static void Print(Control Ctrl, bool bPortrait) {
			Start(Ctrl, true, null, ColorDepthReduction.None, null, null, bPortrait, null);
			return;
		}

		/// <summary>
		/// Print: standard settings only to print the actual form
		/// </summary>
		/// <param name="Ctrl">Control (form or other) that is to be printed</param>
		public static void Print(Control Ctrl) {
			Start(Ctrl, true, null, ColorDepthReduction.None, null, null, true, null);
			return;
		}

		#endregion  //  Public: printing methods

		#region Public: Saving methods
		/// <summary>
		/// Save: setting ImageFormat and filename
		/// </summary>
		/// <param name="Ctrl">Control (form or other) that is to be saved</param>
		/// <param name="fImage">System.Drawing.Imaging.ImageFormat by which the bitmap
		/// has to be saved</param>
		/// <param name="sFileName">String with the complete filename to which the bitmap
		/// has to be saved</param>
		public static void Save(Control Ctrl, ImageFormat fImage, string sFileName)
		{
			Start(Ctrl, false, fImage, ColorDepthReduction.None, 
		          null, null, false, sFileName);
			return;
		}
		/// <summary>
		/// Save: setting ImageFormat and using standard filename
		/// </summary>
		/// <param name="Ctrl">Control (form or other) that is to be saved</param>
		/// <param name="fImage">System.Drawing.Imaging.ImageFormat by which the bitmap
		/// has to be saved</param>
		public static void Save(Control Ctrl, ImageFormat fImage)
		{
			Start(Ctrl, false, fImage, ColorDepthReduction.None, null, null, false, null);
			return;
		}

		/// <summary>
		/// Save: setting filename and using png format
		/// </summary>
		/// <param name="Ctrl">Control (form or other) that is to be saved</param>
		/// <param name="sFileName">String with the complete filename to which the bitmap
		/// has to be saved</param>
		public static void Save(Control Ctrl, string sFileName)
		{
			Start(Ctrl, false, null, ColorDepthReduction.None, null, null, false, sFileName);
			return;
		}

		/// <summary>
		/// Save: using png format and standard filename
		/// </summary>
		/// <param name="Ctrl">Control (form or other) that is to be saved</param>
		public static void Save(Control Ctrl)
		{
			Start(Ctrl, false, null, ColorDepthReduction.None, null, null, false, null);
			return;
		}
		#endregion  //  Public: Saving methods

	}
}
