//
// Pdf.dll利用サンプル（iTextSharp7使用につき、無償利用の場合はソース公開義務有り。注意！）
//
// Author. "Masahiko Ito"<m-ito@myh.no-ip.org>
//
// Compile: c:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe /r:Pdf.dll PdfSample1.cs
//
using System.IO;
using System.Linq;
using System.Text;
using System;

public class PdfSample{
	static void Main(string[] args){

		Pdf mis = new Pdf("A4", "sample.pdf");
//		Pdf mis = new Pdf(210.0f, 297.0f, "sample.pdf");

		mis.setXbias(5.0f);
		mis.setYbias(1.0f);

		mis.setTemplate(@"template.pdf");

		mis.open();
		mis.setColor("BLACK");
		mis.showTextByPoint(7.2f*30, 72.0f/6.0f*6, "漢字文字列test", 14.4f);
		mis.showTextByPoint(12.0f, 12.0f, "漢字文字列", 12.0f);
		mis.showText(0.0f, 0.0f, "000000000111111111122222222223333333333444444444455555555556666666666777777777788888888889999999999", 14.4f);
		mis.showText(0.0f, 1.0f, "000000000111111111122222222223333333333444444444455555555556666666666777777777788888888889999999999", 14.4f);
		mis.showText(5.0f, 6.0f, "漢字文字列test", 14.4f);
		mis.showText(0.0f, 2.0f, "漢字文字列", 14.4f);
		mis.showText(0.0f, 3.0f, "漢字文字列", 14.4f);
		mis.showText(0.0f, 4.0f, "𠮷漢字文字列", 14.4f);
		mis.showText(0.0f, 5.0f, "辻󠄀漢字文字列", 14.4f);
//		mis.setFont(@"C:\Windows\Fonts\msmincho.ttf");
		mis.showText(0.0f, 6.0f, "辻漢字文字列", 14.4f);

		mis.setColor("RED");
		mis.drawLineByPoint(12.0f, 12.0f, 120.0f, 120.0f, 5.0f);
		mis.drawLine(0.0f, 0.0f, 5.0f, 5.0f, 1.0f);
		mis.setColor("GREEN");
		mis.drawRect(2.0f, 2.0f, 5.0f, 5.0f, 1.0f);
		mis.fillRect(12.0f, 2.0f, 15.0f, 5.0f, 1.0f);
		mis.setImage(@"image.png");
		mis.showImage(3.0f, 3.0f, 5.0f, 5.0f);
		mis.showImageInBox(10.0f, 3.0f, 20.0f, 20.0f);
		mis.setColor("BLUE");
		mis.drawOval(15.0f, 25.0f, 12.0f, 22.0f, 1.0f);
		mis.fillOval(25.0f, 25.0f, 22.0f, 22.0f, 1.0f);

		mis.setColor("GREEN");
		mis.setOpacity(0.5f);
		mis.setQRCode("This is QRCode test by PdfSample1.exe");
		mis.showQRCodeByPoint(14.4f, 14.4f*6, 14.4f*4, 14.4f*9);
		mis.setOpacity(1.0f);
		mis.showQRCode(30.0f, 5.0f, 40.0f, 10.0f);
		mis.showQRCodeInBox(30.0f, 20.0f, 40.0f, 25.0f);

		mis.newPage();
		mis.setFont(@"C:\Windows\Fonts\msgothic.ttc,0");
		mis.setColor("RED");
		mis.setOpacity(0.5f);
		mis.showTextByPoint(12.0f, 12.0f, "漢字文字列", 12.0f);
		mis.showText(0.0f, 0.0f, "000000000111111111122222222223333333333444444444455555555556666666666777777777788888888889999999999", 14.4f);
		mis.showText(0.0f, 1.0f, "000000000111111111122222222223333333333444444444455555555556666666666777777777788888888889999999999", 14.4f);
		mis.showText(0.0f, 2.0f, "漢字文字列", 14.4f);
		mis.showText(0.0f, 3.0f, "漢字文字列", 14.4f);
		mis.showText(0.0f, 4.0f, "𠮷漢字文字列", 14.4f);
		mis.showText(0.0f, 5.0f, "辻󠄀漢字文字列", 14.4f);
//		mis.setFont(@"C:\Windows\Fonts\msmincho.ttf");
		mis.showText(0.0f, 6.0f, "辻漢字文字列", 14.4f);

		mis.setColor("RED");
		mis.drawLineByPoint(12.0f, 12.0f, 120.0f, 120.0f, 5.0f);
		mis.drawLine(1.0f, 1.0f, 5.0f, 5.0f, 1.0f);
		mis.setColor("GREEN");
		mis.drawRect(2.0f, 2.0f, 5.0f, 5.0f, 1.0f);
		mis.setImage(@"image.png");
		mis.showImage(3.0f, 3.0f, 5.0f, 5.0f);
		mis.setOpacity(0.5f);
		mis.showImageInBox(10.0f, 3.0f, 20.0f, 20.0f);
		mis.setOpacity(1.0f);

		mis.close();

		return;
	}
}
