using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpUtil
{
    public abstract class DBTablePreset
    {
        public abstract void initKey();
        
        public string tableName = "";
        public List<TableKeyObj> keyList = new List<TableKeyObj>();
        
        public object getKeyValue(string key)
        {
            for (int i = 0; i < keyList.Count; i++)
            {
                if (keyList[i].m_name.CompareTo(key) == 0)
                {
                    return keyList[i].m_value;
                }
            }

            return null;
        }
    }

    public class TableKeyObj
    {
        public enum ValueType
        {
            ValueType_int,
            ValueType_float,
            ValueType_string,
        }

        public string m_name;
        public ValueType m_valueType;
        public object m_value;

        public TableKeyObj(string name, ValueType valueType)
        {
            m_name = name;
            m_valueType = valueType;
        }

        public TableKeyObj(string name, ValueType valueType,object value)
        {
            m_name = name;
            m_valueType = valueType;
            m_value = value;
        }
    }

    //--------------------------------------------------------------------------------------------------

    public class DBTableManager
    {
        static DBTableManager s_instance = null;

        List<DBTablePreset> tableList = new List<DBTablePreset>();

        public static DBTableManager getInstance()
        {
            if (s_instance == null)
            {
                s_instance = new DBTableManager();
            }

            return s_instance;
        }

        public void init()
        {
            UserInfo_Preset table = new UserInfo_Preset("userinfo");
            table.initKey();

            tableList.Add(table);
        }

        public List<DBTablePreset> getTableList()
        {
            return tableList;
        }

        public DBTablePreset getDBTablePreset(string tableName)
        {
            for(int i = 0; i < tableList.Count; i++)
            {
                if (tableList[i].tableName.CompareTo(tableName) == 0)
                {
                    return tableList[i];
                }
            }

            return null;
        }
    }
}
