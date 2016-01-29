using GalaSoft.MvvmLight;
using InkCanvas.Model;
using System.Windows.Ink;
using System.Windows;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.Diagnostics;
using GalaSoft.MvvmLight.Command;
using System.Linq;
using System.Windows.Input;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Data;
using System;

namespace InkCanvas.ViewModel
{

    public class TimeSpanToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return 1 - (double)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return 1 - (double)value;
        }
    }

    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    { 
    public MainViewModel(IDataService dataService)
    {
        pDic = new Dictionary<XGraphics, List<Point[]>>();
        pList = new List<Point[]>();
        document = new PdfDocument();
        pointL = new List<List<Point[]>>();
        this.count = 0;
    }

    public Dictionary<XGraphics, List<Point[]>> pDic { get; set; }
    public List<Point[]> pList { get; set; }
    public List<List<Point[]>> pointL { get; set; }
    public PdfDocument document { get; set; }
    public int count { get; set; }
    private RelayCommand nextPDF;
    public RelayCommand NextPDF
    {
        get
        {
            return nextPDF ?? (nextPDF = new RelayCommand(addNextPDF));
        }
    }
    private void addNextPDF()
    {

        PdfPage page = new PdfPage();
        page.Size = PageSize.A4;

        double h = SystemParameters.PrimaryScreenHeight;
        double w = SystemParameters.PrimaryScreenWidth;
        //XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);
        var rateW = page.Width / w;
        var rateH = page.Height / h;

        var strokes = CanvasStroke.ToList();
        List<Point[]> pl = new List<Point[]>();
        foreach (Stroke stroke in strokes)
        {

            StylusPointCollection points = stroke.StylusPoints;
            StylusPointCollection newPoints = new StylusPointCollection();
            var pointList = points.ToList();
            foreach (StylusPoint pt in pointList)
            {

                StylusPoint newPt = new StylusPoint(pt.X * rateW, pt.Y * rateH);
                newPoints.Add(newPt);
            }
            Point[] p = (Point[])newPoints;
            pList.Add(p);
            pl.Add(p);
            CanvasStroke.Remove(stroke);
        }

        pointL.Add(pl);

    }


    private RelayCommand savePDF;
    public RelayCommand SavePDF
    {
        get
        {
            return savePDF ?? (savePDF = new RelayCommand(createPDF));
        }
    }

    private void createPDF()
    {
            
        XPen pen = new XPen(XColor.FromArgb(Convert.ToByte(alpha),Convert.ToByte(r),Convert.ToByte(g),Convert.ToByte(b)));
        for (int i = 0; i < pointL.Count; i++)
        {
            List<Point[]> p1 = pointL[i];

            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
                foreach (Point[] p in p1)
                {
                    if (p.Length > 1)
                    {
                        gfx.DrawCurve(pen, p);
                    }
                }
            this.count = pointL.Count;

        }
            if (document.PageCount > 0)
            {
                // Save the document...
                const string filename = "XXXXprint.pdf";
                document.Save(filename);
                // ...and start a viewer.
                Process.Start(filename);
            }
    }




    private StrokeCollection canvasStroke;
    public StrokeCollection CanvasStroke
    {
        get
        {
            return canvasStroke ?? (canvasStroke = new StrokeCollection());
        }
        set
        {
            canvasStroke = value;
            RaisePropertyChanged("CanvasStroke");
        }
    }

        RelayCommand<string> changeColor;
        public RelayCommand<string> ChangeColor
        {
            get
            {
                return changeColor ?? (changeColor = new RelayCommand<string>(selectedColor));
            }
        }
        
        private void selectedColor(string strokeColor)
        {
            if(strokeColor == "red")
            {
                r = 255; g = b = 0;
            }
            if(strokeColor == "black")
            {
                r = g = b = 0;
            }
            if(strokeColor == "yellow")
            {
                r = g = 255; b = 0;
            }
            if (strokeColor == "green")
            {
                r = b = 0; g = 255;
            }
            if (strokeColor == "pink")
            {
                r = b = 255; g = 155;
            }
            if (strokeColor == "white")
            {
                r = g = b = 255;
            }
            if (strokeColor == "rosybrown")
            {
                r = 188; g = b = 143;
            }
            if (strokeColor == "cyan")
            {
                r = 0; g = b = 255;
            }
            if (strokeColor == "magenta")
            {
                r = b = 255; g = 0;
            }
            if (strokeColor == "silver")
            {
                r = g = b = 192;
            }
            if (strokeColor == "khaki")
            {
                r = 255; g = 205; b = 0;
            }
            if (strokeColor == "gray")
            {
                r = g = b = 128;
            }
            if (strokeColor == "purple")
            {
                r = 160; g = 32; b = 240;
            }
            if (strokeColor == "orange")
            {
                r = 255; g = 165; b = 0;
            }
            if (strokeColor == "blue")
            {
                r = g = 0; b = 255;
            }
            printlineColor();
        }

        private DrawingAttributes incanvasDrawingAttributes;
        public DrawingAttributes IncanvasDrawingAttributes
        {
            get
            {
                return incanvasDrawingAttributes ?? (incanvasDrawingAttributes = new DrawingAttributes());
            }
            set
            {
                incanvasDrawingAttributes = value;
                RaisePropertyChanged("IncanvasDrawingAttributes");
            }
        }

        RelayCommand setOpenOrNot;
        public RelayCommand SetOpenOrNot
        {
            get
            {
                return setOpenOrNot ?? (setOpenOrNot = new RelayCommand(openOrNot));
            }
        }

        private void openOrNot()
        {
            Isopen = true;
        }

        private bool isopen;
        public bool Isopen
        {
            set
            {
                isopen = value;
                RaisePropertyChanged("Isopen");
            }
            get
            {
                return isopen;
            }
        }
        private double width = 6.0;
        public double Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
                IncanvasDrawingAttributes.Width = width;
                incanvasDrawingAttributes.Height = width;
                RaisePropertyChanged("Width");
            }
        }

        private int alpha = 127;
        private int r = 0;
        private int g = 0;
        private int b = 0;
        private double opacity = 1.0;
        public double Opacity
        {
            get
            {
                return opacity;
            }

            set
            {
                opacity = value;
                alpha = (int)(255 * opacity);
                printlineColor();
                RaisePropertyChanged("Opacity");
            }

        }

        private void printlineColor()
        {
            incanvasDrawingAttributes.Color = new SolidColorBrush(Color.FromArgb(Convert.ToByte(alpha), (byte)r, (byte)g, (byte)b)).Color;
        }
        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}