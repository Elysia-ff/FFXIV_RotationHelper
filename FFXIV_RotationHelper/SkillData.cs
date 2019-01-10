using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FFXIV_RotationHelper
{
    public class SkillData
    {
        /*
        Count 652
        
        4096:
        {
            icon:"001072",
            help_html:"Increases the chance of obtaining an HQ item on your next gathering attempt by 10%. Does not increase chances for items which start at 0%.",
            recast_time:0,
            category:6,
            deprecated:null,
            radius:0,
            name:"Prune",
            level:58,
            cost:100,
            cast_time:0,
            cast_range:0
        },
        */

        public int Idx { get; private set; }
        public string IconURL { get; private set; }
        public string Name { get; private set; }

        private static readonly Regex idxRegex = new Regex(@"^\d+");                        // 4096
        private static readonly Regex iconURLRegex = new Regex(@"icon:""((\d|\w|\s)+)"","); // icon:"001072",
        private static readonly Regex nameRegex = new Regex(@"name:""((\w|\s)+)"",");       // name:"Prune",

        public const string IconURLFormat = "http://ffxivrotations.com/icons/{0}.png";

        public void Load(string data)
        {
            string idxStr = idxRegex.Match(data).Value;
            Idx = int.Parse(idxStr);

            string iconURLStr = iconURLRegex.Match(data).Groups[1].Value;
            IconURL = string.Format(IconURLFormat, iconURLStr);

            string nameStr = nameRegex.Match(data).Groups[1].Value;
            Name = nameStr;
        }
    }
}
