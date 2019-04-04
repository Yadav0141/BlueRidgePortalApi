using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueRidgeUtility_BAL.Models
{
    public class UserListViewModel
    {
        public List<UserListModel> lstUsers { get; set; }
        public int totalCount { get; set; }
    }
    public class UserListModel
    {
        public Guid userId { get; set; }
        public string employeeId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string reportingTo { get; set; }
        public string designation { get; set; }

        public int srNo { get; set; }
    }
}
