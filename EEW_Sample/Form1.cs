using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
//Newtonsoft.JsonをNugetから導入するのを忘れずに

namespace EEW_Sample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private readonly HttpClient client = new HttpClient();

        private async void timer1_Tick(object sender, EventArgs e)
        {
            try{
                DateTime dt = DateTime.Now; //現在時刻の取得(PC時刻より)
                var tm = dt.AddSeconds(-2); //現在時刻から2秒引く(取得失敗を防ぐため)
                var time = tm.ToString("yyyyMMddHHmmss");//時刻形式の指定(西暦/月/日/時/分/秒)

                var url = $"http://www.kmoni.bosai.go.jp/webservice/hypo/eew/{time}.json"; //強震モニタURLの指定

                var json = await client.GetStringAsync(url); //awaitを用いた非同期JSON取得
                var eew = JsonConvert.DeserializeObject<EEW>(json);//EEWクラスを用いてJSONを解析(デシリアライズ)

                //JSONの中から使うデータを指定して使いやすいように名前を変えます
                var repo_time = eew.report_time;                             // 取得時刻(string)
                var reg_name = eew.region_name;                              // 震源地名(string)
                var latitude = eew.latitude;                                 // 緯度(string(本来はfloat))
                var longtude = eew.longitude;                                // 経度(string(本来はfloat))
                var depth = eew.depth;                                       // 深さ(string)
                var max_int = eew.calcintensity;                             // 予測震度(string)
                var mag = eew.magunitude;                                    // マグニチュード(string(本来はfloat))
                bool end_flg = eew.is_final == "true";                       //最終報フラグ(bool)
                var repo_num = eew.report_num;                               // 報番(string(本来はint))
                var ori_time = eew.origin_time;                              // 発生時刻(string)
                var aler_flg = eew.alertflg;                                 // 警報フラグ(string)
                var eew_flg = eew.result.message;                            // EEWフラグ(string)
                string eew_flgs = null;

                //種別判別(これをAPIレベルでやれるようになってほしい)
                if (eew_flg != "データがありません")
                {
                    if (aler_flg == "予報")
                    {
                        eew_flgs = "fore";
                    }
                    else if (end_flg != false)
                    {
                        eew_flgs = "fore_end";
                    }
                    if (aler_flg == "予報")
                    {
                        eew_flgs = "war";
                    }
                    else if (end_flg != false)
                    {
                        eew_flgs = "war_end";
                    }
                }
                else
                {
                    eew_flgs = "none";
                }

                //実処理部分(switch文を使ってけ)
                switch (eew_flgs)
                {
                    case "fore":
                        label1.Text = $"緊急地震速報(予報)  第{repo_num}報  {reg_name}で地震  最大max_int{max_int}\r\nmag{mag}  reg_nameのdepth:{depth}";
                        break;
                    case "fore_end":
                        label1.Text = $"緊急地震速報(予報)  最終報  {reg_name}で地震  最大max_int{max_int}\r\nmag{mag}  reg_nameのdepth:{depth}";
                        break;
                    case "war":
                        label1.Text = $"緊急地震速報(警報)  第{repo_num}報  {reg_name}で地震  最大max_int{max_int}\r\nmag{mag}  reg_nameのdepth:{depth}";
                        break;
                    case "war_end":
                        label1.Text = $"緊急地震速報(警報)  最終報  {reg_name}で地震  最大max_int{max_int}\r\nmag{mag}  reg_nameのdepth:{depth}";
                        break;
                    case "none":
                        label1.Text = "EEWが発表されていません";
                        break;
                }
            }
            catch (Exception ex){

                timer1.Enabled = false;
                await Task.Delay(100);
                timer1.Enabled = true;
            }

        }

    }    
}