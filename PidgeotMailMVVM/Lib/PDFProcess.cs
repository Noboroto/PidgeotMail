using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace PidgeotMail.Lib
{
	public class PDFProcess
	{
		public static string GetPDFPath(AttachmentInfo info)
		{
			return Path.Combine(ViewModel.MainViewModel.TempFolder, Path.GetFileNameWithoutExtension(info.AttachmentPath));
		}

		private static int Min(int a, int b)
		{
			return (a < b) ? a : b;
		}

		public static Task SplitPDF(AttachmentInfo info, IList<IList<Object>> values, int col)
		{
			// Open the file
			return Task.Run(() =>
			{
				PdfDocument inputDocument = PdfReader.Open(info.AttachmentPath, PdfDocumentOpenMode.Import);
				Directory.CreateDirectory(GetPDFPath(info));
				string s = GetPDFPath(info);
				for (int idx = 0; idx < Min(inputDocument.PageCount, values.Count - 1); idx++)
				{
					// Create new document
					PdfDocument outputDocument = new PdfDocument();
					outputDocument.Version = inputDocument.Version;
					outputDocument.Info.Title = Path.GetFileNameWithoutExtension(info.AttachmentPath) + "-" + values[idx + 1][col].ToString() + "-" + idx.ToString();
					outputDocument.Info.Creator = inputDocument.Info.Creator;

					// Add the page and save it
					outputDocument.AddPage(inputDocument.Pages[idx]);
					outputDocument.Save(GetPDFPath(info) + "/" + outputDocument.Info.Title + ".pdf");
				}
			}
			);
		}
	}
}
