﻿using iText.Kernel.Pdf;
using iText.Kernel.Utils;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;



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
			return Task.Run(() =>
			{
				int tmp = info.GroupIndex = 0;
				info.GroupIndex = 0;
				PdfReader reader = new PdfReader(info.GetFile(0));
				PdfDocument doc = new PdfDocument(reader);
				if (!Directory.Exists(GetPDFPath(info))) Directory.CreateDirectory(GetPDFPath(info));
				for (int i = 1; i <= Min(doc.GetNumberOfPages(), values.Count - 1); i++)
				{
					string name = Path.GetFileNameWithoutExtension(info.AttachmentPath) + "-" + values[i][col].ToString() + "-" + i;
					PdfWriter writer = new PdfWriter(GetPDFPath(info) + "/" + name + ".pdf");
					PdfDocument pdfDoc = new PdfDocument(writer);
					PdfPage page = doc.GetPage(i).CopyTo(pdfDoc);
					pdfDoc.AddPage(page);
					pdfDoc.Close();
				}
				info.GroupIndex = tmp;
			}
			);
		}
	}
}
