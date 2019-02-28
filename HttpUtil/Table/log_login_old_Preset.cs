using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpUtil
{
    public class log_login_old_Preset : DBTablePreset
    {
        public log_login_old_Preset(string name)
        {
            tableName = name;
        }

        public override void initKey()
        {
            keyList.Add(new TableKeyObj("uid_old", TableKeyObj.ValueType.ValueType_string));
            keyList.Add(new TableKeyObj("game_id", TableKeyObj.ValueType.ValueType_int));
            keyList.Add(new TableKeyObj("channel_name", TableKeyObj.ValueType.ValueType_string));
            keyList.Add(new TableKeyObj("machine_id", TableKeyObj.ValueType.ValueType_string));
            keyList.Add(new TableKeyObj("version_name", TableKeyObj.ValueType.ValueType_string));
            keyList.Add(new TableKeyObj("create_time", TableKeyObj.ValueType.ValueType_string));
        }
    }
}
