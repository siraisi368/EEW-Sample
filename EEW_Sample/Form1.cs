using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Windows.Forms;
using System.Threading.Tasks;
namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public readonly HttpClient client = new HttpClient();

        private async void timer1_Tick(object sender, EventArgs e)//緊急地震速報(NIED)
        {
            try
            {
                var url2 = "http://www.kmoni.bosai.go.jp/webservice/server/pros/latest.json";//時刻調整 NIEDのAPIのURLをurl2に代入
                var json2 = await client.GetStringAsync(url2);//非同期処理でurl2(http://....)を取得
                var eew2 = JsonConvert.DeserializeObject<NIEDTime.Root>(json2);//Json解析 NIEDTimeはTime.csにあります
                var time = eew2.latest_time.Replace("/", "").Replace(":", "").Replace(" ", "");//解析したJsonのlatest_timeの"/",":"," "を削除

                label6.Text = eew2.latest_time;//時間表示

                var json = await client.GetStringAsync($"http://www.kmoni.bosai.go.jp/webservice/hypo/eew/{time}.json");//取得した時刻の緊急地震速報を取得
                var eew = JsonConvert.DeserializeObject<NIED>(json);//Json解析 NIEDはJsonDeserialize.csにあります
                string eew_flgs = null;//eew_flgsを作成

                label2.Text = eew.result.message;//テスト用 result.messageをlabel2に表示
                label3.Text = eew.alertflg;//テスト用 result.messageをlabel3に表示

                //ここで緊急地震速報の状態を識別
                if (eew.result.message != "データがありません")//a != b はaとbが異なることを表す
                {
                    if (eew.alertflg == "予報")
                    {
                        eew_flgs = "fore";

                        if (eew.is_final == "true")
                        {
                            eew_flgs = "fore_end";
                        }
                    }

                    if (eew.alertflg == "警報")
                    {
                        eew_flgs = "warning";
                        if (eew.is_final == "true")
                        {
                            eew_flgs = "warning_end";
                        }
                    }
                }
                else
                {
                    eew_flgs = "none";//result.messageが データがありません のときは eew_flgsがnoneになる
                }

                switch (eew_flgs)//switch文 この場合、eew_flgsの中身によって挙動が変わる
                {
                    case "fore":
                        label1.Text = $"緊急地震速報(予報) 第{eew.report_num}報  {eew.region_name}で地震  最大震度{eew.calcintensity}\r\nマグニチュード{eew.magunitude}  震源の深さ:{eew.depth}";
                        break;
                    case "fore_end":
                        label1.Text = $"緊急地震速報(予報)  最終報  {eew.region_name}で地震  最大震度{eew.calcintensity}\r\nマグニチュード{eew.magunitude}  震源の深さ:{eew.depth}";
                        break;
                    case "warning":
                        label1.Text = $"緊急地震速報(警報)  第{eew.report_num}報  {eew.region_name}で地震  最大震度{eew.calcintensity}\r\nマグニチュード{eew.magunitude}  震源の深さ:{eew.depth}";
                        break;
                    case "warning_end":
                        label1.Text = $"緊急地震速報(警報)  最終報  {eew.region_name}で地震  最大震度{eew.calcintensity}\r\nマグニチュード{eew.magunitude}  震源の深さ:{eew.depth}";
                        break;
                    case "none":
                        label1.Text = "EEWが発表されていません";
                        break;
                    default://これは上の5つのどれにも該当しなかったときの挙動
                        label1.Text = "エラーが発生しました。";
                        break;
                }
            }
            catch 
            {
                timer1.Enabled = false;
                timer2.Enabled = false;
                label1.Text = "エラーが発生しました。";
                await Task.Delay(1000);
                timer1.Enabled = true;
                timer2.Enabled = true;
            }
        }

        private async void timer2_Tick(object sender, EventArgs e)//振動レベル
        {
            var json = await client.GetStringAsync($"https://kwatch-24h.net/EQLevel.json");//Jsonを取得
            var eql = JsonConvert.DeserializeObject<EQL.Root>(json);//Jsonを解析 クラスはEQLevel.csにあります
            label4.Text = eql.l;//lを表示
            label5.Text = eql.t;//
        }
    }
}
