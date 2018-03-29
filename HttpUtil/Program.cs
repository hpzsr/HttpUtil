using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace HttpUtil
{
    class Program
    {
        private static HttpListener httpRequest;   // 请求监听  
        private static HttpListenerContext requestContext;

        static void Main(string[] args)
        {
            MySqlUtil.getInstance().openDatabase();
            
            DBTableManager.getInstance().init();

            // 遍历整个表
            List<DBTablePreset> list = MySqlUtil.getInstance().queryDatabaseTable("userinfo");
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < list[i].keyList.Count; j++)
                {
                    Console.Write(list[i].keyList[j].m_value + "    ");
                }

                Console.WriteLine();
            }

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

            {
                httpRequest = new HttpListener();
                //httpRequest.Prefixes.Add("http://www.huangpin.xyz/");  //添加监听地址 注意是以/结尾

                string name = Dns.GetHostName();
                IPAddress[] ipadrlist = Dns.GetHostAddresses(name);
                Console.WriteLine("本机ip:" + ipadrlist[1].ToString());

                httpRequest.Prefixes.Add("http://" + ipadrlist[1].ToString() + "/");
                httpRequest.Start(); //允许该监听地址接受请求的传入。  

                GethttpRequestAsync();

                Console.WriteLine("开始监听客户端请求:");
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

                    onHttpHandle(requestContext);
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

        public static void onHttpHandle(object reecontext)
        {
            HttpListenerContext request = (HttpListenerContext)reecontext;
            string msg = HttpUtility.UrlDecode(request.Request.QueryString["name"]);        //接受GET请求过来的参数；
            
            Console.WriteLine("有客户端请求:" + request.Request.RemoteEndPoint.Address.ToString() + "  " + CommonUtil.getCurTime());

            request.Response.StatusCode = 200;
            request.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            request.Response.ContentType = "application/json";
            requestContext.Response.ContentEncoding = Encoding.UTF8;
            //byte[] buffer = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { success = true, behavior = msg }));
            //byte[] buffer = System.Text.Encoding.UTF8.GetBytes("你的名字：" + msg);
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes("hp love zhangshiran!");
            request.Response.ContentLength64 = buffer.Length;
            Stream output = request.Response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }
    }
}
