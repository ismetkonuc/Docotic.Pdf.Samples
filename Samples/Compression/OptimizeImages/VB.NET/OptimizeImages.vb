Imports System
Imports System.Diagnostics
Imports System.IO
Imports System.Windows.Forms

Imports Microsoft.VisualBasic

Imports BitMiracle.Docotic.Pdf

Namespace BitMiracle.Docotic.Pdf.Samples
    Public NotInheritable Class OptimizeImages
        Public Shared Sub Main()
            ' NOTE: 
            ' When used in trial mode, the library imposes some restrictions.
            ' Please visit http://bitmiracle.com/pdf-library/trial-restrictions.aspx
            ' for more information.

            Const originalFile As String = "Sample Data\jpeg.pdf"
            Const compressedFile As String = "OptimizeImages.pdf"

            Using pdf As New PdfDocument(originalFile)
                For Each page As PdfPage In pdf.Pages
                    For Each painted As PdfPaintedImage In page.GetPaintedImages()
                        Dim image As PdfImage = painted.Image

                        ' image that is used as mask or image with attached mask are
                        ' not good candidates for recompression
                        If image.IsMask OrElse image.Mask IsNot Nothing OrElse image.Width < 8 OrElse image.Height < 8 Then
                            Continue For
                        End If

                        ' get size of the painted image
                        Dim width As Integer = Math.Max(1, CInt(painted.Bounds.Width))
                        Dim height As Integer = Math.Max(1, CInt(painted.Bounds.Height))

                        If image.ComponentCount = 1 AndAlso image.BitsPerComponent = 1 AndAlso image.Compression <> PdfImageCompression.Group4Fax Then
                            image.RecompressWithGroup4Fax()
                        ElseIf width < image.Width OrElse height < image.Height Then
                            ' NOTE: PdfImage.ResizeTo() method is not supported in version for .NET Standard
                            If image.ComponentCount = 3 Then
                                image.ResizeTo(width, height, PdfImageCompression.Jpeg, 90)
                            Else
                                image.ResizeTo(width, height, PdfImageCompression.Flate, 9)
                            End If
                        End If
                    Next
                Next

                pdf.Save(compressedFile)
            End Using

            Dim message As String = String.Format(
                "Original file size: {0} bytes;" & vbCr & vbLf & "Compressed file size: {1} bytes",
                New FileInfo(originalFile).Length,
                New FileInfo(compressedFile).Length
            )
            MessageBox.Show(message)

            Process.Start(compressedFile)
        End Sub
    End Class
End Namespace
