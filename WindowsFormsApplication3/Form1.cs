using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SpeechLib; //for ver11
using System.Speech.Recognition;
using System.Xml;

namespace WindowsFormsApplication1
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            /*
            // ここから追加するコード

            // 音声認識エンジンを生成
            SpeechRecognitionEngine engine = new SpeechRecognitionEngine();

            // 音声を認識した際に発生するイベントに処理を追加
            engine.SpeechRecognized += delegate(object sender, SpeechRecognizedEventArgs e)
            {
                // 音声認識した結果をフォームのラベルに表示
                this.label1.Text = e.Result.Text;
            };

            // 文法読み込み
            engine.LoadGrammar(new DictationGrammar());

            // 既定のマイク入力をインプットデバイスとして設定
            engine.SetInputToDefaultAudioDevice();
            //engine.SetInputToWaveFile(@"C:\Documents and Settings\admin\デスクトップ\sample.wav");

            // 音声認識を非同期で開始
            engine.RecognizeAsync(RecognizeMode.Multiple);

            // ここまで追加するコード
            */


            LWWS lwws = new LWWS(63); // 63：東京
            lwws.Refresh();
            textBox1.Text = lwws.Tenki;

        }


        public iRemocon iRem = new iRemocon();

        private void button1_Click(object sender, EventArgs e)
        {
            iRem.connect("192.168.11.6", 51013);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = iRem.check();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = iRem.sending(int.Parse(textBox2.Text));
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Text = iRem.learn(int.Parse(textBox2.Text));

        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox1.Text = iRem.timer(textBox2.Text,long.Parse(textBox3.Text));
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox1.Text = iRem.timerlist();
        }

        private void button7_Click(object sender, EventArgs e)
        {


        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close(); 
        }
        protected override void WndProc(ref Message m)
        {
            // Form のドラッグ移動 処理
            base.WndProc(ref m);
            if ((m.Msg == 0x84) && (m.Result == (IntPtr)1)) m.Result = (IntPtr)2;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            textBox1.Text = "aaaa";
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            textBox1.Text = "bbbb";
        }
    }


    public class iRemocon
    {
        private System.Net.Sockets.TcpClient objSck = new System.Net.Sockets.TcpClient();
        private System.Net.Sockets.NetworkStream objStm;

        public void connect(string ip, int port)
        {
            // ソケット接続
            objSck.Connect(ip, port);
            // ソケットストリーム取得
            objStm = objSck.GetStream();
        }

        public string sending(int num)
        {
            Byte[] dat = System.Text.Encoding.GetEncoding("SHIFT-JIS").GetBytes("*is;" + num + ";\r\n");
            objStm.Write(dat, 0, dat.GetLength(0));
            System.Threading.Thread.Sleep(1000);
            // ソケット受信
            if (objSck.Available > 0)
            {
                dat = new Byte[objSck.Available];
                objStm.Read(dat, 0, dat.GetLength(0));
                return System.Text.Encoding.GetEncoding("SHIFT-JIS").GetString(dat);
            }
            return "lost";

        }

        public string check()
        {
            Byte[] dat = System.Text.Encoding.GetEncoding("SHIFT-JIS").GetBytes("*au\r\n");
            objStm.Write(dat, 0, dat.GetLength(0));
            System.Threading.Thread.Sleep(1000);
            // ソケット受信
            if (objSck.Available > 0)
            {
                dat = new Byte[objSck.Available];
                objStm.Read(dat, 0, dat.GetLength(0));
                return System.Text.Encoding.GetEncoding("SHIFT-JIS").GetString(dat);
            }
            return "lost";

        }

        public string learn(int num)
        {

            Byte[] dat = System.Text.Encoding.GetEncoding("SHIFT-JIS").GetBytes("*ic;" + num + ";\r\n");
            objStm.Write(dat, 0, dat.GetLength(0));
            System.Threading.Thread.Sleep(1000);
            // ソケット受信
            if (objSck.Available > 0)
            {
                dat = new Byte[objSck.Available];
                objStm.Read(dat, 0, dat.GetLength(0));
                return System.Text.Encoding.GetEncoding("SHIFT-JIS").GetString(dat);
            }
            return "lost";
        }

        public string timerlist()
        {

            Byte[] dat = System.Text.Encoding.GetEncoding("SHIFT-JIS").GetBytes("*tl\r\n");
            objStm.Write(dat, 0, dat.GetLength(0));
            System.Threading.Thread.Sleep(1000);
            // ソケット受信
            if (objSck.Available > 0)
            {
                dat = new Byte[objSck.Available];
                objStm.Read(dat, 0, dat.GetLength(0));
                return System.Text.Encoding.GetEncoding("SHIFT-JIS").GetString(dat);
            }
            return "lost";
        }

        public string timer(string rcode, long time)
        {
            Byte[] dat = System.Text.Encoding.GetEncoding("SHIFT-JIS").GetBytes("*tg\r\n");
            objStm.Write(dat, 0, dat.GetLength(0));
            System.Threading.Thread.Sleep(1000);
            // ソケット受信
            if (objSck.Available == 0)
            {
                return "lost";
            }

            dat = new Byte[objSck.Available];
            objStm.Read(dat, 0, dat.GetLength(0));
            string str = System.Text.Encoding.GetEncoding("SHIFT-JIS").GetString(dat);

            str = str.Substring(6, 10);

            long a = int.Parse(str);

            long b = a + time;

            System.Threading.Thread.Sleep(5000);

            dat = System.Text.Encoding.GetEncoding("SHIFT-JIS").GetBytes("*tm;"+ rcode +";"+b.ToString()+";0\r\n");
            objStm.Write(dat, 0, dat.GetLength(0));
            System.Threading.Thread.Sleep(1000);
            // ソケット受信
            if (objSck.Available > 0)
            {
                dat = new Byte[objSck.Available];
                objStm.Read(dat, 0, dat.GetLength(0));
                return System.Text.Encoding.GetEncoding("SHIFT-JIS").GetString(dat);
            }
            return "lost";
        }
    }

    public class LWWS
    {
        // フィールド
        string _tenki; // 例：曇時々雨
        string _title; // 例：東京都 東京 - 今日の天気
        string _link;  // 例：http://weather.livedoor.com/area/13/63.html?v=1
        bool _available; // データが正しく取得できればtrue
        DateTime _publicTime; // 予報の発表日時
        Icon _icon; // タスクトレイ用のアイコン

        // プロパティ
        public string Tenki { get { return _tenki; } }
        public string Title { get { return _title; } }
        public string Link { get { return _link; } }
        public bool Available { get { return _available; } }
        public DateTime PublicTime { get { return _publicTime; } }
        public Icon Icon { get { return _icon; } }

        string lwwsUrl; // お天気WebサービスのURL

        // コンストラクタ
        public LWWS(int id)
        {
            string lwws = "http://weather.livedoor.com/forecast/webservice/rest/v1?city={0}&day=today";
            lwwsUrl = String.Format(lwws, id);
        }

        public void Refresh()
        {
            _available = false;

            //  XMLデータの取得と解析
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(lwwsUrl);
            }
            catch
            {
                return;
            }
            _title = doc.DocumentElement.SelectSingleNode
                                           ("/lwws/title").InnerText;
            _tenki = doc.DocumentElement.SelectSingleNode
                                           ("/lwws/image/title").InnerText;
            _link = doc.DocumentElement.SelectSingleNode
                                           ("/lwws/image/link").InnerText;
            string imageURL = doc.DocumentElement.SelectSingleNode
                                           ("/lwws/image/url").InnerText;
            string pt = doc.DocumentElement.SelectSingleNode
                                          ("/lwws/publictime").InnerText;
            _publicTime = DateTime.Parse(pt);

            //  アイコンの作成

            _available = true;
        }
    }
}
