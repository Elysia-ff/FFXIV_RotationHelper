using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIV_RotationHelper
{
    public class Rotation
    {
        public string Url { get; set; }
        public RotationData Data { get; set; }
        public bool Loop { get; set; }
        public List<SkillData> SkillList { get; set; }
    }
}
