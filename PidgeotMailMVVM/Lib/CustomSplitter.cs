﻿using iText.Kernel.Pdf;
using iText.Kernel.Utils;

class CustomSplitter : PdfSplitter
{
    private int _order;
    private readonly string _destinationFolder;

    public CustomSplitter(PdfDocument pdfDocument, string destinationFolder) : base(pdfDocument)
    {
        _destinationFolder = destinationFolder;
        _order = 0;
    }

    protected override PdfWriter GetNextPdfWriter(PageRange documentPageRange)
    {
        return new PdfWriter(_destinationFolder + _order++ + ".pdf");
    }
}