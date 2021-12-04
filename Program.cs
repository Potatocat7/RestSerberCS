using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;

//ここでJOSNファイル用の構造体を準備しておく
class Data {
    public string id {get; set;}
    public string name {get; set;}
    //public string dept { get; set;}
}
class SimpleWebServer
{
  static void Main()
  {
    // HTTPリスナー作成
    HttpListener listener = new HttpListener();
    // リスナー設定
    listener.Prefixes.Clear();
    listener.Prefixes.Add(@"http://localhost:8080/"); // プレフィックスの登録
    listener.Start();
    var jsonList = new List<Data>();
    //Data jsonData = new Data();
    //文字列保存用リスト
    var strList = new List<string>();
    while (true)//ブラウザで送信すると２回入る、なぜか不明
    {
        // リクエスト取得
        HttpListenerContext context = listener.GetContext();
        HttpListenerRequest request = context.Request;
        // レスポンス取得
        HttpListenerResponse response = context.Response;

        //POST(PostAsync)で送られた文字列の表示
        string str = request.HttpMethod;
        int count = 0;
        string jsonStr = "";

        if (str == "POST"){
            StreamReader reader = new StreamReader(request.InputStream);
            string reqBody = reader.ReadToEnd();
            Console.WriteLine($"contentbody: {reqBody}");
            //リストから同じ文字列をカウント
            for(int i=0; i<strList.Count;i++){
                if(strList[i]==reqBody){
                    count++;
                }
                Data jsonData = new Data();
                jsonData.name = strList[i];
                jsonData.id = i.ToString();
                jsonList.Add(jsonData);
                jsonData = null;
            }
            //文字列をリストに追加
            strList.Add(reqBody);

            //Jsonファイルの編集
            Data newData = new Data();
            newData.name = reqBody;
            newData.id = (strList.Count-1).ToString();
            jsonList.Add(newData);
            jsonStr = JsonSerializer.Serialize(jsonList);
            jsonList.Clear();
        }
        //結果を送信する
        if (request != null)
        {
            if(request.RawUrl=="/a"){
                //数字を返す
                byte[] text = Encoding.UTF8.GetBytes(count.ToString());
                response.OutputStream.Write(text, 0, text.Length);
                Console.WriteLine("a到達");
            }
            else if(request.RawUrl=="/b"){
                jsonStr = "{\"answer\":" + jsonStr + "}";
                byte[] text = Encoding.UTF8.GetBytes(jsonStr);
                response.OutputStream.Write(text, 0, text.Length);
                Console.WriteLine("b到達");
            }
            else{
                //byte[] text = Encoding.UTF8.GetBytes("");
                //response.OutputStream.Write(text, 0, text.Length);
                Console.WriteLine("到達!");
            }
            //Console.WriteLine("Hello World!");
        }
        else
        {
            response.StatusCode = 404;
        }
        response.Close();
    }
  }
}
