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

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now; //現在時刻の取得(PC時刻より)
            var tm = dt.AddSeconds(-2); //現在時刻から2秒引く(取得失敗を防ぐため)
            var time = tm.ToString("yyyyMMddHHmmss");//時刻形式の指定(西暦/月/日/時/分/秒)
            var client = new HttpClient();



            var url = $"http://www.kmoni.bosai.go.jp/webservice/hypo/eew/{time}.json"; //強震モニタURLの指定

            var json = await client.GetStringAsync(url); //awaitを用いた非同期JSON取得
            var eew = JsonConvert.DeserializeObject<EEW>(json);//EEWクラスを用いてJSONを解析(デシリアライズ)
            //JSONの中から使うデータを指定して使いやすいように名前を変えます
            var 取得時刻 = eew.report_time;
            var 震源 = eew.region_name;
            var 緯度 = eew.latitude;
            var 経度 = eew.longitude;
            var キャンセルフラグ = eew.is_cancel;
            var 深さ = eew.depth;
            var 震度 = eew.calcintensity;
            var 最終報フラグ = eew.is_final;
            var 報数 = eew.report_num;
            var 発生時刻 = eew.origin_time;
            var 警報フラグ = eew.alertflg;
            var EEWフラグ = eew.result.message;
            var マグニチュード = eew.magunitude;
            
            //実処理部分(ここでは全部を一行に入れてます)
            if(EEWフラグ == "データがありません")//もしEEWが発表されていないなら
            {
                label1.Text = "EEW 未発表";
            }
            else //そうでなければ
            {
                if(警報フラグ == "予報")//もし予報なら
                {
                    if(最終報フラグ == "false")//最終報でなければ
                    {
                        label1.Text = $"緊急地震速報(予報)  第{報数}報  {震源}で地震  最大震度{震度}\r\nマグニチュード{マグニチュード}  震源の深さ:{深さ}";
                    }
                    else//そうでなければ
                    {
                        label1.Text = $"緊急地震速報(予報)  最終報  {震源}で地震  最大震度{震度}\r\nマグニチュード{マグニチュード}  震源の深さ:{深さ}";
                    }
                }
                if (警報フラグ == "警報")//もし警報なら
                {
                    if (最終報フラグ == "false")//最終報でなければ
                    {
                        label1.Text = $"緊急地震速報(警報)  第{報数}報  {震源}で地震  最大震度{震度}\r\nマグニチュード{マグニチュード}  震源の深さ:{深さ}";
                    }
                    else//そうでなければ
                    {
                        label1.Text = $"緊急地震速報(警報)  最終報  {震源}で地震  最大震度{震度}\r\nマグニチュード{マグニチュード}  震源の深さ:{深さ}";
                    }
                }
            }
        }
        
        //EEWクラス
        class EEW
        {
            public Result result { get; set; }
            public string report_time { get; set; }
            public string region_code { get; set; }
            public string request_time { get; set; }
            public string region_name { get; set; }
            public string longitude { get; set; }
            public String is_cancel { get; set; }
            public string depth { get; set; }
            public string calcintensity { get; set; }
            public String is_final { get; set; }
            public String is_training { get; set; }
            public string latitude { get; set; }
            public string origin_time { get; set; }
            public Security security { get; set; }
            public string magunitude { get; set; }
            public string report_num { get; set; }
            public string request_hypo_type { get; set; }
            public string report_id { get; set; }
            public string alertflg { get; set; }
        }
        public class Result
        {
            public string status { get; set; }
            public string message { get; set; }
            public String is_auth { get; set; }
        }

        public class Security
        {
            public string realm { get; set; }
            public string hash { get; set; }
        }
    }
}
