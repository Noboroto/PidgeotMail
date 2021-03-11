using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace PidgeotMailMVVM.Lib
{
	public class PDFProcess
	{
		public static string GetPDFPath (AttachmentInfo info)
		{
			return Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + "/temp/ " + Path.GetFileNameWithoutExtension(info.AttachmentPath));
		}

		private static int Min (int a, int b)
		{
			return (a < b) ? a : b;
		}

		public static void SplitPDF (AttachmentInfo info, IList<IList<Object>> values, int col)
		{
			// Open the file
			PdfDocument inputDocument = PdfReader.Open(info.AttachmentPath, PdfDocumentOpenMode.Import);
			if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "/temp")) Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + "/temp", true);
			Directory.CreateDirectory(GetPDFPath(info));
			for (int idx = 0; idx < Min(inputDocument.PageCount, values.Count - 1); idx++)
			{
				// Create new document
				PdfDocument outputDocument = new PdfDocument();
				outputDocument.Version = inputDocument.Version;
				outputDocument.Info.Title = Path.GetFileNameWithoutExtension(info.AttachmentPath) + "-" + values[idx + 1][col].ToString();
				outputDocument.Info.Creator = inputDocument.Info.Creator;

				// Add the page and save it
				outputDocument.AddPage(inputDocument.Pages[idx]);
				outputDocument.Save(GetPDFPath(info) + "/" + outputDocument.Info.Title + ".pdf");
			}
		}
	}
}
