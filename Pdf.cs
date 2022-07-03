//
// PDF作成クラス（iTextSharp7使用につき、無償利用の場合はソース公開義務有り。注意！）
//
// Author. "Masahiko Ito"<m-ito@myh.no-ip.org>
//
// Compile: c:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe /r:itext.commons.dll;itext.barcodes.dll;itext.forms.dll;itext.io.dll;itext.kernel.dll;itext.layout.dll;itext.pdfa.dll;itext.sign.dll;itext.styledxmlparser.dll;itext.svg.dll;BouncyCastle.Crypto.dll;Microsoft.Extensions.Logging.Abstractions.dll;Microsoft.Extensions.Logging.dll;Microsoft.Extensions.Options.dll /t:library Pdf.cs
//
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
using System.IO;
using System.Linq;
using System.Text;
using System;

public enum CurrentFont
{
	unknown,
	normal,
	pua
}

//
// PDF作成クラス（iTextSharp7使用につき、無償利用の場合はソース公開義務有り。注意！）
//
public class Pdf{
	PdfDocument PdfDocument;
	PageSize PageSize;		// A4縦
	Document Document;
	PdfFont PdfFont;		// MSゴシック
	PdfFont PdfFontEudc;		// EUDC.TTE
	PdfDocument PdfDocumentTemplate;
	PdfPage PdfPage;
	PdfCanvas PdfCanvas;
	PdfPage PagePdfTemplate;
	PdfXObject PdfXObject;
	ImageData ImageData;
	Image Image;
	BarcodeQRCode BarcodeQRCode;
	PdfFormXObject PdfFormXObject;
	Image ImageBarcode;
	Color Color;
	float Opacity;
	string TempPuaFontname;
	int Page;
	CurrentFont CurrentFont;	// unknown normal pua
	float CurrentFontPoint;		// 0.0f:unknown
	float CurrentCanvasPoint;	// 0.0f:unknown
	float XBIAS;			// character
	float YBIAS;			// line
	float CPI;			// character / inch
	float LPI;			// line / inch
	float XUNIT;			// point
	float YUNIT;			// point
	float WIDTH;			// point
	float HEIGHT;			// point

//
// 機能：初期化
//
// 引数：paper "a4", "a4v", "A4", "A4V" (A4縦)
//             "a4h", "A4H"             (A4横)
//             "a3", "a3v", "A3", "A3V" (A3縦)
//             "a3h", "A3H"             (A3横)
//       pdffilename 出力PDFファイル名
//
// 例：Pdf pdf = new Pdf("a4", @"output.pdf");
//
	public Pdf(string paper, string pdffilename){
		if (paper.CompareTo("a4") == 0 ||
			paper.CompareTo("a4v") == 0 ||
			paper.CompareTo("A4") == 0 ||
			paper.CompareTo("A4V") == 0){
			this.PageSize = PageSize.A4;
			this.WIDTH = 210.0f / 25.4f * 72.0f;
			this.HEIGHT = 297.0f / 25.4f * 72.0f;
		}else if (paper.CompareTo("a4h") == 0 ||
			paper.CompareTo("A4H") == 0){
			this.PageSize = PageSize.A4.Rotate();
			this.WIDTH = 297.0f / 25.4f * 72.0f;
			this.HEIGHT = 210.0f / 25.4f * 72.0f;
		}else if (paper.CompareTo("a3") == 0 ||
			paper.CompareTo("a3v") == 0 ||
			paper.CompareTo("A3") == 0 ||
			paper.CompareTo("A3V") == 0){
			this.PageSize = PageSize.A3;
			this.WIDTH = 297.0f / 25.4f * 72.0f;
			this.HEIGHT = 420.0f / 25.4f * 72.0f;
		}else if (paper.CompareTo("a3h") == 0 ||
			paper.CompareTo("A3H") == 0){
			this.PageSize = PageSize.A3.Rotate();
			this.WIDTH = 420.0f / 25.4f * 72.0f;
			this.HEIGHT = 297.0f / 25.4f * 72.0f;
		}else{
			this.PageSize = PageSize.A4;
			this.WIDTH = 210.0f / 25.4f * 72.0f;
			this.HEIGHT = 297.0f / 25.4f * 72.0f;
		}
		PdfCommon(pdffilename);
	}

//
// 機能：初期化
//
// 引数：w 用紙の横幅(mm)
//       h 用紙の縦幅(mm)
//       pdffilename 出力PDFファイル名
//
// 例：Pdf pdf = new Pdf(210.0f, 297.0f, @"output.pdf");
//
	public Pdf(float w, float h, string pdffilename){
		this.WIDTH = w / 25.4f * 72.0f;
		this.HEIGHT = h / 25.4f * 72.0f;
		this.PageSize = new PageSize(new Rectangle(this.WIDTH, this.HEIGHT));	// 任意サイズ(point)
		PdfCommon(pdffilename);
	}

//
// 機能：初期化（共通ルーチン：内部利用）
//
// 引数：pdffilename 出力PDFファイル名
//
// 例：PdfCommon(@"output.pdf");
//
	public void PdfCommon(string pdffilename){
		this.PdfDocument = new PdfDocument(new PdfWriter(new FileStream(pdffilename, FileMode.Create, FileAccess.Write)));
		this.Document = new Document(PdfDocument);
		this.PdfFont = PdfFontFactory.CreateFont(@"C:\Windows\Fonts\msgothic.ttc,0", "Identity-H");
		try{
			this.TempPuaFontname = System.IO.Path.GetTempFileName() + ".TTF";
			File.Copy(@"C:\Windows\Fonts\EUDC.TTE", this.TempPuaFontname, true);
			this.PdfFontEudc = PdfFontFactory.CreateFont(this.TempPuaFontname, "Identity-H");
		}catch{
			this.TempPuaFontname = null;
			this.PdfFontEudc = PdfFontFactory.CreateFont(@"C:\Windows\Fonts\msgothic.ttc,0", "Identity-H");
		}
		this.PdfDocumentTemplate = null;
		this.PdfPage = null;
		this.PdfCanvas = null;
		this.PagePdfTemplate = null;
		this.PdfXObject = null;
		this.ImageData = null;
		this.Image = null;
		this.BarcodeQRCode = null;
		this.PdfFormXObject = null;
		this.ImageBarcode = null;
		this.Color = ColorConstants.BLACK;
		this.Opacity = 1.0f;
		this.Page = 0;
		this.CurrentFont = CurrentFont.unknown;
		this.CurrentFontPoint = 0.0f;
		this.CurrentCanvasPoint = 0.0f;
		this.XBIAS = 0.0f;
		this.YBIAS = 0.0f;
		this.CPI = 10.0f;
		this.LPI = 6.0f;
		this.XUNIT = 72.0f / CPI;
		this.YUNIT = 72.0f / LPI;
	}

//
// 機能：フォントファイル設定（正字）
//
// 引数：fontfile フォントファイル名(規定値："C:\Windows\Fonts\msgothic.ttc,0")
//
// 例：pdf.setFont(@"C:\Windows\Fonts\msgothic.ttc,0");
//
	public void setFont(string fontfile){
		this.PdfFont = PdfFontFactory.CreateFont(fontfile, "Identity-H");
		this.CurrentFont = CurrentFont.unknown;
		this.CurrentFontPoint = 0.0f;
	}	

//
// 機能：フォントファイル設定（外字）
//
// 引数：fontfile フォントファイル名
//                (規定値："C:\Windows\Fonts\EUDC.TTE"を一時ファイルにコピーし、拡張子を.TTFに変えたファイル)
//
// 例：pdf.setPuaFont(@"C:\Windows\Fonts\EUDC.TTF");
//
	public void setPuaFont(string fontfile){
		this.TempPuaFontname = null;
		this.PdfFontEudc = PdfFontFactory.CreateFont(fontfile, "Identity-H");
		this.CurrentFont = CurrentFont.unknown;
		this.CurrentFontPoint = 0.0f;
	}	

//
// 機能：横方向印字位置調整
//
// 引数：xbias 右方向印字位置調整値(カラム単位)(規定値：0.0f)
//
// 例：pdf.setXbias(5.5f); // 印字位置を右に半角5.5文字分ずらす
//
	public void setXbias(float xbias){
		this.XBIAS = xbias;
	}

//
// 機能：縦方向印字位置調整
//
// 引数：ybias 下方向印字位置調整値(行単位)(規定値：0.0f)
//
// 例：pdf.setYbias(5.5f); // 印字位置を下に5.5行分ずらす
//
	public void setYbias(float ybias){
		this.YBIAS = ybias;
	}

//
// 機能：カラム単位の設定
//
// 引数：cpi 横方向の、1インチ当たりの半角文字数(Character / Inch)(規定値：10.0f)
//
// 例：pdf.setCpi(10.0f); // 1インチ当たり半角10文字を横方向の単位とする
//
	public void setCpi(float cpi){
		this.CPI = cpi;
		this.XUNIT = 72.0f / cpi;			// point
	}

//
// 機能：行単位の設定
//
// 引数：lpi 縦方向の、1インチ当たりの行数(Line / Inch)(規定値：6.0f)
//
// 例：pdf.setLpi(6.0f); // 1インチ当たり6行を縦方向の単位とする
//
	public void setLpi(float lpi){
		this.LPI = lpi;
		this.YUNIT = 72.0f / lpi;			// point
	}

//
// 機能：描画色の設定
//
// 引数：color "BLACK", "RED", "GREEN", "YELLOW", "BLUE", "MAGENTA", "CYAN", "WHITE"
//
// 例：pdf.setColor("BLACK");
//
	public void setColor(string color){
		if (color.CompareTo("BLACK") == 0){
			this.Color = ColorConstants.BLACK;
		}else if (color.CompareTo("RED") == 0){
			this.Color = ColorConstants.RED;
		}else if (color.CompareTo("GREEN") == 0){
			this.Color = ColorConstants.GREEN;
		}else if (color.CompareTo("YELLOW") == 0){
			this.Color = ColorConstants.YELLOW;
		}else if (color.CompareTo("BLUE") == 0){
			this.Color = ColorConstants.BLUE;
		}else if (color.CompareTo("MAGENTA") == 0){
			this.Color = ColorConstants.MAGENTA;
		}else if (color.CompareTo("CYAN") == 0){
			this.Color = ColorConstants.CYAN;
		}else if (color.CompareTo("WHITE") == 0){
			this.Color = ColorConstants.WHITE;
		}else{
			this.Color = ColorConstants.BLACK;
		}
		this.Document.SetFontColor(this.Color, this.Opacity);
		this.PdfCanvas.SetStrokeColor(this.Color);
		this.PdfCanvas.SetFillColor(this.Color);
	}

//
// 機能：不透明度の設定
//
// 引数：opacity 0.0f(透明) - 1.0f(不透明)
//
// 例：pdf.setOpacity(0.5f);
//
	public void setOpacity(float opacity){
		this.Opacity = opacity;		
		this.Document.SetFontColor(this.Color, this.Opacity);
		this.PdfCanvas.SetExtGState(new PdfExtGState().SetStrokeOpacity(this.Opacity));
		this.PdfCanvas.SetExtGState(new PdfExtGState().SetFillOpacity(this.Opacity));
	}

//
// 機能：オープン
//
// 引数：無し
//
// 例：pdf.open();
//
	public void open(){
		newPage();
	}

//
// 機能：クローズ
//
// 引数：無し
//
// 例：pdf.close();
//
	public void close(){
		this.Document.Close();
		try{
			if (this.TempPuaFontname != null){
				File.Delete(this.TempPuaFontname);
			}
		}catch{
			// ignore error
		}
	}

//
// 機能：テンプレート（下地）の設定と表示
//
// 引数：templatefile テンプレートになるPDFファイル名
//
// 例：pdf.setTemplate(@"Template.pdf");
//
	public void setTemplate(string templatefile){
		if (templatefile == null){
			this.PdfDocumentTemplate = null;
			this.PagePdfTemplate = null;
			this.PdfXObject = null;
		}else if (templatefile.CompareTo("") == 0){
			this.PdfDocumentTemplate = null;
			this.PagePdfTemplate = null;
			this.PdfXObject = null;
		}else{
			this.PdfDocumentTemplate = new PdfDocument(new PdfReader(templatefile));
			this.PagePdfTemplate = PdfDocumentTemplate.GetPage(1);
			this.PdfXObject = PagePdfTemplate.CopyAsFormXObject(this.PdfDocument);
			this.PdfDocumentTemplate.Close();
		}
	}

//
// 機能：テンプレート（下地）の設定取消し
//
// 引数：無し
//
// 例：pdf.unsetTemplate();
//
	public void unsetTemplate(){
		this.PdfDocumentTemplate = null;
		this.PagePdfTemplate = null;
		this.PdfXObject = null;
	}

//
// 機能：改ページ（テンプレートの表示）
//
// 引数：無し
//
// 例：pdf.newPage();
//
	public void newPage(){
		this.PdfPage = PdfDocument.AddNewPage(this.PageSize);
		this.PdfCanvas = new PdfCanvas(this.PdfPage);
		this.PdfCanvas.SetStrokeColor(this.Color);
		this.PdfCanvas.SetFillColor(this.Color);
		this.PdfCanvas.SetExtGState(new PdfExtGState().SetStrokeOpacity(this.Opacity));
		this.PdfCanvas.SetExtGState(new PdfExtGState().SetFillOpacity(this.Opacity));
		if (this.PdfXObject != null){
			this.PdfCanvas.AddXObjectAt(this.PdfXObject, 0, 0);
		}
		this.Page += 1;
	}

//
// 機能：横方向位置変換（カラム位置→ポイント位置：内部利用）
//
// 引数：c カラム位置
//
// 戻値：引数(c)をポイントに変改した値
//
// 例：c2p(10.5f); // 右方向に半角10.5文字の位置をポイントで表す
//
	public float c2p(float c){
		return ((this.XBIAS * this.XUNIT) + (c * this.XUNIT));
	}


//
// 機能：縦方向位置変換（行位置→ポイント位置：内部利用）
//
// 引数：l 行位置
//
// 戻値：引数(l)をポイントに変改した値
//
// 例：l2p(10.5f); // 下方向に10.5行の位置をポイントで表す
//
	public float l2p(float l){
		return (this.HEIGHT - (l * this.YUNIT) - (this.YBIAS * this.YUNIT));
	}

//
// 機能：指定した位置に文字列を表示する
//
// 引数：x 横位置（ポイント）
//       y 縦位置（ポイント）
//       s 表示する文字列（半角、全角（正字、外字）混合可能）
//       point 表示する文字の大きさ（全角文字幅）（ポイント）
//       ※(x, y)は文字列の左下を指定する
//       ※原点は用紙の左下
//       ※本メソッドはsetXbias(), setYbias()の影響を受けない
//
// 例：pdf.showTextByPoint(2.0f, 3.0f, "やっほー", 12.0f);
//     pdf.showTextByPoint(2.0f, 3.0f, "やっほー", 12.0f);
//     pdf.showTextByPoint(2.0f, 3.0f, "やっほー", 12.0f); // 横方向に2.0(point)、
//                                                            上方向に3.0(point)の位置に
//                                                            「やっほー」と
//                                                            12.0fポイントの大きさの文字で表示する
//
	public void showTextByPoint(float x, float y, string s, float point){
		float xpoint = x;
		char [] ca = s.ToCharArray();
		for (int i = 0; i < ca.Length; i++){
			int isave = i;
			if (ca[i] >= '\uE000' && ca[i] <= '\uF8FF'){	// 外字の場合
				if (this.CurrentFont != CurrentFont.pua || this.CurrentFontPoint != point){
					this.Document.SetFont(this.PdfFontEudc).SetFontSize(point);
					this.CurrentFont = CurrentFont.pua;
					this.CurrentFontPoint = point;
				}
				this.Document.ShowTextAligned(ca[i].ToString(), xpoint, y, TextAlignment.LEFT, VerticalAlignment.BOTTOM, 0);
				xpoint += point;
			}else{	// 正字の場合
				char [] oc;
//				if (ca[i] >= '\uD800' && ca[i] <= '\uDBff'){	// サロゲートペアの場合(surrogate2 DC00-DFFF)
				if (Char.IsHighSurrogate(ca[i])){		// サロゲートペアの場合(surrogate2 DC00-DFFF)
					if ((i + 2) < ca.Length){
						if (ca[i + 2] == '\uDB40'){	// IVS付きの場合(IVS2 DD00-DDEF)
							oc = new char [] {ca[i],ca[i+1],ca[i+2],ca[i+3]};
							i += 3;
						}else{				// IVS無しの場合
							oc = new char [] {ca[i],ca[i+1]};
							i += 1;
						}
					}else{	// IVS無しの場合
						oc = new char [] {ca[i],ca[i+1]};
						i += 1;
					}
				}else{	// サロゲートペアでない場合
					if ((i + 1) < ca.Length){
						if (ca[i + 1] == '\uDB40'){	// IVS付きの場合
							oc = new char [] {ca[i],ca[i+1],ca[i+2]};
							i += 2;
						}else{				// IVS無しの場合
							oc = new char [] {ca[i]};
						}
					}else{	// IVS無しの場合
						oc = new char [] {ca[i]};
					}
				}
				String os = new String(oc);
				if (this.CurrentFont != CurrentFont.normal || this.CurrentFontPoint != point){
					this.Document.SetFont(this.PdfFont).SetFontSize(point);
					this.CurrentFont = CurrentFont.normal;
					this.CurrentFontPoint = point;
				}
				this.Document.ShowTextAligned(os, xpoint, y, TextAlignment.LEFT, VerticalAlignment.BOTTOM, 0.0f);

//				if (ca[isave] >= '\uD800' && ca[isave] <= '\uDBff'){	// サロゲートペアの場合(surrogate2 DC00-DFFF)
				if (Char.IsHighSurrogate(ca[isave])){			// サロゲートペアの場合(surrogate2 DC00-DFFF)
						xpoint += point;
				}else{							// サロゲートペアでない場合
					byte [] buf = Encoding.GetEncoding("Shift_jis").GetBytes(ca[isave].ToString());
					if (buf.Length == 1){	// 半角文字の場合
						xpoint += (point / 2.0f);
					}else{			// 全角文字の場合
						xpoint += point;
					}
				}
			}
		}
	}

//
// 機能：指定した位置に文字列を表示する
//
// 引数：x 横位置（半角文字単位）
//       y 縦位置（行単位）
//       s 表示する文字列（半角、全角（正字、外字）混合可能）
//       point 表示する文字の大きさ（全角文字幅）（ポイント）
//       ※(x, y)は文字列の左下を指定する
//       ※原点は用紙の左上
//
// 例：pdf.showText(2.0f, 3.0f, "やっほー", 12.0f); // 横方向に2.0文字、
//                                                     下方向に3.0行の位置に
//                                                     「やっほー」と
//                                                     12.0fポイントの大きさの文字で表示する
//
	public void showText(float x, float y, string s, float point){
		showTextByPoint(c2p(x), l2p(y), s, point);
	}

//
// 機能：直線を描画する
//
// 引数：sx 開始横位置（ポイント）
//       sy 開始縦位置（ポイント）
//       ex 終了横位置（ポイント）
//       ey 終了縦位置（ポイント）
//       point 線の太さ（ポイント）
//       ※原点は用紙の左下
//       ※本メソッドはsetXbias(), setYbias()の影響を受けない
//
// 例：pdf.drawLineByPoin(2.0f, 3.0f, 120.0f, 3.0f, 12.0f); // (2.0f, 3.0f)-(120.0f, 3.0f)に
//                                                             12.0fポイントの太さの線を描く、
//
	public void drawLineByPoint(float sx, float sy, float ex, float ey, float point){
		if (this.CurrentCanvasPoint != point){
			this.PdfCanvas.SetLineWidth(point);
			this.CurrentCanvasPoint = point;
		}
		this.PdfCanvas.MoveTo(sx,sy).LineTo(ex,ey).Stroke();
	}

//
// 機能：直線を描画する
//
// 引数：sx 開始横位置（半角文字単位）
//       sy 開始縦位置（行単位）
//       ex 終了横位置（半角文字単位）
//       ey 終了縦位置（行単位）
//       point 線の太さ（ポイント）
//       ※原点は用紙の左上
//
// 例：pdf.drawLine(2.0f, 3.0f, 120.0f, 3.0f, 12.0f); // (2.0f, 3.0f)-(120.0f, 3.0f)に
//                                                       12.0fポイントの太さの線を描く
//
	public void drawLine(float sx, float sy, float ex, float ey, float point){
		drawLineByPoint(c2p(sx), l2p(sy), c2p(ex), l2p(ey), point);
	}

//
// 機能：矩形を描画する
//
// 引数：sx 開始横位置（ポイント）
//       sy 開始縦位置（ポイント）
//       ex 終了横位置（ポイント）
//       ey 終了縦位置（ポイント）
//       point 線の太さ（ポイント）
//       ※原点は用紙の左下
//       ※本メソッドはsetXbias(), setYbias()の影響を受けない
//
// 例：pdf.drawRectByPoint(2.0f, 3.0f, 120.0f, 3.0f, 12.0f); // (2.0f, 3.0f)-(120.0f, 3.0f)を対角とする矩形を
//                                                              12.0fポイントの太さの線で描く
//
	public void drawRectByPoint(float sx, float sy, float ex, float ey, float point){
		float x, y, w, h;
		if (sx < ex){
			x = sx;
			w = ex - sx;
		}else{
			x = ex;
			w = sx - ex;
		}
		if (sy < ey){
			y = sy;
			h = ey - sy;
		}else{
			y = ey;
			h = sy - ey;
		}
		if (this.CurrentCanvasPoint != point){
			this.PdfCanvas.SetLineWidth(point);
			this.CurrentCanvasPoint = point;
		}
		this.PdfCanvas.Rectangle(x, y, w, h).Stroke();	// x(left),y(bottom),width,height

	}

//
// 機能：矩形を描画する
//
// 引数：sx 開始横位置（半角文字単位）
//       sy 開始縦位置（行単位）
//       ex 終了横位置（半角文字単位）
//       ey 終了縦位置（行単位）
//       point 線の太さ（ポイント）
//       ※原点は用紙の左上
//
// 例：pdf.drawRect(2.0f, 3.0f, 120.0f, 3.0f, 12.0f); // (2.0f, 3.0f)-(120.0f, 3.0f)を対角とする矩形を
//                                                       12.0fポイントの太さの線で描く
//
	public void drawRect(float sx, float sy, float ex, float ey, float point){
		this.drawRectByPoint(c2p(sx), l2p(sy), c2p(ex), l2p(ey), point);
	}

//
// 機能：矩形を塗りつぶす
//
// 引数：sx 開始横位置（ポイント）
//       sy 開始縦位置（ポイント）
//       ex 終了横位置（ポイント）
//       ey 終了縦位置（ポイント）
//       point 線の太さ（ポイント）
//       ※原点は用紙の左下
//       ※本メソッドはsetXbias(), setYbias()の影響を受けない
//
// 例：pdf.fillRectByPoint(2.0f, 3.0f, 120.0f, 3.0f, 12.0f); // (2.0f, 3.0f)-(120.0f, 3.0f)を対角とする矩形を
//                                                              12.0fポイントの太さの線で描き塗りつぶす
//
	public void fillRectByPoint(float sx, float sy, float ex, float ey, float point){
		float x, y, w, h;
		if (sx < ex){
			x = sx;
			w = ex - sx;
		}else{
			x = ex;
			w = sx - ex;
		}
		if (sy < ey){
			y = sy;
			h = ey - sy;
		}else{
			y = ey;
			h = sy - ey;
		}
		if (this.CurrentCanvasPoint != point){
			this.PdfCanvas.SetLineWidth(point);
			this.CurrentCanvasPoint = point;
		}
		this.PdfCanvas.Rectangle(x, y, w, h).FillStroke();	// x(left),y(bottom),width,height

	}

//
// 機能：矩形を塗りつぶす
//
// 引数：sx 開始横位置（半角文字単位）
//       sy 開始縦位置（行単位）
//       ex 終了横位置（半角文字単位）
//       ey 終了縦位置（行単位）
//       point 線の太さ（ポイント）
//       ※原点は用紙の左上
//
// 例：pdf.fillRect(2.0f, 3.0f, 120.0f, 3.0f, 12.0f); // (2.0f, 3.0f)-(120.0f, 3.0f)を対角とする矩形を
//                                                       12.0fポイントの太さの線で描き塗りつぶす
//
	public void fillRect(float sx, float sy, float ex, float ey, float point){
		this.fillRectByPoint(c2p(sx), l2p(sy), c2p(ex), l2p(ey), point);
	}

//
// 機能：楕円を描画する
//
// 引数：sx 開始横位置（ポイント）
//       sy 開始縦位置（ポイント）
//       ex 終了横位置（ポイント）
//       ey 終了縦位置（ポイント）
//       point 線の太さ（ポイント）
//       ※原点は用紙の左下
//       ※本メソッドはsetXbias(), setYbias()の影響を受けない
//
// 例：pdf.drawOvalByPoint(2.0f, 3.0f, 120.0f, 3.0f, 12.0f); // (2.0f, 3.0f)-(120.0f, 3.0f)を対角とする楕円を
//                                                              12.0fポイントの太さの線で描く
//
	public void drawOvalByPoint(float sx, float sy, float ex, float ey, float point){
		if (this.CurrentCanvasPoint != point){
			this.PdfCanvas.SetLineWidth(point);
			this.CurrentCanvasPoint = point;
		}
		this.PdfCanvas.Ellipse(sx, sy, ex, ey).Stroke();	// x(left),y(bottom),width,height
	}

//
// 機能：楕円を描画する
//
// 引数：sx 開始横位置（半角文字単位）
//       sy 開始縦位置（行単位）
//       ex 終了横位置（半角文字単位）
//       ey 終了縦位置（行単位）
//       point 線の太さ（ポイント）
//       ※原点は用紙の左上
//
// 例：pdf.drawOval(2.0f, 3.0f, 120.0f, 3.0f, 12.0f); // (2.0f, 3.0f)-(120.0f, 3.0f)を対角とする楕円を
//                                                       12.0fポイントの太さの線で描く
//
	public void drawOval(float sx, float sy, float ex, float ey, float point){
		this.drawOvalByPoint(c2p(sx), l2p(sy), c2p(ex), l2p(ey), point);
	}

//
// 機能：楕円を塗りつぶす
//
// 引数：sx 開始横位置（ポイント）
//       sy 開始縦位置（ポイント）
//       ex 終了横位置（ポイント）
//       ey 終了縦位置（ポイント）
//       point 線の太さ（ポイント）
//       ※原点は用紙の左下
//       ※本メソッドはsetXbias(), setYbias()の影響を受けない
//
// 例：pdf.fillOvalByPoint(2.0f, 3.0f, 120.0f, 3.0f, 12.0f); // (2.0f, 3.0f)-(120.0f, 3.0f)を対角とする楕円を
//                                                              12.0fポイントの太さの線で描き塗りつぶす
//
	public void fillOvalByPoint(float sx, float sy, float ex, float ey, float point){
		if (this.CurrentCanvasPoint != point){
			this.PdfCanvas.SetLineWidth(point);
			this.CurrentCanvasPoint = point;
		}
		this.PdfCanvas.Ellipse(sx, sy, ex, ey).FillStroke();	// x(left),y(bottom),width,height
	}

//
// 機能：楕円を塗りつぶす
//
// 引数：sx 開始横位置（半角文字単位）
//       sy 開始縦位置（行単位）
//       ex 終了横位置（半角文字単位）
//       ey 終了縦位置（行単位）
//       point 線の太さ（ポイント）
//       ※原点は用紙の左上
//
// 例：pdf.fillOval(2.0f, 3.0f, 120.0f, 3.0f, 12.0f); // (2.0f, 3.0f)-(120.0f, 3.0f)を対角とする楕円を
//                                                       12.0fポイントの太さの線で描き塗りつぶす
//
	public void fillOval(float sx, float sy, float ex, float ey, float point){
		this.fillOvalByPoint(c2p(sx), l2p(sy), c2p(ex), l2p(ey), point);
	}

//
// 機能：描画する画像ファイルを設定する
//
// 引数：imagefile 画像ファイル名
//
// 例：pdf.setImage(@"image.jpg");
//
	public void setImage(string imagefile){
		this.ImageData = ImageDataFactory.Create(imagefile);
		this.Image = new Image(this.ImageData);
	}

//
// 機能：画像を描画する
//
// 引数：sx 開始横位置（ポイント）
//       sy 開始縦位置（ポイント）
//       ex 終了横位置（ポイント）
//       ey 終了縦位置（ポイント）
//       ※原点は用紙の左下
//       ※本メソッドはsetXbias(), setYbias()の影響を受けない
//
// 例：pdf.showImageByPoint(2.0f, 3.0f, 120.0f, 3.0f); // (2.0f, 3.0f)-(120.0f, 3.0f)を対角とする矩形状に
//                                                        画像を表示する
//
	public void showImageByPoint(float sx, float sy, float ex, float ey){
		float x, y, w, h;
		if (sx < ex){
			x = sx;
			w = ex - sx;
		}else{
			x = ex;
			w = sx - ex;
		}
		if (sy < ey){
			y = sy;
			h = ey - sy;
		}else{
			y = ey;
			h = sy - ey;
		}
		this.Image.ScaleAbsolute(w, h).SetFixedPosition(this.Page, x, y);
		this.Document.Add(this.Image);
	}

//
// 機能：画像を描画する
//
// 引数：sx 開始横位置（半角文字単位）
//       sy 開始縦位置（行単位）
//       ex 終了横位置（半角文字単位）
//       ey 終了縦位置（行単位）
//       ※原点は用紙の左上
//
// 例：pdf.showImage(2.0f, 3.0f, 120.0f, 3.0f); // (2.0f, 3.0f)-(120.0f, 3.0f)を対角とする矩形状に
//                                                 画像を表示する
//
	public void showImage(float sx, float sy, float ex, float ey){
		showImageByPoint(c2p(sx), l2p(sy), c2p(ex), l2p(ey));
	}

//
// 機能：指定した矩形領域内の中央にぴったり収まるように画像を描画する
//
// 引数：sx 開始横位置（半角文字単位）
//       sy 開始縦位置（行単位）
//       ex 終了横位置（半角文字単位）
//       ey 終了縦位置（行単位）
//       ※原点は用紙の左上
//
// 例：pdf.showImageInBox(2.0f, 3.0f, 12.0f, 13.0f); // (2.0f,3.0f)-(12.0f,13.0f)を対角とする矩形領域に
//                                                      画像を表示する
//
	public void showImageInBox(float sx, float sy, float ex, float ey){
		float w, h, iw, ih, w2, h2;
		if (sx < ex){
			w = ex - sx;
		}else{
			w = sx - ex;
		}
		if (sy < ey){
			h = ey - sy;
		}else{
			h = sy - ey;
		}
		iw = this.Image.GetImageWidth();	// dot
		ih = this.Image.GetImageHeight();	// dot
		if ((iw / ih) > ((w * this.XUNIT) / (h * this.YUNIT))){
			h2 = w * this.XUNIT * ih / iw / this.YUNIT;
			if (sy < ey){
				sy = sy + (h - h2) / 2;
				ey = ey - (h - h2) / 2;
			}else{
				sy = sy - (h - h2) / 2;
				ey = ey + (h - h2) / 2;
			}
		}else{
			w2 = h * this.YUNIT * iw / ih / this.XUNIT;
			if (sx < ex){
				sx = sx + (w - w2) / 2;
				ex = ex - (w - w2) / 2;
			}else{
				sx = sx - (w - w2) / 2;
				ex = ex + (w - w2) / 2;
			}
		}
		showImageByPoint(c2p(sx), l2p(sy), c2p(ex), l2p(ey));
	}

//
// 機能：描画するQRCode画像ファイルを設定する
//
// 引数：str QRCodeに変換する文字列
//
// 例：pdf.setQRCode("http://www.foo.com/"); // 文字列「http://www.foo.com/」
//                                              のQRCode画像を生成し設定する
//
	public void setQRCode(string str){
		this.BarcodeQRCode = new BarcodeQRCode(str);
		this.PdfFormXObject = BarcodeQRCode.CreateFormXObject(this.Color, this.PdfDocument);
		this.ImageBarcode = new Image(this.PdfFormXObject);
	}

//
// 機能：QRCode画像を描画する
//
// 引数：sx 開始横位置（ポイント）
//       sy 開始縦位置（ポイント）
//       ex 終了横位置（ポイント）
//       ey 終了縦位置（ポイント）
//       ※原点は用紙の左下
//
// 例：pdf.showQRCodeByPoint(2.0f, 3.0f, 120.0f, 3.0f); // (2.0f, 3.0f)-(120.0f, 3.0f)を対角とする矩形状に
//                                                         QRCode画像を表示する
//
	public void showQRCodeByPoint(float sx, float sy, float ex, float ey){
		if (this.Image == null){
			this.Image = this.ImageBarcode;
			showImageByPoint(sx, sy, ex, ey);
		}else{
			Image imagesave = this.Image;
			this.Image = this.ImageBarcode;
			showImageByPoint(sx, sy, ex, ey);
			this.Image = imagesave;
		}
	}

//
// 機能：QRCode画像を描画する
//
// 引数：sx 開始横位置（半角文字単位）
//       sy 開始縦位置（行単位）
//       ex 終了横位置（半角文字単位）
//       ey 終了縦位置（行単位）
//       ※原点は用紙の左上
//
// 例：pdf.showQRCode(2.0f, 3.0f, 120.0f, 3.0f); // (2.0f, 3.0f)-(120.0f, 3.0f)を対角とする矩形状に
//                                                  QRCode画像を表示する
//
	public void showQRCode(float sx, float sy, float ex, float ey){
		if (this.Image == null){
			this.Image = this.ImageBarcode;
			showImageByPoint(c2p(sx), l2p(sy), c2p(ex), l2p(ey));
		}else{
			Image imagesave = this.Image;
			this.Image = this.ImageBarcode;
			showImageByPoint(c2p(sx), l2p(sy), c2p(ex), l2p(ey));
			this.Image = imagesave;
		}
	}

//
// 機能：指定した矩形領域内の中央にぴったり収まるようにQRCode画像を描画する
//
// 引数：sx 開始横位置（半角文字単位）
//       sy 開始縦位置（行単位）
//       ex 終了横位置（半角文字単位）
//       ey 終了縦位置（行単位）
//       ※原点は用紙の左上
//
// 例：pdf.showQRCodeInBox(2.0f, 3.0f, 12.0f, 13.0f); // (2.0f,3.0f)-(12.0f,13.0f)を対角とする矩形領域に
//                                                       QRCode画像を表示する
//
	public void showQRCodeInBox(float sx, float sy, float ex, float ey){
		if (this.Image == null){
			this.Image = this.ImageBarcode;
			showImageInBox(sx, sy, ex, ey);
		}else{
			Image imagesave = this.Image;
			this.Image = this.ImageBarcode;
			showImageInBox(sx, sy, ex, ey);
			this.Image = imagesave;
		}
	}
}
