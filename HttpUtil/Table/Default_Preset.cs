using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpUtil
{
    class Default_Preset : DBTablePreset
    {
        public Default_Preset(string name)
        {
            tableName = name;
        }

        public override void initKey()
        {
        }
    }
}
