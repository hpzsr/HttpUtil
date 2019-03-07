using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Threading.Tasks;
using LitJson;

namespace HttpUtil
{
    class Program
    {
        private static HttpListener httpRequest;   // 请求监听  
        private static HttpListenerContext requestContext;

        static void Main(string[] args)
        {
            // MySqlUtil.getInstance().openDatabase();

            // DBTableManager.getInstance().init();

            // 遍历整个表
            //List<DBTablePreset> list = MySqlUtil.getInstance().queryDatabaseTable("userinfo");
            //for (int i = 0; i < list.Count; i++)
            //{
            //    for (int j = 0; j < list[i].keyList.Count; j++)
            //    {
            //        Console.Write(list[i].keyList[j].m_value + "    ");
            //    }

            //    Console.WriteLine();
            //}

            // 按条件查询
            //List<DBTablePreset> list = MySqlUtil.getInstance().getTableData("userinfo", new List<TableKeyObj>() { new TableKeyObj("id",TableKeyObj.ValueType.ValueType_int, 2), new TableKeyObj("name", TableKeyObj.ValueType.ValueType_string, "zsr") });
            //for (int i = 0; i < list.Count; i++)
            //{
            //    for (int j = 0; j < list[i].keyList.Count; j++)
            //    {
            //        Console.Write(list[i].keyList[j].m_value + "    ");
            //    }

            //    Console.WriteLine();
            //}

            // 增加数据
            //MySqlUtil.getInstance().insertData("userinfo", new List<TableKeyObj>() { new TableKeyObj("id", TableKeyObj.ValueType.ValueType_int, 3), new TableKeyObj("account", TableKeyObj.ValueType.ValueType_string, "444444"), new TableKeyObj("psw", TableKeyObj.ValueType.ValueType_string, "111"), new TableKeyObj("name", TableKeyObj.ValueType.ValueType_string, "hpzsr"), new TableKeyObj("sex", TableKeyObj.ValueType.ValueType_int, 0),});

            // 删除数据
            // MySqlUtil.getInstance().deleteData("userinfo", new List<TableKeyObj>() { new TableKeyObj("id",TableKeyObj.ValueType.ValueType_int, 1), new TableKeyObj("name", TableKeyObj.ValueType.ValueType_string, "hp") });

            // 修改数据
            // MySqlUtil.getInstance().updateData("userinfo", new List<TableKeyObj>() { new TableKeyObj("name", TableKeyObj.ValueType.ValueType_string, "hpzsr")}, new List<TableKeyObj>() { new TableKeyObj("psw", TableKeyObj.ValueType.ValueType_string, "123456") });

            try
            {
                httpRequest = new HttpListener();
                //httpRequest.Prefixes.Add("http://fksq.hy51v.com:10086/");  //添加监听地址 注意是以/结尾

                {
                    string name = Dns.GetHostName();
                    IPAddress[] ipadrlist = Dns.GetHostAddresses(name);

                    /*
                     * httplistener 拒绝访问  解决办法
                     * cmd:netsh http add urlacl url=http://*:10086/ user=Everyone listen=yes
                     */

                    // 亚马逊服务器  http://3.17.204.203:10086/test3?param=124
                    string url = "http://*:10086/";
                    httpRequest.Prefixes.Add(url + "test1/");
                    httpRequest.Prefixes.Add(url + "test2/");
                    httpRequest.Prefixes.Add(url + "test3/");
                    
                    Console.WriteLine("监听地址    " + url);
                }

                httpRequest.Start(); //允许该监听地址接受请求的传入。  

                GethttpRequestAsync();

                Console.WriteLine("开始监听客户端请求:");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
            Console.ReadKey();
        }

        /// <summary>  
        /// 执行其他超做请求监听行为  
        /// </summary>  
        public static async void GethttpRequestAsync()
        {
            while (true)
            {
                requestContext = await httpRequest.GetContextAsync(); //接受到新的请求

                try
                {
                    //reecontext 为开启线程传入的 requestContext请求对象  
                    //Thread subthread = new Thread(onHttpHandle);
                    //subthread.Start(requestContext);

                    await onHttpHandle(requestContext);
                }
                catch (Exception ex)
                {
                    try
                    {
                        requestContext.Response.StatusCode = 500;
                        requestContext.Response.ContentType = "application/text";
                        requestContext.Response.ContentEncoding = Encoding.UTF8;
                        byte[] buffer = System.Text.Encoding.UTF8.GetBytes("System Error");

                        //对客户端输出相应信息.  
                        requestContext.Response.ContentLength64 = buffer.Length;
                        Stream output = requestContext.Response.OutputStream;
                        output.Write(buffer, 0, buffer.Length);
                        //关闭输出流，释放相应资源  
                        output.Close();
                    }
                    catch
                    {
                    }
                }
            }
        }

        public static async Task onHttpHandle(object reecontext)
        {
            HttpListenerContext request = (HttpListenerContext)reecontext;
            string jiekou = requestContext.Request.Url.AbsolutePath;

            string param = "";

            if (request.Request.HttpMethod.CompareTo("GET") == 0)
            {
                param = HttpUtility.UrlDecode(request.Request.QueryString["param"]);

                Console.WriteLine(CommonUtil.getCurTime() + "----收到GET请求:" + jiekou + "    参数：" + param);
            }
            else if (request.Request.HttpMethod.CompareTo("POST") == 0)
            {
                Stream body = request.Request.InputStream;
                Encoding encoding = request.Request.ContentEncoding;
                StreamReader reader = new System.IO.StreamReader(body, encoding);

                param = reader.ReadToEnd();
                body.Close();
                reader.Close();

                Console.WriteLine(CommonUtil.getCurTime() + "----收到POST请求:" + jiekou + "    参数：" + param);
            }

            string backData = param;

            if ("/test1".Equals(jiekou))
            {
                backData = "test1 " + param;
            }
            else if ("/test2".Equals(jiekou))
            {
                backData = "test2 " + param;
            }
            else if ("/test3".Equals(jiekou))
            {
                backData = "test3 " + param;
            }

            // ----这里不写好像也没问题----
            //request.Response.StatusCode = 200;
            //request.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            //request.Response.ContentType = "application/json";
            //requestContext.Response.ContentEncoding = Encoding.UTF8;

            //string old_uid = "";

            //// 按条件查询
            //List<DBTablePreset> list = await MySqlUtil.getInstance().getTableData("log_login_old", new List<TableKeyObj>() { new TableKeyObj("machine_id", TableKeyObj.ValueType.ValueType_string, machine_id), new TableKeyObj("game_id", TableKeyObj.ValueType.ValueType_string, game_id) });

            //if (list.Count > 0)
            //{
            //    old_uid = list[0].keyList[0].m_value.ToString();
            //}

            //JsonData jsonData = new JsonData();
            //jsonData["old_uid"] = old_uid;

            //string backData = jsonData.ToJson();
            //Console.WriteLine(CommonUtil.getCurTime() + "----返回客户端数据:" + backData);

            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(backData);
            request.Response.ContentLength64 = buffer.Length;
            Stream output = request.Response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }
    }
}
