//
// iText7利用サンプル（iTextSharp7使用につき、無償利用の場合はソース公開義務有り。注意！）
//
// Author. "Masahiko Ito"<m-ito@myh.no-ip.org>
//
// 開発環境
//   エディタ　：メモ帳
//   コンパイル：c:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe /r:itext.commons.dll;itext.barcodes.dll;itext.forms.dll;itext.io.dll;itext.kernel.dll;itext.layout.dll;itext.pdfa.dll;itext.sign.dll;itext.styledxmlparser.dll;itext.svg.dll;BouncyCastle.Crypto.dll;Microsoft.Extensions.Logging.Abstractions.dll;Microsoft.Extensions.Logging.dll;Microsoft.Extensions.Options.dll Itext7Sample.cs
//   以上...
//
// 必要なDLL
//   以下の nupkg の拡張子を.zipに変更して.dllを取り出す...
//
//   bouncycastle.1.8.9.nupkg
//     BouncyCastle.Crypto.dll
//
//   itext7.7.2.2.nupkg
//     itext.barcodes.dll
//     itext.forms.dll
//     itext.io.dll
//     itext.kernel.dll
//     itext.layout.dll
//     itext.pdfa.dll
//     itext.sign.dll
//     itext.styledxmlparser.dll
//     itext.svg.dll
//
//   itext7.commons.7.2.2.nupkg
//     itext.commons.dll
//
//   microsoft.extensions.logging.5.0.0.nupkg
//     Microsoft.Extensions.Logging.dll
//
//   microsoft.extensions.logging.abstractions.5.0.0.nupkg
//     Microsoft.Extensions.Logging.Abstractions.dll
//
//   microsoft.extensions.options.5.0.0.nupkg
//     Microsoft.Extensions.Options.dll
//

using System;
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Pdf.Extgstate;
using iText.Kernel.Geom;
using iText.Kernel.Font;
using iText.Kernel.Colors;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.IO.Image;
using iText.Barcodes;

public class Program{
//======================================================================
// iText7 サンプル
//======================================================================
	static void Main(string[] args){
		Sample_ShowTextAligned("sample_showtextaligned.pdf");
		Sample_DrawLine("sample_drawline.pdf");
		Sample_DrawRectangle("sample_drawrectangle.pdf");
		Sample_FillRectangle("sample_fillrectangle.pdf");
		Sample_DrawEllipse("sample_drawellipse.pdf");
		Sample_FillEllipse("sample_fillellipse.pdf");
		Sample_DrawImage("sample_drawimage.pdf");
		Sample_DrawQRCode("sample_drawqrcode.pdf");
		Sample_SetTemplate("sample_settemplate.pdf");
	}
//======================================================================
// 文字列描画
//======================================================================
	static void Sample_ShowTextAligned(string filename){
//
// 出力する PdfDocument 作成
//
		PdfDocument PdfDocument = new PdfDocument(new PdfWriter(new FileStream(filename, FileMode.Create, FileAccess.Write)));
//
// PdfDocument から Document 作成
//
		Document Document = new Document(PdfDocument);
//
// PdfDocument に新しいページを追加し、用紙をA4縦に設定
//
//		PdfDocument.AddNewPage(new PageSize(new Rectangle(210.0f / 25.4f * 72.0f, 297.0f / 25.4f * 72.0f));
		PdfDocument.AddNewPage(PageSize.A4);
//		PdfDocument.AddNewPage(PageSize.A4.Rotate());	// A4横の場合

//
// PdfFont をＭＳ明朝で作成
//
		PdfFont PdfFont = PdfFontFactory.CreateFont(@"C:\Windows\Fonts\msgothic.ttc,0", "Identity-H");
//
// Document にＭＳ明朝を設定
//
		Document.SetFont(PdfFont);
//
// Document にフォントのサイズを 24point に設定
//
		Document.SetFontSize(24);
//
// Document にフォントの色を黒、不透過度を 1.0f に設定
//
		Document.SetFontColor(ColorConstants.BLACK, 1.0f);
//
// 用紙の右端から24.0point、下から48.0pointの位置に文字列を表示
// 指定する位置に文字列の左下が位置付けられる
// サロゲートペア文字は表示可能、IVS/IVD文字は表示不可能
//
		Document.ShowTextAligned("はろーわーるど！", 24.0f, 48.0f, TextAlignment.LEFT, VerticalAlignment.BOTTOM, 0.0f);
//
// 用紙の右端から24.0point、下から48.0pointの位置に45度傾けて文字列を表示。
//
		Document.ShowTextAligned("はろーわーるど！", 24.0f, 48.0f, TextAlignment.LEFT, VerticalAlignment.BOTTOM, (float)Math.PI / 180 * 45);
//
// Document を閉じてPDFを完成させる
//
		Document.Close();
	}
//======================================================================
// 直線描画
//======================================================================
	static void Sample_DrawLine(string filename){
//
// 出力する PdfDocument 作成
//
		PdfDocument PdfDocument = new PdfDocument(new PdfWriter(new FileStream(filename, FileMode.Create, FileAccess.Write)));
//
// PdfDocument から Document 作成
//
		Document Document = new Document(PdfDocument);
//
// PdfDocument に新しいページを追加および用紙をA4縦に設定し、PdfPage を作成
//
		PdfPage PdfPage = PdfDocument.AddNewPage(PageSize.A4);
//
// PdfPage から PdfCanvas 作成
//
		PdfCanvas PdfCanvas = new PdfCanvas(PdfPage);
//
// PdfCanvas に直線描画の色を RED に設定
//
		PdfCanvas.SetStrokeColor(ColorConstants.RED);
//
// PdfCanvas に描画の線幅を2.0point に設定
//
		PdfCanvas.SetLineWidth(2.0f);
//
// 用紙の左下を原点として、(100.0point, 50.0point)から(200.0point, 100.0point)に直線を描画
//
		PdfCanvas.MoveTo(100.0f,50.0f).LineTo(200.0f,100.0f).Stroke();
//
// Document を閉じてPDFを完成させる
//
		Document.Close();
	}
//======================================================================
// 矩形描画
//======================================================================
	static void Sample_DrawRectangle(string filename){
//
// 出力する PdfDocument 作成
//
		PdfDocument PdfDocument = new PdfDocument(new PdfWriter(new FileStream(filename, FileMode.Create, FileAccess.Write)));
//
// PdfDocument から Document 作成
//
		Document Document = new Document(PdfDocument);
//
// PdfDocument に新しいページを追加および用紙をA4縦に設定し、PdfPage を作成
//
		PdfPage PdfPage = PdfDocument.AddNewPage(PageSize.A4);
//
// PdfPage から PdfCanvas 作成
//
		PdfCanvas PdfCanvas = new PdfCanvas(PdfPage);
//
// PdfCanvas に直線描画の色を RED に設定
//
		PdfCanvas.SetStrokeColor(ColorConstants.RED);
//
// PdfCanvas に描画の線幅を2.0point に設定
//
		PdfCanvas.SetLineWidth(2.0f);
//
// 用紙の左下を原点として、(10.0point, 20.0point)から右方向に30.0pointの幅, 上方向に40.0pointの高さで矩形を描画
//
		PdfCanvas.Rectangle(10.0f,20.0f,30.0f,40.0f).Stroke();
//
// Document を閉じてPDFを完成させる
//
		Document.Close();
	}
//======================================================================
// 矩形塗りつぶし
//======================================================================
	static void Sample_FillRectangle(string filename){
//
// 出力する PdfDocument 作成
//
		PdfDocument PdfDocument = new PdfDocument(new PdfWriter(new FileStream(filename, FileMode.Create, FileAccess.Write)));
//
// PdfDocument から Document 作成
//
		Document Document = new Document(PdfDocument);
//
// PdfDocument に新しいページを追加および用紙をA4縦に設定し、PdfPage を作成
//
		PdfPage PdfPage = PdfDocument.AddNewPage(PageSize.A4);
//
// PdfPage から PdfCanvas 作成
//
		PdfCanvas PdfCanvas = new PdfCanvas(PdfPage);
//
// PdfCanvas に直線描画の色を RED に設定
//
		PdfCanvas.SetStrokeColor(ColorConstants.RED);
//
// PdfCanvas に直線描画の不透過度を 1.0f に設定
//
		PdfCanvas.SetExtGState(new PdfExtGState().SetStrokeOpacity(1.0f));
//
// PdfCanvas に塗りつぶしの色を GREEN に設定
//
		PdfCanvas.SetFillColor(ColorConstants.GREEN);
//
// PdfCanvas に塗りつぶしの不透過度を 0.5f に設定
//
		PdfCanvas.SetExtGState(new PdfExtGState().SetFillOpacity(0.5f));
//
// PdfCanvas に描画の線幅を2.0point に設定
//
		PdfCanvas.SetLineWidth(2.0f);
//
// 用紙の左下を原点として、(10.0point, 20.0point)から右方向に30.0pointの幅, 上方向に40.0pointの高さで矩形を描画し内部を塗りつぶす
//
		PdfCanvas.Rectangle(10.0f,20.0f,30.0f,40.0f).FillStroke();
//
// 用紙の左下を原点として、(25.0point, 40.0point)から右方向に30.0pointの幅, 上方向に40.0pointの高さで矩形を描画し内部を塗りつぶす
//
		PdfCanvas.Rectangle(25.0f,40.0f,30.0f,40.0f).FillStroke();
//
// Document を閉じてPDFを完成させる
//
		Document.Close();
	}
//======================================================================
// 楕円描画
//======================================================================
	static void Sample_DrawEllipse(string filename){
//
// 出力する PdfDocument 作成
//
		PdfDocument PdfDocument = new PdfDocument(new PdfWriter(new FileStream(filename, FileMode.Create, FileAccess.Write)));
//
// PdfDocument から Document 作成
//
		Document Document = new Document(PdfDocument);
//
// PdfDocument に新しいページを追加および用紙をA4縦に設定し、PdfPage を作成
//
		PdfPage PdfPage = PdfDocument.AddNewPage(PageSize.A4);
//
// PdfPage から PdfCanvas 作成
//
		PdfCanvas PdfCanvas = new PdfCanvas(PdfPage);
//
// PdfCanvas に直線描画の色を RED に設定
//
		PdfCanvas.SetStrokeColor(ColorConstants.RED);
//
// PdfCanvas に描画の線幅を2.0point に設定
//
		PdfCanvas.SetLineWidth(2.0f);
//
// 用紙の左下を原点として、(10.0point, 20.0point)-(30.0point, 40.0point)を対角とする矩形内に接する楕円を描画
//
		PdfCanvas.Ellipse(10.0f, 20.0f, 30.0f, 40.0f).Stroke();
//
// Document を閉じてPDFを完成させる
//
		Document.Close();
	}
//======================================================================
// 楕円塗りつぶし
//======================================================================
	static void Sample_FillEllipse(string filename){
//
// 出力する PdfDocument 作成
//
		PdfDocument PdfDocument = new PdfDocument(new PdfWriter(new FileStream(filename, FileMode.Create, FileAccess.Write)));
//
// PdfDocument から Document 作成
//
		Document Document = new Document(PdfDocument);
//
// PdfDocument に新しいページを追加および用紙をA4縦に設定し、PdfPage を作成
//
		PdfPage PdfPage = PdfDocument.AddNewPage(PageSize.A4);
//
// PdfPage から PdfCanvas 作成
//
		PdfCanvas PdfCanvas = new PdfCanvas(PdfPage);
//
// PdfCanvas に直線描画の色を RED に設定
//
		PdfCanvas.SetStrokeColor(ColorConstants.RED);
//
// PdfCanvas に直線描画の不透過度を 1.0f に設定
//
		PdfCanvas.SetExtGState(new PdfExtGState().SetStrokeOpacity(1.0f));
//
// PdfCanvas に塗りつぶしの色を GREEN に設定
//
		PdfCanvas.SetFillColor(ColorConstants.GREEN);
//
// PdfCanvas に塗りつぶしの不透過度を 0.5f に設定
//
		PdfCanvas.SetExtGState(new PdfExtGState().SetFillOpacity(0.5f));
//
// PdfCanvas に描画の線幅を2.0point に設定
//
		PdfCanvas.SetLineWidth(2.0f);
//
// 用紙の左下を原点として、(10.0point, 20.0point)-(30.0point, 40.0point)を対角とする矩形内に接する楕円を描画し内部を塗りつぶす
//
		PdfCanvas.Ellipse(10.0f,20.0f,30.0f,40.0f).FillStroke();
//
// 用紙の左下を原点として、(20.0point, 30.0point)-(40.0point, 50.0point)を対角とする矩形内に接する楕円を描画し内部を塗りつぶす
//
		PdfCanvas.Ellipse(20.0f,30.0f,40.0f,50.0f).FillStroke();
//
// Document を閉じてPDFを完成させる
//
		Document.Close();
	}
//======================================================================
// 画像ファイル描画
//======================================================================
	static void Sample_DrawImage(string filename){
//
// 出力する PdfDocument 作成
//
		PdfDocument PdfDocument = new PdfDocument(new PdfWriter(new FileStream(filename, FileMode.Create, FileAccess.Write)));
//
// PdfDocument から Document 作成
//
		Document Document = new Document(PdfDocument);
//
// PdfDocument に新しいページを追加および用紙をA4縦に設定し、PdfPage を作成
//
		PdfPage PdfPage = PdfDocument.AddNewPage(PageSize.A4);
//
// PdfPage から PdfCanvas 作成
//
		PdfCanvas PdfCanvas = new PdfCanvas(PdfPage);
//
// PdfCanvas に塗りつぶしの（描画の）不透過度を 0.5f に設定
//
		PdfCanvas.SetExtGState(new PdfExtGState().SetFillOpacity(0.5f));
//
// image.png を取り込んで、 ImageData を作成
//
		ImageData ImageData = ImageDataFactory.Create(@"image.png");
//
// ImageData から Image を作成
//
		Image Image = new Image(ImageData);
//
// イメージの幅を 100point、高さを 50point に設定
//
		Image.ScaleAbsolute(100.0f, 50.0f);
//
// 用紙の左下を原点として、(1ページ目の)(20.0point, 30.0point)　にイメージを位置づけ
//
		Image.SetFixedPosition(1, 20.0f, 30.0f);
//
// Document に Image を描画
//
		Document.Add(Image);
//
// Document を閉じてPDFを完成させる
//
		Document.Close();
	}
//======================================================================
// QRCode描画
//======================================================================
	static void Sample_DrawQRCode(string filename){
//
// 出力する PdfDocument 作成
//
		PdfDocument PdfDocument = new PdfDocument(new PdfWriter(new FileStream(filename, FileMode.Create, FileAccess.Write)));
//
// PdfDocument から Document 作成
//
		Document Document = new Document(PdfDocument);
//
// PdfDocument に新しいページを追加および用紙をA4縦に設定し、PdfPage を作成
//
		PdfPage PdfPage = PdfDocument.AddNewPage(PageSize.A4);
//
// PdfPage から PdfCanvas 作成
//
		PdfCanvas PdfCanvas = new PdfCanvas(PdfPage);
//
// PdfCanvas に塗りつぶしの（描画の）不透過度を 1.0f に設定
//
		PdfCanvas.SetExtGState(new PdfExtGState().SetFillOpacity(1.0f));
//
// "this is test qrcode" を内容とする BarcodeQRCode を作成
//
		BarcodeQRCode BarcodeQRCode = new BarcodeQRCode("this is test qrcode");
//
// BarcodeQRCode から、描画色を BLACK で PdfFormXObject を作成
//
		PdfFormXObject PdfFormXObject = BarcodeQRCode.CreateFormXObject(ColorConstants.BLACK, PdfDocument);
//
// PdfFormXObject から ImageBarcode を作成
//
		Image ImageBarcode = new Image(PdfFormXObject);
//
// ImageBarcode にイメージの幅を 100point、高さを 100point に設定
//
		ImageBarcode.ScaleAbsolute(100.0f, 100.0f);
//
// ImageBarcode に用紙の左下を原点として、(1ページ目の)(20.0point, 30.0point)　にイメージを位置づけ
//
		ImageBarcode.SetFixedPosition(1, 20.0f, 30.0f);
//
// Document に ImageBarcode を描画
//
		Document.Add(ImageBarcode);
//
// Document を閉じてPDFを完成させる
//
		Document.Close();
	}
//======================================================================
// テンプレート設定
//======================================================================
	static void Sample_SetTemplate(string filename){
//
// 出力する PdfDocument 作成
//
		PdfDocument PdfDocument = new PdfDocument(new PdfWriter(new FileStream(filename, FileMode.Create, FileAccess.Write)));
//
// PdfDocument から Document 作成
//
		Document Document = new Document(PdfDocument);
//
// template.pdf から PdfDocumentTemplate 生成
//
		PdfDocument PdfDocumentTemplate = new PdfDocument(new PdfReader(@"template.pdf"));
//
// PdfDocumentTemplate の1ページ目を取り込んで PagePdfTemplate 生成
//
		PdfPage PagePdfTemplate = PdfDocumentTemplate.GetPage(1);
//
// PagePdfTemplate から PdfXObject 生成
//
		PdfXObject PdfXObject = PagePdfTemplate.CopyAsFormXObject(PdfDocument);
//
// 取込が完了して不要になった PdfDocumentTemplate を閉じる
//
		PdfDocumentTemplate.Close();
//
// PdfDocument に新しいページ（１ページ目）を追加および用紙をA4縦に設定し、PdfPage を作成
//
		PdfPage PdfPage = PdfDocument.AddNewPage(PageSize.A4);
//
// PdfPage から PdfCanvas 作成
//
		PdfCanvas PdfCanvas = new PdfCanvas(PdfPage);
//
// 新しいページに PdfXObject（テンプレート） を設定
//
		PdfCanvas.AddXObjectAt(PdfXObject, 0, 0);
//
// PdfDocument に新しいページ（２ページ目）を追加および用紙をA4縦に設定し、PdfPage を作成
//
		PdfPage = PdfDocument.AddNewPage(PageSize.A4);
//
// PdfPage から PdfCanvas 作成
//
		PdfCanvas = new PdfCanvas(PdfPage);
//
// 新しいページに PdfXObject（テンプレート） を設定
//
		PdfCanvas.AddXObjectAt(PdfXObject, 0, 0);
//
// Document を閉じてPDFを完成させる
//
		Document.Close();
	}
}
