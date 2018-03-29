using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpUtil
{
    public class UserInfo_Preset : DBTablePreset
    {
        public UserInfo_Preset(string name)
        {
            tableName = name;
        }

        public override void initKey()
        {
            keyList.Add(new TableKeyObj("id", TableKeyObj.ValueType.ValueType_int));
            keyList.Add(new TableKeyObj("account", TableKeyObj.ValueType.ValueType_string));
            keyList.Add(new TableKeyObj("psw", TableKeyObj.ValueType.ValueType_string));
            keyList.Add(new TableKeyObj("name", TableKeyObj.ValueType.ValueType_string));
            keyList.Add(new TableKeyObj("sex", TableKeyObj.ValueType.ValueType_int));
        }
    }
}
