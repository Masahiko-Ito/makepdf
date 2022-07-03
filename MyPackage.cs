//
// 汎用クラスもろもろ
//
// Author. "Masahiko Ito"<m-ito@myh.no-ip.org>
//
// Compile: c:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe /t:library MyPackage.cs
//
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Collections;

// ============================================================
// 汎用テキストファイル入出力クラス
// ============================================================
public class TextFile : ITextIO{
	IRecord ir;
	string filename;
	string encoding;
	Encoding objEncoding;
	StreamReader sr;
	StreamWriter sw;

//
// 初期化（引数無し）
//
	public TextFile(){
		this.ir = null;
		initCommon();
	}

//
// 初期化（引数：レコードオブジェクト（レコードインタフェースを継承））
//
	public TextFile(IRecord ir){
		this.ir = ir;
		initCommon();
	}

//
// 初期化共通処理
//
	public void initCommon(){
		this.filename = "";
		setEncoding("UTF-8N");
		this.sr = null;
		this.sw = null;
	}

//
// レコードオブジェクト設定（引数：レコードオブジェクト（レコードインタフェースを継承））
//
	public void setRecord(IRecord ir){
		this.ir = ir;
	}

//
// レコードオブジェクト取得（引数無し）
//
	public IRecord getRecord(){
		return this.ir;
	}

//
// ファイル名設定（引数：ファイル名）
//
	public void setFilename(string filename){
		this.filename = filename;
	}

//
// ファイル名取得（引数無し）
//
	public string getFilename(){
		return this.filename;
	}

//
// エンコーディング設定（引数：エンコーディング文字列）
// "UTF-8" | "UTF-8N" | "UTF-16" | "UTF-16N" | "UTF-16BE" | ""UTF-16BEN"
//
	public void setEncoding(string encoding){
		bool bom;
		bool bigendian;
		this.encoding = encoding;
		if (Regex.IsMatch(encoding, "^UTF-8N$")){
			bom = false;
			this.objEncoding = new UTF8Encoding(bom);
		}else if (Regex.IsMatch(encoding, "^UTF-8$")){
			bom = true;
			this.objEncoding = new UTF8Encoding(bom);
		}else if (Regex.IsMatch(encoding, "^UTF-16N$") || Regex.IsMatch(encoding, "^UTF-16LEN$")){
			bigendian = false;
			bom = false;
			this.objEncoding = new UnicodeEncoding(bigendian, bom);
		}else if (Regex.IsMatch(encoding, "^UTF-16$") || Regex.IsMatch(encoding, "^UTF-16LE$")){
			bigendian = false;
			bom = true;
			this.objEncoding = new UnicodeEncoding(bigendian, bom);
		}else if (Regex.IsMatch(encoding, "^UTF-16BEN$")){
			bigendian = true;
			bom = false;
			this.objEncoding = new UnicodeEncoding(bigendian, bom);
		}else if (Regex.IsMatch(encoding, "^UTF-16BE$")){
			bigendian = true;
			bom = true;
			this.objEncoding = new UnicodeEncoding(bigendian, bom);
		}else{
			this.objEncoding = Encoding.GetEncoding(encoding);
		}
	}

//
// エンコーディング取得（引数無し）
//
	public string getEncoding(){
		return this.encoding;
	}

//
// 入力オープン（引数無し）
//
	public void openInput(){
		this.sr = new StreamReader(this.filename, this.objEncoding);
	}

//
// 入力オープン（引数：ファイル名）
//
	public void openInput(string filename){
		this.filename = filename;
		this.sr = new StreamReader(filename, this.objEncoding);
	}

//
// 出力オープン（引数無し）
//
	public void openOutput(){
		this.sw = new StreamWriter(this.filename, false, this.objEncoding);
	}

//
// 出力オープン（引数：ファイル名）
//
	public void openOutput(string filename){
		this.filename = filename;
		this.sw = new StreamWriter(filename, false, this.objEncoding);
	}

//
// 追加オープン（引数無し）
//
	public void openAppend(){
		this.sw = new StreamWriter(this.filename, true, this.objEncoding);
	}

//
// 追加オープン（引数：ファイル名）
//
	public void openAppend(string filename){
		this.filename = filename;
		this.sw = new StreamWriter(filename, true, this.objEncoding);
	}

//
// クローズ（引数無し）
//
	public void close(){
		if (this.sr != null){
			this.sr.Close();
			this.sr = null;
		}
		if (this.sw != null){
			this.sw.Close();
			this.sw = null;
		}
	}

//
// １行読込（引数無し）
//
	public string read(){
		string str = sr.ReadLine();
		if (str != null){
			if (ir != null){
				ir.setRecord(str);
			}
		}
		return str;
	}

//
// １行書込（引数無し）
//
	public void write(){
		sw.WriteLine(ir.getRecord());
	}

//
// １行書込（引数：レコード文字列）
//
	public void write(string line){
		if (line == null){
			if (ir != null){
				sw.WriteLine(ir.getRecord());
			}
		}else{
			sw.WriteLine(line);
		}
	}
}

// ============================================================
// 汎用テキスト標準入出力クラス
//
// Hint. Powershell上で使用する(|)場合は、以下を参考に...
//
// # 標準出力（パイプ）への出力時のエンコーディングをバックアップ
// $oe = $OutputEncoding
// # 標準出力（画面）への出力時のエンコーディングをバックアップ
// $scoe = [System.Console]::OutputEncoding
//
// # 標準出力（パイプ）、標準出力（画面）への出力時のエンコーディングをUTF-8BOM無しに設定
// $OutputEncoding = [System.Console]::OutputEncoding = New-Object System.Text.UTF8Encoding $false
// # 標準出力（パイプ）、標準出力（画面）への出力時のエンコーディングをUTF-16、LE、BOM有りに設定
// #$OutputEncoding = [System.Console]::OutputEncoding = New-Object System.Text.UnicodeEncoding $false,$true
// # 標準出力（パイプ）、標準出力（画面）への出力時のエンコーディングをShift-JISに設定
// #$OutputEncoding = [System.Console]::OutputEncoding = [Text.Encoding]::GetEncoding("Shift-JIS")
//
// # ... UTF-8BOMなしデータを処理 ...
// Hogge.exe | fuga.exe | foo.exe | bar.exe ...
//
// # 標準出力（パイプ）への出力時のエンコーディングを元に戻す
// $OutputEncoding = $oe
// # 標準出力（画面）への出力時のエンコーディングを元に戻す
// [System.Console]::OutputEncoding = $scoe
//
// ============================================================
public class TextStdio : ITextIO{
	IRecord ir;
	string encoding;
	Encoding objEncoding;
	TextReader sr;
	TextWriter sw;

//
// 初期化（引数無し）
//
	public TextStdio(){
		this.ir = null;
		initCommon();
	}

//
// 初期化（引数：レコードオブジェクト（レコードインタフェースを継承））
//
	public TextStdio(IRecord ir){
		this.ir = ir;
		initCommon();
	}
//
// 初期化共通処理
//
	public void initCommon(){
		setEncoding("UTF-8N");
		this.sr = null;
		this.sw = null;
	}


//
// レコードオブジェクト設定（引数：レコードオブジェクト（レコードインタフェースを継承））
//
	public void setRecord(IRecord ir){
		this.ir = ir;
	}

//
// レコードオブジェクト取得（引数無し）
//
	public IRecord getRecord(){
		return this.ir;
	}

//
// エンコーディング設定（引数：エンコーディング文字列）
// "UTF-8" | "UTF-8N" | "UTF-16" | "UTF-16N" | "UTF-16BE" | ""UTF-16BEN"
//
	public void setEncoding(string encoding){
		bool bom;
		bool bigendian;
		this.encoding = encoding;
		if (Regex.IsMatch(encoding, "^UTF-8N$")){
			bom = false;
			this.objEncoding = new UTF8Encoding(bom);
		}else if (Regex.IsMatch(encoding, "^UTF-8$")){
			bom = true;
			this.objEncoding = new UTF8Encoding(bom);
		}else if (Regex.IsMatch(encoding, "^UTF-16N$") || Regex.IsMatch(encoding, "^UTF-16LEN$")){
			bigendian = false;
			bom = false;
			this.objEncoding = new UnicodeEncoding(bigendian, bom);
		}else if (Regex.IsMatch(encoding, "^UTF-16$") || Regex.IsMatch(encoding, "^UTF-16LE$")){
			bigendian = false;
			bom = true;
			this.objEncoding = new UnicodeEncoding(bigendian, bom);
		}else if (Regex.IsMatch(encoding, "^UTF-16BEN$")){
			bigendian = true;
			bom = false;
			this.objEncoding = new UnicodeEncoding(bigendian, bom);
		}else if (Regex.IsMatch(encoding, "^UTF-16BE$")){
			bigendian = true;
			bom = true;
			this.objEncoding = new UnicodeEncoding(bigendian, bom);
		}else{
			this.objEncoding = Encoding.GetEncoding(encoding);
		}
	}

//
// エンコーディング取得（引数無し）
//
	public string getEncoding(){
		return this.encoding;
	}

//
// 入力オープン（引数無し）
//
	public void openInput(){
		this.sr = new StreamReader(Console.OpenStandardInput(), this.objEncoding);
	}

//
// 出力オープン（引数無し）
//
	public void openOutput(){
		this.sw = new StreamWriter(Console.OpenStandardOutput(), this.objEncoding);
	}

//
// クローズ（引数無し）
//
	public void close(){
		if (this.sr != null){
			this.sr.Close();
			this.sr = null;
		}
		if (this.sw != null){
			this.sw.Close();
			this.sw = null;
		}
	}

//
// １行読込（引数無し）
//
	public string read(){
		string str = this.sr.ReadLine();
		if (str != null){
			if (ir != null){
				ir.setRecord(str);
			}
		}
		return str;
	}

//
// １行書込（引数無し）
//
	public void write(){
		this.sw.WriteLine(ir.getRecord());
	}

//
// １行書込（引数：レコード文字列）
//
	public void write(string line){
		if (line == null){
			if (ir != null){
				this.sw.WriteLine(ir.getRecord());
			}
		}else{
			this.sw.WriteLine(line);
		}
	}
}


// ============================================================
// 汎用テキスト入出力インターフェース
// ============================================================
public interface ITextIO{
	void setRecord(IRecord ir);
	IRecord getRecord();
	void setEncoding(string encoding);
	string getEncoding();
	void openInput();
	void openOutput();
	void close();
	string read();
	void write();
	void write(string line);
}

// ============================================================
// 汎用レコードインターフェース
// ============================================================
public interface IRecord{
	void setSeparator(string sep);
	string getSeparator();
	void setRecord(string record);
	string getRecord();
}

// ============================================================
// オラクル操作クラス
//
//２重キー　　：ORA-00001: 一意制約(....)に反しています
//デッドロック：ORA-00060: リソース待機の間にデッドロックが検出されました。
// ============================================================
public class Oracle{
	OracleConnection con;
	OracleTransaction tran;
	Hashtable htParam;

//
// 初期化（引数無し）
// 例文  Oracle odb = new Oracle();
//       odb.open("192.168.0.1", "USER", "PASSWORD");
//
	public Oracle(){
		this.con = null;
		this.tran = null;
		this.htParam = null;
	}

//
// 初期化（引数：データソース、ユーザ名、パスワード）
// 例文   Oracle odb = new Oracle("192.168.0.1", "USER", "PASSWORD");
//
	public Oracle(string dataSource, string user, string password){
		this.open(dataSource, user, password);
	}

//
// 接続（引数：データソース、ユーザ名、パスワード）
// 例文   odb.open("192.168.0.1", "USER", "PASSWORD");
//
	public void open(string dataSource, string user, string password){
		this.con = new OracleConnection("Data Source=" + dataSource + ";User ID=" + user + ";Password=" + password + ";Integrated Security=false;");
		this.con.Open();
		this.tran = null;
		this.htParam = new Hashtable();
	}

//
// 切断（引数無し）
// 例文  odb.close();
//
	public void close(){
		this.con.Close();
		this.con.Dispose();
	}

//
// トランザクション開始（引数無し）
// 例文  odb.beginTransaction();
//
	public void beginTransaction(){
		this.tran = this.con.BeginTransaction();
	}

//
// トランザクション開始（引数：分離レベル）
// 例文  odb.beginTransaction("ReadCommitted");
//
	public void beginTransaction(string isolationlevel){
		if (isolationlevel.CompareTo("Serializable") == 0){
			this.tran = this.con.BeginTransaction(IsolationLevel.Serializable);
		}else if (isolationlevel.CompareTo("ReadCommitted") == 0){
			this.tran = this.con.BeginTransaction(IsolationLevel.ReadCommitted);
		}else{
			this.tran = this.con.BeginTransaction();
		}
	}

//
// トランザクション設定（引数：ＯＲＡＣＬＥコマンド）
// 例文  odb.setTransaction(com);
//
	public void setTransaction(object com){
		((OracleCommand)com).Transaction = this.tran;
	}

//
// ＳＱＬ文生成（引数：ＳＱＬ文）
//             （戻値：ＯＲＡＣＬＥコマンド）
// 例文  object com = odb.setSql("select * from table where id = :id");
//       何らかの処理 
//       odb.closeSql(com);	// ＳＱＬ文の生成と解放は原則セットで行うこと
//
	public object setSql(string sql){
		OracleCommand com;
		com = new OracleCommand();
		com.Connection = this.con;
		com.Transaction = this.tran;
		com.CommandText = sql;
		return com;
	}

//
// ＳＱＬ文解放（引数：ＯＲＡＣＬＥコマンド）
// 例文  odb.closeSql(com);
//
	public void closeSql(object com){
		((OracleCommand)com).Dispose();
	}

//
// バインド値設定（引数：ＯＲＡＣＬＥコマンド、バインド名、値）
// 例文  odb.bind(com, "id", "001234");
//
// BUGS  execNonQuery(), execQuery()実行後、すべてのバインド値は削除される
//
	public void bind(object com, string name, object value){
		if (htParam.ContainsKey(name)){
			htParam.Remove(name);
			((OracleCommand)com).Parameters.Remove(htParam[name]);
		}
		htParam[name] = new OracleParameter(name, value);
		((OracleCommand)com).Parameters.Add(htParam[name]);
	}

//
// 更新系ＳＱＬ実行（引数：ＯＲＡＣＬＥコマンド）
//                 （戻値：更新件数）
// 例文  int row = odb.execNonQuery(com);
//
// BUGS  execNonQuery(), execQuery()実行後、すべてのバインド値は削除される
//
	public int execNonQuery(object com){
		int row;
		string errmsg = "";
		ArrayList list;

		try {
			row = ((OracleCommand)com).ExecuteNonQuery();
		}catch (Exception e){
			row = -1;
			errmsg = e.Message;
		}

		list = new ArrayList (htParam.Keys);
		foreach (string key in list){
			((OracleCommand)com).Parameters.Remove(htParam[key]);
			htParam.Remove(key);
		}
		if (row < 0){
			throw new Exception(errmsg);
		}
		return row;
	}

//
// 参照系ＳＱＬ実行（引数：ＯＲＡＣＬＥコマンド）
//                 （戻値：ＯＲＡＣＬＥデータリーダ）
// 例文  object reader = odb.execQuery(com);
//
// BUGS  execNonQuery(), execQuery()実行後、すべてのバインド値は削除される
//
	public object execQuery(object com){
		OracleDataReader reader;
		ArrayList list;

		reader = ((OracleCommand)com).ExecuteReader();

		list = new ArrayList (htParam.Keys);
		foreach (string key in list){
			((OracleCommand)com).Parameters.Remove(htParam[key]);
			htParam.Remove(key);
		}
		return reader;
	}

//
// 参照行取得（引数：ＯＲＡＣＬＥデータリーダ）
//           （戻値：真偽値）
// 例文  while (odb.fetch(reader)){
//           Console.WriteLine(odb.get(reader,"id") );
//       }
//       odb.closeFetch(reader);	// 参照行取得と終了は原則セットで行うこと
//
	public bool fetch(object reader){
		return ((OracleDataReader)reader).Read();
	}

//
// 参照行終了（引数：オラクルデータリーダ）
// 例文  odb.closeFetch(reader);
//
	public void closeFetch(object reader){
		((OracleDataReader)reader).Close();
		((OracleDataReader)reader).Dispose();
	}

//
// 項目取得（引数：オラクルデータリーダ、項目名）
//         （戻値：項目内容）
// 例文  string id = odb.get(reader, "id");
//
	public object get(object reader, string name){
		return ((OracleDataReader)reader)[name];
	}

//
// コミット実行（引数無し）
// 例文  odb.commit();
//       odb.endTransaction();
//
	public void commit(){
		this.tran.Commit();
		this.tran = null;
	}
	public void endTransaction(){
		this.commit();
	}
//
// ロールバック実行（引数無し）
// 例文  odb.rollback();
//       odb.abortTransaction();
//
	public void rollback(){
		this.tran.Rollback();
		this.tran = null;
	}
	public void abortTransaction(){
		this.rollback();
	}
}

// ============================================================
// MSSQL操作クラス
//
//２重キー　　：制約 '....' の PRIMARY KEY 違反。オブジェクト '....' には重複するキーを挿入できません。重複するキーの値は (....) です。
//デッドロック：トランザクション (プロセス ID xx) が、ロック 個のリソースで他のプロセスとデッドロック して、このトランザクションがそのデッドロックの対象となりました。トランザクションを再実行してください。
// ============================================================
public class MsSql{
	SqlConnection con;
	SqlTransaction tran;
	Hashtable htParam;

//
// 初期化（引数無し）
// 例文  MsSql odb = new MsSql();
//       odb.open("192.168.0.1", "DBNAME", 15000, "USER", "PASSWORD");
//   
//
	public MsSql(){
		this.con = null;
		this.tran = null;
		this.htParam = null;
	}

//
// 初期化（引数：データソース、初期ＤＢ、タイムアウト(ms)、ユーザ名、パスワード）
// 例文  MsSql odb = new MsSql("192.168.0.1", "DBNAME", 15000, "USER", "PASSWORD");
//
	public MsSql(string dataSource, string initialCatalog, int timeoutMs, string user, string password){
		this.open(dataSource, initialCatalog, timeoutMs, user, password);
	}

//
// 接続（引数：データソース、初期ＤＢ、タイムアウト(ms)、ユーザ名、パスワード）
// 例文  odb.open("192.168.0.1", "DBNAME", 15000, "USER", "PASSWORD");
//
	public void open(string dataSource, string initialCatalog, int timeoutMs, string user, string password){
		SqlConnectionStringBuilder builder;

		builder = new SqlConnectionStringBuilder();
		builder.DataSource = dataSource;
		builder.MultipleActiveResultSets = true;
		builder.UserID = user;
		builder.Password = password;
		builder.InitialCatalog = initialCatalog;
		builder.ConnectTimeout = timeoutMs;

		this.con = new SqlConnection(builder.ConnectionString);
		this.con.Open();
		this.tran = null;

		this.htParam = new Hashtable();
	}

//
// 切断（引数無し）
// 例文  odb.close();
//
	public void close(){
		this.con.Close();
		this.con.Dispose();

	}

//
// トランザクション開始（引数無し）
// 例文  odb.beginTransaction();
//
	public void beginTransaction(){
		this.tran = con.BeginTransaction();
	}

//
// トランザクション開始（引数：分離レベル）
// 例文  odb.beginTransaction("ReadCommitted");
//
	public void beginTransaction(string isolationlevel){
		if (isolationlevel.CompareTo("Serializable") == 0){
			this.tran = this.con.BeginTransaction(IsolationLevel.Serializable);
		}else if (isolationlevel.CompareTo("ReadCommitted") == 0){
			this.tran = this.con.BeginTransaction(IsolationLevel.ReadCommitted);
		}else if (isolationlevel.CompareTo("RepeatableRead") == 0){
			this.tran = this.con.BeginTransaction(IsolationLevel.RepeatableRead);
		}else{
			this.tran = this.con.BeginTransaction();
		}
	}

//
// トランザクション設定（引数：ＳＱＬコマンド）
// 例文  odb.setTransaction(com);
//
	public void setTransaction(object com){
		((SqlCommand)com).Transaction = this.tran;
	}

//
// ＳＱＬ文生成（引数：ＳＱＬ文）
//             （戻値：ＳＱＬコマンド）
// 例文  object com = odb.setSql("select * from table where id = @id");
//       何らかの処理 
//       odb.closeSql(com);	// ＳＱＬ文の生成と解放は原則セットで行うこと
//
	public object setSql(string sql){
		SqlCommand com;
		com = new SqlCommand(sql, this.con);
		com.Transaction = this.tran;
		return com;
	}

//
// ＳＱＬ文解放（引数：ＳＱＬコマンド）
// 例文  odb.closeSql(com);
//
	public void closeSql(object com){
		((SqlCommand)com).Dispose();
	}

//
// バインド値設定（引数：ＳＱＬコマンド、バインド名、値）
// 例文  odb.bind(com, "@id", "001234");
//
// BUGS  execNonQuery(), execQuery()実行後、すべてのバインド値は削除される
//
	public void bind(object com, string name, object value){
		if (htParam.ContainsKey(name)){
			htParam.Remove(name);
			((SqlCommand)com).Parameters.Remove(htParam[name]);
		}
		htParam[name] = new SqlParameter(name, value);
		((SqlCommand)com).Parameters.Add(htParam[name]);
	}

//
// 更新系ＳＱＬ実行（引数：ＳＱＬコマンド）
//                 （戻値：更新件数）
// 例文  int row = odb.execNonQuery(com);
//
// BUGS  execNonQuery(), execQuery()実行後、すべてのバインド値は削除される
//
	public int execNonQuery(object com){
		int row;
		string errmsg = "";
		ArrayList list;

		try {
			row = ((SqlCommand)com).ExecuteNonQuery();
		}catch (Exception e){
			row = -1;
			errmsg = e.Message;
		}

		list = new ArrayList (htParam.Keys);
		foreach (string key in list){
			((SqlCommand)com).Parameters.Remove(htParam[key]);
			htParam.Remove(key);
		}
		if (row < 0){
			throw new Exception(errmsg);
		}
		return row;
	}

//
// 参照系ＳＱＬ実行（引数：ＳＱＬコマンド）
//                 （戻値：ＳＱＬデータリーダ）
// 例文  object reader = odb.execQuery(com);
//
// BUGS  execNonQuery(), execQuery()実行後、すべてのバインド値は削除される
//
	public object execQuery(object com){
		SqlDataReader reader;
		ArrayList list;

		reader = ((SqlCommand)com).ExecuteReader();

		list = new ArrayList (htParam.Keys);
		foreach (string key in list){
			((SqlCommand)com).Parameters.Remove(htParam[key]);
			htParam.Remove(key);
		}
		return reader;
	}

//
// 参照行取得（引数：ＳＱＬデータリーダ）
//           （戻値：真偽値）
// 例文  while (odb.fetch(reader)){
//           Console.WriteLine(odb.get(reader,"id") );
//       }
//       odb.closeFetch(reader);	// 参照行取得と終了は原則セットで行うこと
//
	public bool fetch(object reader){
		return ((SqlDataReader)reader).Read();
	}

//
// 参照行終了（引数：ＳＱＬデータリーダ）
// 例文  odb.closeFetch(reader);
//
	public void closeFetch(object reader){
		((SqlDataReader)reader).Close();
		((SqlDataReader)reader).Dispose();
	}

//
// 項目取得（引数：ＳＱＬデータリーダ、項目名）
//         （戻値：項目内容）
// 例文  string id = odb.get(reader, "id");
//
	public object get(object reader, string name){
		return ((SqlDataReader)reader)[name];
	}

//
// コミット実行（引数無し）
// 例文  odb.commit();
//       odb.endTransaction();
//
	public void commit(){
		this.tran.Commit();
		this.tran = null;
	}
	public void endTransaction(){
		this.commit();
	}
//
// ロールバック実行（引数無し）
// 例文  odb.rollback();
//       odb.abortTransaction();
//
	public void rollback(){
		this.tran.Rollback();
		this.tran = null;
	}
	public void abortTransaction(){
		this.rollback();
	}
}
