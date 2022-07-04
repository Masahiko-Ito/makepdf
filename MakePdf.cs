//
// PDF作成ツール（iTextSharp7使用につき、無償利用の場合はソース公開義務有り。注意！）
//
// Author. "Masahiko Ito"<m-ito@myh.no-ip.org>
//
// Compile: c:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe /r:MyPackage.dll;Pdf.dll MakePdf.cs
//
// Run: MakePdf.exe -i input.txt -e UTF-8 -t template.pdf -o output.pdf
//
using System;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using System.Text;

public class MakePdf{
	static void Main(string[] args){
		string Input = "-", Encoding = "UTF-8", Template = null, Output = "output.pdf";
		ITextIO inf = null;
		string inrec;
		string[] array;
		Pdf pdf = null;
		float Line = 0, Point = 14.4f;
		int i, SpecifiedPage = 0, Page = 0;

		for (i = 0; i < args.Length; i++){
			if (args[i].CompareTo("-v") == 0){
				i++;
				SpecifiedPage = int.Parse(args[i]);
			}else if (args[i].CompareTo("-i") == 0){
				i++;
				Input = args[i];
			}else if (args[i].CompareTo("-e") == 0){
				i++;
				Encoding = args[i];
			}else if (args[i].CompareTo("-t") == 0){
				i++;
				Template = args[i];
			}else if (args[i].CompareTo("-o") == 0){
				i++;
				Output = args[i];
			}else{
				Console.WriteLine("Usage : MakePdf.exe [-v page] [-i input.txt] [-e input_encoding] [-t template.pdf] [-o output.pdf]");
				Console.WriteLine("PDF generating tool.");
				Console.WriteLine("");
				Console.WriteLine("  -v page            Verbose page counter per specified page.");
				Console.WriteLine("  -i input.txt       default - (- means stdin)");
				Console.WriteLine("  -e input_encoding  default UTF-8");
				Console.WriteLine("                     UTF-8        utf-8 with bom.");
				Console.WriteLine("                     UTF-8N       utf-8 without bom.");
				Console.WriteLine("                     UTF-16       utf-16 little endian with bom.");
				Console.WriteLine("                     UTF-16N      utf-16 little endian without bom.");
				Console.WriteLine("                     UTF-16BE     utf-16 big endian with bom.");
				Console.WriteLine("                     UTF-16BEN    utf-16 big endian without bom.");
				Console.WriteLine("                     SHIFT_JIS    obsolete :P.");
				Console.WriteLine("  -t template.pdf    default not specified");
				Console.WriteLine("  -o output.pdf      default output.pdf");
				Console.WriteLine("");
				Console.WriteLine("Format of input.txt.");
				Console.WriteLine("");
				Console.WriteLine("  Separator must be \"\\t(TAB)\".");
				Console.WriteLine("  Line starts with \"#\" and empty line are ignored.");
				Console.WriteLine("");
				Console.WriteLine("  [PaperType|PT] PAPER_TYPE");
				Console.WriteLine("      PAPER_TYPE must be A4V, A4H, A3V, A3H.");
				Console.WriteLine("  [PaperType|PT] WIDTH HEIGHT");
				Console.WriteLine("      WIDTH and HEIGTH must be in mm.");
				Console.WriteLine("  [CharPerInch|CPI] CPI");
				Console.WriteLine("      CPI is chars in a inch (default 10).");
				Console.WriteLine("  [LinePerInch|LPI] LPI");
				Console.WriteLine("      LPI is lines in a inch (default 6).");
				Console.WriteLine("  [XBias|XB] XBIAS");
				Console.WriteLine("      XBIAS must be in chars (default 0).");
				Console.WriteLine("  [YBias|YB] YBIAS");
				Console.WriteLine("      YBIAS must be in lines (default 0).");
				Console.WriteLine("  [FontNormal|FN] FONT_PATH");
				Console.WriteLine("      default \"C:\\Windows\\Fonts\\msgothic.ttc,0\".");
				Console.WriteLine("  [FontPua|FP] FONT_PATH_PUA");
				Console.WriteLine("      default \"C:\\Windows\\Fonts\\msgothic.ttc,0\".");
				Console.WriteLine("      If you want to specify \"C:\\Windows\\Fonts\\EUDC.TTE\", you have to copy \"EUDC.TTE\" to \"EUDC.TTF\" and use it.");
				Console.WriteLine("  [Template|TP] TEMPLATE");
				Console.WriteLine("      TEMPLATE must be PDF (default not specified).");
//				Console.WriteLine("  [Open|OP]");
//				Console.WriteLine("  [Close|CL]");
				Console.WriteLine("  [NewPage|NP]");
				Console.WriteLine("  [Line|LN] LINE_TO_GO_FOR_SHOWTEXT");
				Console.WriteLine("      LINE_TO_GO_FOR_SHOWTEXT must be in lines.");
				Console.WriteLine("      LINE_TO_GO_FOR_SHOWTEXT is pointed at bottom of text.");
				Console.WriteLine("  [Point|PO] POINT");
				Console.WriteLine("      POINT is for width of japanese character and graphical line (default 14.4).");
				Console.WriteLine("      One point means 1/72 inch.");
				Console.WriteLine("  [Color|CO] COLOR OPACITY");
				Console.WriteLine("      COLOR must be BLACK, RED, GREEN, YELLOW, BLUE, MAGENTA, CYAN, WHITE (default BLACK).");
				Console.WriteLine("      OPACITY must be 0.0f to 1.0f (default 1.0f).");
				Console.WriteLine("  [Opacity|OP] OPACITY");
				Console.WriteLine("      OPACITY must be 0.0f to 1.0f (default 1.0f).");
				Console.WriteLine("  [ShowText|ST] COLUMN STRING");
				Console.WriteLine("      COLUMN must be in chars.");
				Console.WriteLine("      IVS/IVD characters can not be handled.");
				Console.WriteLine("  [DrawLine|DL] START_COLUMN START_LINE END_COLUMN END_LINE");
				Console.WriteLine("      COLUMN must be in chars.");
				Console.WriteLine("      LINE must be in lines.");
				Console.WriteLine("  [DrawBox|DB] START_COLUMN START_LINE END_COLUMN END_LINE");
				Console.WriteLine("      COLUMN must be in chars.");
				Console.WriteLine("      LINE must be in lines.");
				Console.WriteLine("  [FillBox|FB] START_COLUMN START_LINE END_COLUMN END_LINE");
				Console.WriteLine("      COLUMN must be in chars.");
				Console.WriteLine("      LINE must be in lines.");
				Console.WriteLine("  [DrawOval|DO] START_COLUMN START_LINE END_COLUMN END_LINE");
				Console.WriteLine("      COLUMN must be in chars.");
				Console.WriteLine("      LINE must be in lines.");
				Console.WriteLine("  [FillOval|FO] START_COLUMN START_LINE END_COLUMN END_LINE");
				Console.WriteLine("      COLUMN must be in chars.");
				Console.WriteLine("      LINE must be in lines.");
				Console.WriteLine("  [SetImage|SI] IMAGE_FILE_PATH");
				Console.WriteLine("  [DrawImage|DI] START_COLUMN START_LINE END_COLUMN END_LINE");
				Console.WriteLine("      COLUMN must be in chars.");
				Console.WriteLine("      LINE must be in lines.");
				Console.WriteLine("  [SetQRCode|SQ] STRING");
				Console.WriteLine("  [DrawQRCode|DQ] START_COLUMN START_LINE END_COLUMN END_LINE");
				Console.WriteLine("      COLUMN must be in chars.");
				Console.WriteLine("      LINE must be in lines.");
				Console.WriteLine("");
				Console.WriteLine("Misc.");
				Console.WriteLine("");
				Console.WriteLine("When MakePdf.exe read stdin in Powershell environment, do like next.");
				Console.WriteLine("  $oe = $OutputEncoding	# Backup output encoding for pipe.");
				Console.WriteLine("  $scoe = [System.Console]::OutputEncoding	# Backup output encoding for console.");
				Console.WriteLine("");
				Console.WriteLine("  $OutputEncoding = [System.Console]::OutputEncoding = New-Object System.Text.UTF8Encoding $true	# UTF-8 with BOM");
				Console.WriteLine("");
				Console.WriteLine("  get-content input.txt | MakePdf.exe -i - -e UTF-8 -o output.pdf");
				Console.WriteLine("");
				Console.WriteLine("  $OutputEncoding = $oe	# Restore output encoding for pipe.");
				Console.WriteLine("  [System.Console]::OutputEncoding = $scoe	# Restore output encoding for console.");
				Environment.Exit(1);
			}
		}

		if (Input.CompareTo("-") == 0){
			inf = new TextStdio();
			inf.setEncoding(Encoding);
			inf.openInput();
		}else{
			inf = new TextFile();
			inf.setEncoding(Encoding);
			((TextFile)inf).openInput(Input);
		}
		while ((inrec = inf.read()) != null){
			if (Regex.IsMatch(inrec, "^#") || Regex.IsMatch(inrec, "^ *$")){
				continue;
			}
			array = Regex.Split(inrec, "\t");
			if (array.Length > 0){
				if (array[0].CompareTo("PaperType") == 0 || array[0].CompareTo("PT") == 0){
					if (array.Length == 2){
						pdf = new Pdf(array[1], Output);
					}else if (array.Length == 3){
						pdf = new Pdf(float.Parse(array[1]), float.Parse(array[2]), Output);
					}else{
						Console.WriteLine("Error input:" + inrec);
						Environment.Exit(1);
					}
					if (Template != null){
						pdf.setTemplate(Template);
					}
				}else if(array[0].CompareTo("FontNormal") == 0 || array[0].CompareTo("FN") == 0){
					if (array.Length == 2){
						pdf.setFont(array[1]);
					}else{
						Console.WriteLine("Error input:" + inrec);
						closeAndExit(inf, pdf, 1);
					}
				}else if(array[0].CompareTo("FontPua") == 0 || array[0].CompareTo("FP") == 0){
					if (array.Length == 2){
						pdf.setPuaFont(array[1]);
					}else{
						Console.WriteLine("Error input:" + inrec);
						closeAndExit(inf, pdf, 1);
					}
				}else if(array[0].CompareTo("XBias") == 0 || array[0].CompareTo("XB") == 0){
					if (array.Length == 2){
						pdf.setXbias(float.Parse(array[1]));
					}else{
						Console.WriteLine("Error input:" + inrec);
						closeAndExit(inf, pdf, 1);
					}
				}else if(array[0].CompareTo("YBias") == 0 || array[0].CompareTo("YB") == 0){
					if (array.Length == 2){
						pdf.setYbias(float.Parse(array[1]));
					}else{
						Console.WriteLine("Error input:" + inrec);
						closeAndExit(inf, pdf, 1);
					}
				}else if(array[0].CompareTo("CharPerInch") == 0 || array[0].CompareTo("CPI") == 0){
					if (array.Length == 2){
						pdf.setCpi(float.Parse(array[1]));
					}else{
						Console.WriteLine("Error input:" + inrec);
						closeAndExit(inf, pdf, 1);
					}
				}else if(array[0].CompareTo("LinePerInch") == 0 || array[0].CompareTo("LPI") == 0){
					if (array.Length == 2){
						pdf.setLpi(float.Parse(array[1]));
					}else{
						Console.WriteLine("Error input:" + inrec);
						closeAndExit(inf, pdf, 1);
					}
//				}else if(array[0].CompareTo("Open") == 0 || array[0].CompareTo("OP") == 0){
//					if (array.Length == 1){
//						pdf.open();
//					}else{
//						Console.WriteLine("Error input:" + inrec);
//						Environment.Exit(1);
//					}
//				}else if(array[0].CompareTo("Close") == 0 || array[0].CompareTo("CL") == 0){
//					if (array.Length == 1){
//						pdf.close();
//						pdf = null;
//					}else{
//						Console.WriteLine("Error input:" + inrec);
//						Environment.Exit(1);
//					}
				}else if(array[0].CompareTo("Template") == 0 || array[0].CompareTo("TP") == 0){
					if (array.Length == 1){
						pdf.unsetTemplate();
					}else if (array.Length == 2){
						pdf.setTemplate(array[1]);
					}else{
						Console.WriteLine("Error input:" + inrec);
						closeAndExit(inf, pdf, 1);
					}
				}else if(array[0].CompareTo("NewPage") == 0 || array[0].CompareTo("NP") == 0){
					if (array.Length == 1){
						pdf.newPage();
						if (SpecifiedPage > 0){
							Page++;
							if ((Page % SpecifiedPage) == 0){
								Console.WriteLine("Processed page : " + Page);
							}
						}
					}else{
						Console.WriteLine("Error input:" + inrec);
						closeAndExit(inf, pdf, 1);
					}
				}else if(array[0].CompareTo("Line") == 0 || array[0].CompareTo("LN") == 0){
					if (array.Length == 2){
						Line = float.Parse(array[1]);
					}else{
						Console.WriteLine("Error input:" + inrec);
						closeAndExit(inf, pdf, 1);
					}
				}else if(array[0].CompareTo("Point") == 0 || array[0].CompareTo("PO") == 0){
					if (array.Length == 2){
						Point = float.Parse(array[1]);
					}else{
						Console.WriteLine("Error input:" + inrec);
						closeAndExit(inf, pdf, 1);
					}
				}else if(array[0].CompareTo("Color") == 0 || array[0].CompareTo("CO") == 0){
					if (array.Length == 3){
						pdf.setColor(array[1]);
						pdf.setOpacity(float.Parse(array[2]));
					}else{
						Console.WriteLine("Error input:" + inrec);
						closeAndExit(inf, pdf, 1);
					}
				}else if(array[0].CompareTo("Opacity") == 0 || array[0].CompareTo("OP") == 0){
					if (array.Length == 2){
						pdf.setOpacity(float.Parse(array[1]));
					}else{
						Console.WriteLine("Error input:" + inrec);
						closeAndExit(inf, pdf, 1);
					}
				}else if(array[0].CompareTo("ShowText") == 0 || array[0].CompareTo("ST") == 0){
					if (array.Length == 3){
						pdf.showText(float.Parse(array[1]), Line, array[2], Point);
					}else{
						Console.WriteLine("Error input:" + inrec);
						closeAndExit(inf, pdf, 1);
					}
				}else if(array[0].CompareTo("DrawLine") == 0 || array[0].CompareTo("DL") == 0){
					if (array.Length == 5){
						pdf.drawLine(float.Parse(array[1]), float.Parse(array[2]), float.Parse(array[3]), float.Parse(array[4]), Point);
					}else{
						Console.WriteLine("Error input:" + inrec);
						closeAndExit(inf, pdf, 1);
					}
				}else if(array[0].CompareTo("DrawBox") == 0 || array[0].CompareTo("DB") == 0){
					if (array.Length == 5){
						pdf.drawRect(float.Parse(array[1]), float.Parse(array[2]), float.Parse(array[3]), float.Parse(array[4]), Point);
					}else{
						Console.WriteLine("Error input:" + inrec);
						closeAndExit(inf, pdf, 1);
					}
				}else if(array[0].CompareTo("FillBox") == 0 || array[0].CompareTo("FB") == 0){
					if (array.Length == 5){
						pdf.fillRect(float.Parse(array[1]), float.Parse(array[2]), float.Parse(array[3]), float.Parse(array[4]), Point);
					}else{
						Console.WriteLine("Error input:" + inrec);
						closeAndExit(inf, pdf, 1);
					}
				}else if(array[0].CompareTo("DrawOval") == 0 || array[0].CompareTo("DO") == 0){
					if (array.Length == 5){
						pdf.drawOval(float.Parse(array[1]), float.Parse(array[2]), float.Parse(array[3]), float.Parse(array[4]), Point);
					}else{
						Console.WriteLine("Error input:" + inrec);
						closeAndExit(inf, pdf, 1);
					}
				}else if(array[0].CompareTo("FillOval") == 0 || array[0].CompareTo("FO") == 0){
					if (array.Length == 5){
						pdf.fillOval(float.Parse(array[1]), float.Parse(array[2]), float.Parse(array[3]), float.Parse(array[4]), Point);
					}else{
						Console.WriteLine("Error input:" + inrec);
						closeAndExit(inf, pdf, 1);
					}
				}else if(array[0].CompareTo("SetImage") == 0 || array[0].CompareTo("SI") == 0){
					if (array.Length == 2){
						pdf.setImage(array[1]);
					}else{
						Console.WriteLine("Error input:" + inrec);
						closeAndExit(inf, pdf, 1);
					}
				}else if(array[0].CompareTo("DrawImage") == 0 || array[0].CompareTo("DI") == 0){
					if (array.Length == 5){
						pdf.showImageInBox(float.Parse(array[1]), float.Parse(array[2]), float.Parse(array[3]), float.Parse(array[4]));
					}else{
						Console.WriteLine("Error input:" + inrec);
						closeAndExit(inf, pdf, 1);
					}
				}else if(array[0].CompareTo("SetQRCode") == 0 || array[0].CompareTo("SQ") == 0){
					if (array.Length == 2){
						pdf.setQRCode(array[1]);
					}else{
						Console.WriteLine("Error input:" + inrec);
						closeAndExit(inf, pdf, 1);
					}
				}else if(array[0].CompareTo("DrawQRCode") == 0 || array[0].CompareTo("DQ") == 0){
					if (array.Length == 5){
						pdf.showQRCodeInBox(float.Parse(array[1]), float.Parse(array[2]), float.Parse(array[3]), float.Parse(array[4]));
					}else{
						Console.WriteLine("Error input:" + inrec);
						closeAndExit(inf, pdf, 1);
					}
				}else{
					Console.WriteLine("Error input:" + inrec);
					closeAndExit(inf, pdf, 1);
				}
			}else{
				Console.WriteLine("Error input:" + inrec);
				closeAndExit(inf, pdf, 1);
			}
		}
		closeAndExit(inf, pdf, 0);
	}

	static void closeAndExit(ITextIO inf, Pdf pdf, int status){
		if (inf != null){
			inf.close();
		}
		if (pdf != null){
			pdf.close();
		}
		Environment.Exit(status);
	}
}
