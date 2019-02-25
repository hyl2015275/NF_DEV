using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFine.Domain.Entity.SystemManage
{
    public class FamilyEntity
    {
        public string F_Id { get; set; }
        public string F_Name { get; set; }
        public string F_Password { get; set; }
        public string F_NickName { get; set; }
        public string F_Photo { get; set; }
        public string F_Email { get; set; }
        public bool? F_Sex { get; set; }
        public DateTime? F_BornTime { get; set; }
        public string F_Mobile { get; set; }
        public string F_Tel { get; set; }
        public string F_Qq { get; set; }
        public string F_Wechat { get; set; }
        public string F_Alipy { get; set; }
        public string F_OpenId { get; set; }
        public string F_LoginIp { get; set; }
        public DateTime? F_LoginTime { get; set; }
        public string F_LastLoginIp { get; set; }
        public DateTime? F_LastLoginTime { get; set; }
        public string F_RegisterIp { get; set; }
        public DateTime? F_RegisterTime { get; set; }
        public int? F_State { get; set; }
    }
}
