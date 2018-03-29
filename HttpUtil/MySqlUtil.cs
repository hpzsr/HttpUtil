using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Threading;
using System.IO;
using LitJson;

namespace HttpUtil
{
    class LockObject : Object
    {
        public bool m_isLock;

        public LockObject()
        {
            m_isLock = false;
        }
    }

    class MySqlUtil
    {
        static MySqlUtil s_mySqlUtil = null;

        MySqlConnection m_mySqlConnection;

        LockObject m_lockObject = new LockObject();

        public static MySqlUtil getInstance()
        {
            if (s_mySqlUtil == null)
            {
                s_mySqlUtil = new MySqlUtil();
            }

            return s_mySqlUtil;
        }

        public void Lock()
        {
            m_lockObject.m_isLock = true;
        }

        public void UnLock()
        {
            m_lockObject.m_isLock = false;
        }

        // 打开数据库
        public void openDatabase()
        {
            string ip = "";
            int port = 0;
            string user = "";
            string password = "";
            string databaseName = "";

            // 读取文件
            {
                StreamReader sr = new StreamReader("data/MySqlConfig.json", System.Text.Encoding.GetEncoding("utf-8"));
                string config = sr.ReadToEnd().ToString();
                sr.Close();

                JsonData jsonData = JsonMapper.ToObject(config);

                ip = jsonData["ip"].ToString();
                port = (int)jsonData["port"];
                user = jsonData["user"].ToString();
                password = jsonData["password"].ToString();
                databaseName = jsonData["databaseName"].ToString();
            }

            string conn = string.Format("Data Source= {0}; Port= {1} ; User ID = {2} ; Password = {3} ; DataBase = {4} ; Charset = utf8" , ip , port , user , password , databaseName);
            m_mySqlConnection = new MySqlConnection(conn);

            //进行数据库连接
            m_mySqlConnection.Open();

            Console.WriteLine("数据库打开成功");
        }

        // 数据库查询-遍历整个表
        public List<DBTablePreset> queryDatabaseTable(string table)
        {
            MySqlCommand cmd = new MySqlCommand("select * from " + table, m_mySqlConnection);
            MySqlDataReader dr = cmd.ExecuteReader();   //读出数据

            // 读出来是否有数据
            //Console.WriteLine("----"+dr.HasRows);

            List<DBTablePreset> listData = new List<DBTablePreset>();
            DBTablePreset baseTablePreset = DBTableManager.getInstance().getDBTablePreset(table);

            while (dr.Read())
            {
                DBTablePreset tempTablePreset = new Default_Preset(baseTablePreset.tableName);
                
                for (int i = 0; i < baseTablePreset.keyList.Count; i++)
                {
                    TableKeyObj tempTableKeyObj = new TableKeyObj(baseTablePreset.keyList[i].m_name, baseTablePreset.keyList[i].m_valueType);
                    tempTableKeyObj.m_value = getObjectByValueType(dr,i, tempTableKeyObj.m_valueType);
                    tempTablePreset.keyList.Add(tempTableKeyObj);
                }

                listData.Add(tempTablePreset);
            }

            dr.Close();

            return listData;
            //return dr;
        }

        // 数据库查询-按条件查询
        public List<DBTablePreset> getTableData(string table, List<TableKeyObj> keyObjList)
        {
            List<DBTablePreset> dbTablePresetList = new List<DBTablePreset>();

            string command = "select * from " + table + " where ";
            for (int i = 0; i < keyObjList.Count; i++)
            {
                if (keyObjList[i].m_valueType == TableKeyObj.ValueType.ValueType_string)
                {
                    command += (keyObjList[i].m_name + " = '");
                    command += keyObjList[i].m_value;
                    command += "'";
                }
                else
                {
                    command += (keyObjList[i].m_name + " = " + keyObjList[i].m_value);
                }

                if (i != (keyObjList.Count - 1))
                {
                    command += " and ";
                }
            }
            Console.WriteLine(command);

            MySqlDataReader dr = ExecuteCommandHasReturn(command);

            List<DBTablePreset> listData = new List<DBTablePreset>();
            DBTablePreset baseTablePreset = DBTableManager.getInstance().getDBTablePreset(table);

            while (dr.Read())
            {
                DBTablePreset tempTablePreset = new Default_Preset(baseTablePreset.tableName);

                for (int i = 0; i < baseTablePreset.keyList.Count; i++)
                {
                    TableKeyObj tempTableKeyObj = new TableKeyObj(baseTablePreset.keyList[i].m_name, baseTablePreset.keyList[i].m_valueType);
                    tempTableKeyObj.m_value = getObjectByValueType(dr, i, tempTableKeyObj.m_valueType);
                    tempTablePreset.keyList.Add(tempTableKeyObj);
                }

                listData.Add(tempTablePreset);
            }

            return listData;
        }

        // 增加数据
        public void insertData(string table, List<TableKeyObj> keyObjList)
        {
            string command = "insert into " + table + " (";
            for (int i = 0; i < keyObjList.Count; i++)
            {
                command += (keyObjList[i].m_name);

                if (i != (keyObjList.Count - 1))
                {
                    command += " , ";
                }
                else
                {
                    command += " ) values (";
                }
            }

            for (int i = 0; i < keyObjList.Count; i++)
            {
                if (keyObjList[i].m_valueType == TableKeyObj.ValueType.ValueType_string)
                {
                    command += "'";
                    command += keyObjList[i].m_value;
                    command += "'";
                }
                else
                {
                    command += (keyObjList[i].m_value);
                }

                if (i != (keyObjList.Count - 1))
                {
                    command += " , ";
                }
                else
                {
                    command += " )";
                }
            }

            Console.WriteLine(command);

            MySqlCommand cmd = new MySqlCommand(command, m_mySqlConnection);
            MySqlDataReader dr = cmd.ExecuteReader();
            dr.Close();
        }

        // 删除数据
        public void deleteData(string table, List<TableKeyObj> keyObjList)
        {
            string command = "delete from " + table + " where ";
            for (int i = 0; i < keyObjList.Count; i++)
            {
                if (keyObjList[i].m_valueType == TableKeyObj.ValueType.ValueType_string)
                {
                    command += (keyObjList[i].m_name + " = '");
                    command += keyObjList[i].m_value;
                    command += "'";
                }
                else
                {
                    command += (keyObjList[i].m_name + " = " + keyObjList[i].m_value);
                }

                if (i != (keyObjList.Count - 1))
                {
                    command += " and ";
                }
            }

            Console.WriteLine(command);

            MySqlCommand cmd = new MySqlCommand(command, m_mySqlConnection);
            MySqlDataReader dr = cmd.ExecuteReader();
            dr.Close();
        }

        // 修改数据
        public void updateData(string table, List<TableKeyObj> tianjian_keyObjList, List<TableKeyObj> change_keyObjList)
        {
            string command = "update " + table + " set ";//name = 'zsr' ,sex = 0 where id = 2";
            for (int i = 0; i < change_keyObjList.Count; i++)
            {
                command += (change_keyObjList[i].m_name + " = ");

                if (change_keyObjList[i].m_valueType == TableKeyObj.ValueType.ValueType_string)
                {
                    command += "'";
                    command += change_keyObjList[i].m_value;
                    command += "'";
                }
                else
                {
                    command += change_keyObjList[i].m_value;
                }

                if (i != (change_keyObjList.Count - 1))
                {
                    command += " , ";
                }
            }

            command += " where ";

            for (int i = 0; i < tianjian_keyObjList.Count; i++)
            {
                if (tianjian_keyObjList[i].m_valueType == TableKeyObj.ValueType.ValueType_string)
                {
                    command += (tianjian_keyObjList[i].m_name + " = '");
                    command += tianjian_keyObjList[i].m_value;
                    command += "'";
                }
                else
                {
                    command += (tianjian_keyObjList[i].m_name + " = " + tianjian_keyObjList[i].m_value);
                }

                if (i != (tianjian_keyObjList.Count - 1))
                {
                    command += " and ";
                }
            }

            Console.WriteLine(command);

            MySqlCommand cmd = new MySqlCommand(command, m_mySqlConnection);
            MySqlDataReader dr = cmd.ExecuteReader();
            dr.Close();
        }

        public void ExecuteCommand(string command)
        {
            lock (m_lockObject)
            {
                while (m_lockObject.m_isLock)
                {
                    // 数据库同一时间只能干一件事
                }

                try
                {
                    Lock();

                    MySqlCommand cmd = new MySqlCommand(command, m_mySqlConnection);
                    MySqlDataReader dr = cmd.ExecuteReader();
                    dr.Close();

                    UnLock();
                }
                catch (Exception ex)
                {
                    //LogUtil.getInstance().writeErrorLog("错误信息：" + ex.Message.ToString() + "\r\n");
                    Console.WriteLine("错误信息：" + ex.Message.ToString() + "\r\n"); ;
                    UnLock();

                    throw ex;
                }
            }
        }

        public MySqlDataReader ExecuteCommandHasReturn(string command)
        {
            lock (m_lockObject)
            {
                while (m_lockObject.m_isLock)
                {
                    // 数据库同一时间只能干一件事
                }

                try
                {
                    Lock();

                    MySqlCommand cmd = new MySqlCommand(command, m_mySqlConnection);
                    MySqlDataReader dr = cmd.ExecuteReader();

                    return dr;
                }
                catch (Exception ex)
                {
                    //LogUtil.getInstance().writeErrorLog("错误信息：" + ex.Message.ToString() + "\r\n");
                    Console.WriteLine("错误信息：" + ex.Message.ToString() + "\r\n");
                    UnLock();

                    throw ex;
                }
            }
        }

        // 关闭数据库
        public void closeDatabase()
        {
            m_mySqlConnection.Close();
        }

        //-----------------------------------------------------
        public object getObjectByValueType(MySqlDataReader dr, int index, TableKeyObj.ValueType valueType)
        {
            object obj = null;

            switch (valueType)
            {
                case TableKeyObj.ValueType.ValueType_int:
                    {
                        obj = dr.GetInt32(index);
                    }
                    break;

                case TableKeyObj.ValueType.ValueType_float:
                    {
                        obj = dr.GetFloat(index);
                    }
                    break;

                case TableKeyObj.ValueType.ValueType_string:
                    {
                        obj = dr.GetString(index);
                    }
                    break;
            }

            return obj;
        }
    }
}
