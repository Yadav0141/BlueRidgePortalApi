using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueRidgeUtility_BAL.Models
{
    public class PowershellBKPFileCopyModel
    {
        public string sourceusername { get; set; }
        public string sourcepassword { get; set; }
        public string destinationusername { get; set; }
        public string destinationpassword { get; set; }
        public string source { get; set; }
        public string target { get; set; }

        public string FromRemoteServer { get; set; }
        public string ToRemoteServer { get; set; }
        public string FolderBKPPath { get; set; }
        public string SourceBKPPath { get; set; }
    }
}
