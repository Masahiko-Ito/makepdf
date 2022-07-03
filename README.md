# MakePdf - PDF作成ツール with iText7
** MakePdf **は、PDFを生成するための簡易なコマンドを記述したファイル（
もしくは標準入力）を解釈しPDFを生成します。** MakePdf **はPDFの生成エン
ジンとして** iText7 **を使用します。またビルドは** csc.exe **のみで行い
ます。

## 使い方
ヘルプ表示を以下に示します。
```
>MakePdf -h
Usage : MakePdf.exe [-v page] [-i input.txt] [-e input_encoding] [-t template.pdf] [-o output.pdf]
PDF generating tool.

  -v page            Verbose page counter per specified page.
  -i input.txt       default - (- means stdin)
  -e input_encoding  default UTF-8
                     UTF-8        utf-8 with bom.
                     UTF-8N       utf-8 without bom.
                     UTF-16       utf-16 little endian with bom.
                     UTF-16N      utf-16 little endian without bom.
                     UTF-16BE     utf-16 big endian with bom.
                     UTF-16BEN    utf-16 big endian without bom.
                     SHIFT_JIS    obsolete :P.
  -t template.pdf    default not specified
  -o output.pdf      default output.pdf

Format of input.txt.

  Separator must be "\t(TAB)".
  Line starts with "#" and empty line are ignored.

  [PaperType|PT] PAPER_TYPE
      PAPER_TYPE must be A4V, A4H, A3V, A3H.
  [PaperType|PT] WIDTH HEIGHT
      WIDTH and HEIGTH must be in mm.
  [CharPerInch|CPI] CPI
      CPI is chars in a inch (default 10).
  [LinePerInch|LPI] LPI
      LPI is lines in a inch (default 6).
  [XBias|XB] XBIAS
      XBIAS must be in chars (default 0).
  [YBias|YB] YBIAS
      YBIAS must be in lines (default 0).
  [FontNormal|FN] FONT_PATH
      default "C:\Windows\Fonts\msgothic.ttc,0".
  [FontPua|FP] FONT_PATH_PUA
      FONT_PATH_PUA must have .TTF for extention.
      default "C:\Windows\Fonts\EUDC.TTE", but extention is changed .TTF.
  [Template|TP] TEMPLATE
      TEMPLATE must be PDF (default not specified).
  [NewPage|NP]
  [Line|LN] LINE_TO_GO_FOR_SHOWTEXT
      LINE_TO_GO_FOR_SHOWTEXT must be in lines.
      LINE_TO_GO_FOR_SHOWTEXT is pointed at bottom of text.
  [Point|PO] POINT
      POINT is for width of japanese character and graphical line (default 14.4).
      One point means 1/72 inch.
  [Color|CO] COLOR OPACITY
      COLOR must be BLACK, RED, GREEN, YELLOW, BLUE, MAGENTA, CYAN, WHITE (default BLACK).
      OPACITY must be 0.0f to 1.0f (default 1.0f).
  [Opacity|OP] OPACITY
      OPACITY must be 0.0f to 1.0f (default 1.0f).
  [ShowText|ST] COLUMN STRING
      COLUMN must be in chars.
      IVS/IVD characters can not be handled.
  [DrawLine|DL] START_COLUMN START_LINE END_COLUMN END_LINE
      COLUMN must be in chars.
      LINE must be in lines.
  [DrawBox|DB] START_COLUMN START_LINE END_COLUMN END_LINE
      COLUMN must be in chars.
      LINE must be in lines.
  [FillBox|FB] START_COLUMN START_LINE END_COLUMN END_LINE
      COLUMN must be in chars.
      LINE must be in lines.
  [DrawOval|DO] START_COLUMN START_LINE END_COLUMN END_LINE
      COLUMN must be in chars.
      LINE must be in lines.
  [FillOval|FO] START_COLUMN START_LINE END_COLUMN END_LINE
      COLUMN must be in chars.
      LINE must be in lines.
  [SetImage|SI] IMAGE_FILE_PATH
  [DrawImage|DI] START_COLUMN START_LINE END_COLUMN END_LINE
      COLUMN must be in chars.
      LINE must be in lines.
  [SetQRCode|SQ] STRING
  [DrawQRCode|DQ] START_COLUMN START_LINE END_COLUMN END_LINE
      COLUMN must be in chars.
      LINE must be in lines.

Misc.

When MakePdf.exe read stdin in Powershell environment, do like next.
  $oe = $OutputEncoding # Backup output encoding for pipe.
  $scoe = [System.Console]::OutputEncoding      # Backup output encoding for console.

  $OutputEncoding = [System.Console]::OutputEncoding = New-Object System.Text.UTF8Encoding $true        # UTF-8 with BOM

  get-content input.txt | MakePdf.exe -i - -e UTF-8 -o output.pdf

  $OutputEncoding = $oe # Restore output encoding for pipe.
  [System.Console]::OutputEncoding = $scoe      # Restore output encoding for console.
```
基本的に** iText7 **のインタフェースはページの左下が原点で、座標はポイ
ントで指定するのですが、** MakePdf **ではページの右上が原点で、座標の
横方向はカラム数、縦方向は行数で指定します。COBOL等の事務処理用言語で
帳票を開発していた方には馴染み易いのではないかと思います。

## 必要なDLL
ダウンロードした** .nuget **の拡張子を** .zip **に変えて、** .dll **を
取り出します（すいません、私は** .nuget **の本来の使い方を理解してませ
ん...）。
* [bouncycastle.1.8.9.nupkg](https://xxxxxxxx/)
** BouncyCastle.Crypto.dll
* [itext7.7.2.2.nupkg](https://xxxxxxxx/)
** itext.barcodes.dll
** itext.forms.dll
** itext.io.dll
** itext.kernel.dll
** itext.layout.dll
** itext.pdfa.dll
** itext.sign.dll
** itext.styledxmlparser.dll
** itext.svg.dll
* [itext7.commons.7.2.2.nupkg](https://xxxxxxxx/)
** itext.commons.dll
* [microsoft.extensions.logging.5.0.0.nupkg](https://xxxxxxxx/)
** Microsoft.Extensions.Logging.dll
* [microsoft.extensions.logging.abstractions.5.0.0.nupkg](https://xxxxxxxx/)
** Microsoft.Extensions.Logging.Abstractions.dll
* [microsoft.extensions.options.5.0.0.nupkg](https://xxxxxxxx/)
** Microsoft.Extensions.Options.dll

## ビルド手順
** csc.exe **のパスは、ご自身の環境に合わせて読み替えてください。
```
>c:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe /t:library MyPackage.cs
```
Oracle関係のワーニングが出ますが、今回のビルドには直接関係の無い部分
なので、とりあえず無視してください...
```
>c:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe /t:library /r:itext.commons.dll;itext.barcodes.dll;itext.forms.dll;itext.io.dll;itext.kernel.dll;itext.layout.dll;itext.pdfa.dll;itext.sign.dll;itext.styledxmlparser.dll;itext.svg.dll;BouncyCastle.Crypto.dll;Microsoft.Extensions.Logging.Abstractions.dll;Microsoft.Extensions.Logging.dll;Microsoft.Extensions.Options.dll Pdf.cs
```
```
>c:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe /r:MyPackage.dll;Pdf.dll MakePdf.cs
```

## サンプルPDF（機能単位）の生成
** MakePdf **のヘルプ表示と、以下のサンプルをご覧いただければ、使い方
は容易に理解いただけると思います。
```
>MakePdf.exe -i sample_input_showtext.txt -e UTF-8 -o sample_input_showtext.pdf
```
```
>MakePdf.exe -i sample_input_drawline.txt -e UTF-8 -o sample_input_drawline.pdf
```
```
>MakePdf.exe -i sample_input_drawbox.txt -e UTF-8 -o sample_input_drawbox.pdf
```
```
>MakePdf.exe -i sample_input_fillbox.txt -e UTF-8 -o sample_input_fillbox.pdf
```
```
>MakePdf.exe -i sample_input_drawoval.txt -e UTF-8 -o sample_input_drawoval.pdf
```
```
>MakePdf.exe -i sample_input_filloval.txt -e UTF-8 -o sample_input_filloval.pdf
```
```
>MakePdf.exe -i sample_input_drawimage.txt -e UTF-8 -o sample_input_drawimage.pdf
```
```
>MakePdf.exe -i sample_input_drawqrcode.txt -e UTF-8 -o sample_input_drawqrcode.pdf
```

## サンプルPDF（ファイル一覧表）の生成
```
>powershell -ex bypass -f .\sample_filelist.ps1
```
