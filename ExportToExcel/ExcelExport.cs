using System;
using System.Data;
using System.Drawing;
using System.IO;

using System.Collections.Generic;
using System.ComponentModel;
using OfficeOpenXml;

namespace ExcelExport
{
    public class ExcelExporter
    {
        public ExcelExporter()
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
        }

        public void ExportDataTableToExcel(DataTable table, string filePath)
        {
            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Pendulum Data");
                ws.Cells["A1"].LoadFromDataTable(table, true);
                pck.SaveAs(new FileInfo(filePath));
            }
        }

        public void ExportPointsToExcel(List<(double Time, double Angle)> points, string filePath, Bitmap plotImage)
        {
            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Pendulum Points");
                ws.Cells[1, 1].Value = "Time";
                ws.Cells[1, 2].Value = "Angle";

                for (int i = 0; i < points.Count; i++)
                {
                    ws.Cells[i + 2, 1].Value = points[i].Time;
                    ws.Cells[i + 2, 2].Value = points[i].Angle;
                }

                if (plotImage != null)
                {
                    var imagePath = Path.Combine(Path.GetTempPath(), "plot.png");
                    plotImage.Save(imagePath, System.Drawing.Imaging.ImageFormat.Png);
                    var image = ws.Drawings.AddPicture("Plot", new FileInfo(imagePath));
                    image.SetPosition(points.Count + 3, 0, 0, 0);
                }

                pck.SaveAs(new FileInfo(filePath));
            }
        }
    }
}
