# 0. Mode
# 1. LastWriteTime
# 2. Length
# 3. Name

$oe = $OutputEncoding # Backup output encoding for pipe.
$scoe = [System.Console]::OutputEncoding      # Backup output encoding for console.

$OutputEncoding = [System.Console]::OutputEncoding = New-Object System.Text.UTF8Encoding $true        # UTF-8 with BOM

# copy-item C:\Windows\Fonts\eudc.tte .\eudc.ttf
&{
	echo "PT	A4H"
	echo "CPI	10"
	echo "LPI	6"
	echo "XB	3"
	echo "YB	3.3"
	echo "FN	C:\Windows\Fonts\msmincho.ttc,0"
#	echo "FP	.\eudc.ttf"

	echo "NP"
	echo "PO	1.0"
	echo "DB	0	2	110	44"
	for ($line = 4; $line -lt 44; $line += 2){
		echo "DL	0	$line	110	$line"
	}
	echo "DL	10	2	10	44"
	echo "DL	32	2	32	44"
	echo "DL	45	2	45	44"

	echo "PO	14.4"
	echo "LN	2"
	echo "ST	40	フ　ァ　イ　ル　一　覧　表       作成日：            ページ："
	echo "LN	4"
	echo "ST	1	Mode"
	echo "ST	11	LastWriteTime"
	echo "ST	33	Length"
	echo "ST	46	Name"
	echo "OP	0.2"
	echo "PO	1.0"
	echo "FB	0	2	110	4"

} |`
.\MakePdf.exe -i - -e UTF-8 -o sample_filelist_template.pdf

get-item * |`
foreach{
	($_.Mode + "," + $_.LastWriteTime + "," + $_.Length + "," + $_.Name)
} |`
&{
	begin{
		echo "PT	A4H"
		echo "CPI	10"
		echo "LPI	6"
		echo "XB	3"
		echo "YB	3.3"
		echo "PO	14.4"
		echo "FN	C:\Windows\Fonts\msmincho.ttc,0"
#		echo "FP	.\eudc.ttf"

		$page = 0
		$line = 0
	}
	process{
		$array = $_ -split ","
		$line++
		if ($page -eq 0 -or $line -gt 20){
			$page++
			$line = 1
			echo "NP"
			$day = get-date -uformat "%Y/%m/%d"
			echo "LN	2"
			echo "ST	81	${day}"
			echo "ST	101	${page}"
		}
		$showtext_line = 4 + (2 * $line) 
		echo "LN	${showtext_line}"
		echo ("ST	1	" + $array[0])
		echo ("ST	11	" + $array[1])
		echo ("ST	33	" + $array[2])
		echo ("ST	46	" + $array[3])
	}
	end{
	}
} |`
.\MakePdf.exe -i - -e UTF-8 -t sample_filelist_template.pdf -o sample_filelist.pdf

# remove-item .\eudc.ttf

$OutputEncoding = $oe # Restore output encoding for pipe.
[System.Console]::OutputEncoding = $scoe      # Restore output encoding for console.
