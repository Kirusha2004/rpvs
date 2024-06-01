using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection.Metadata;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using static System.Net.Mime.MediaTypeNames;
using A = DocumentFormat.OpenXml.Drawing;
using Document = DocumentFormat.OpenXml.Wordprocessing.Document;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;

namespace WordExport
{
    public class WordExporter
    {
        public void ExportPointsToWord(List<(double Time, double Angle)> points, string filePath, Bitmap plotImage)
        {
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = wordDoc.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = new Body();
                Table wordTable = new Table();

                // Создание свойств таблицы
                TableProperties tblProp = new TableProperties(
                    new TableBorders(
                        new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                        new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                        new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                        new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                        new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                        new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 }
                    )
                );
                wordTable.AppendChild(tblProp);

                // Добавление заголовка таблицы
                TableRow headerRow = new TableRow();
                headerRow.Append(new TableCell(new Paragraph(new Run(new Text("Time")))));
                headerRow.Append(new TableCell(new Paragraph(new Run(new Text("Angle")))));
                wordTable.Append(headerRow);

                // Добавление строк данных
                foreach (var point in points)
                {
                    TableRow dataRow = new TableRow();
                    dataRow.Append(new TableCell(new Paragraph(new Run(new Text(point.Time.ToString())))));
                    dataRow.Append(new TableCell(new Paragraph(new Run(new Text(point.Angle.ToString())))));
                    wordTable.Append(dataRow);
                }

                body.Append(wordTable);
                mainPart.Document.Append(body);

                // Добавление изображения графика
                AddImageToBody(wordDoc, mainPart, plotImage);
            }
        }

        private void AddImageToBody(WordprocessingDocument wordDoc, MainDocumentPart mainPart, Bitmap image)
        {
            ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Jpeg);

            using (var stream = new MemoryStream())
            {
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                stream.Position = 0;
                imagePart.FeedData(stream);
            }

            AddImageToBody(wordDoc, mainPart.GetIdOfPart(imagePart));
        }

        private void AddImageToBody(WordprocessingDocument wordDoc, string relationshipId)
        {
            // Устанавливаем размеры изображения (6 дюймов по ширине и 4 дюйма по высоте)
            long cx = 6 * 914400; // 6 дюймов в EMUs
            long cy = 4 * 914400; // 4 дюйма в EMUs

            var element =
                new Drawing(
                    new DW.Inline(
                        new DW.Extent() { Cx = cx, Cy = cy },
                        new DW.EffectExtent()
                        {
                            LeftEdge = 0L,
                            TopEdge = 0L,
                            RightEdge = 0L,
                            BottomEdge = 0L
                        },
                        new DW.DocProperties()
                        {
                            Id = (UInt32Value)1U,
                            Name = "Chart Image"
                        },
                        new DW.NonVisualGraphicFrameDrawingProperties(
                            new A.GraphicFrameLocks() { NoChangeAspect = true }),
                        new A.Graphic(
                            new A.GraphicData(
                                new PIC.Picture(
                                    new PIC.NonVisualPictureProperties(
                                        new PIC.NonVisualDrawingProperties()
                                        {
                                            Id = (UInt32Value)0U,
                                            Name = "New Bitmap Image.jpg"
                                        },
                                        new PIC.NonVisualPictureDrawingProperties()),
                                    new PIC.BlipFill(
                                        new A.Blip()
                                        {
                                            Embed = relationshipId,
                                            CompressionState = A.BlipCompressionValues.Print
                                        },
                                        new A.Stretch(
                                            new A.FillRectangle())),
                                    new PIC.ShapeProperties(
                                        new A.Transform2D(
                                            new A.Offset() { X = 0L, Y = 0L },
                                            new A.Extents() { Cx = cx, Cy = cy }),
                                        new A.PresetGeometry(
                                            new A.AdjustValueList()
                                        )
                                        { Preset = A.ShapeTypeValues.Rectangle }))
                            )
                            { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
                    )
                    {
                        DistanceFromTop = (UInt32Value)0U,
                        DistanceFromBottom = (UInt32Value)0U,
                        DistanceFromLeft = (UInt32Value)0U,
                        DistanceFromRight = (UInt32Value)0U
                    });

            var run = new Run(element);
            var paragraph = new Paragraph(run);
            var body = wordDoc.MainDocumentPart.Document.Body;
            body.AppendChild(paragraph);
        }
    }
}
