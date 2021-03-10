using System;
using System.Collections.Generic;
using System.IO;

using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace PidgeotMailMVVM.Lib
{
	public class PDFProcess
	{
		public static void SplitPDF (AttachmentInfo info, IList<IList<Object>> values, int row)
		{
			// Open the file
			PdfDocument inputDocument = PdfReader.Open(info.AttachmentPath, PdfDocumentOpenMode.ReadOnly);

			string name = Path.GetFileNameWithoutExtension(info.AttachmentPath);
			for (int idx = 0; idx < inputDocument.PageCount; idx++)
			{
				// Create new document
				PdfDocument outputDocument = new PdfDocument();
				outputDocument.Version = inputDocument.Version;
				outputDocument.Info.Title = values[0][row].ToString();
				outputDocument.Info.Creator = inputDocument.Info.Creator;

				// Add the page and save it
				outputDocument.AddPage(inputDocument.Pages[idx]);
				outputDocument.Save("/temp/" + name + "/" + outputDocument.Info.Title + ".pdf");
			}
		}
	}
}
