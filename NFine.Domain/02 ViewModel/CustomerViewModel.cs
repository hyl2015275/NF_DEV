using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NFine.Domain.Entity.SystemManage;

namespace NFine.Domain.ViewModel
{
    public class CustomerViewModel
    {
        public string F_Id { get { return CompanyInfo.F_Id ?? ""; } }
        public string R_Id { get { return DefaultRole.F_Id ?? ""; } }
        public string U_Id { get { return DefaultUser.F_Id ?? ""; } }
        public int? F_Layers { get { return CompanyInfo.F_Layers; } }
        public string F_EnCode { get { return CompanyInfo.F_EnCode ?? ""; } }
        public string F_ShortName { get { return CompanyInfo.F_ShortName ?? ""; } }
        public string F_CategoryId { get { return CompanyInfo.F_CategoryId ?? ""; } }
        public string F_TelePhone { get { return CompanyInfo.F_TelePhone ?? ""; } }
        public string F_WeChat { get { return CompanyInfo.F_WeChat ?? ""; } }
        public string F_Fax { get { return CompanyInfo.F_Fax ?? ""; } }
        public string F_Email { get { return CompanyInfo.F_Email ?? ""; } }
        public string F_AreaId { get { return CompanyInfo.F_AreaId ?? ""; } }
        public int? F_SortCode { get { return CompanyInfo.F_SortCode; } }
        public string F_FullName { get { return CompanyInfo.F_FullName ?? ""; } }
        public string F_ManagerId { get { return CompanyInfo.F_ManagerId ?? ""; } }
        public string F_MobilePhone { get { return CompanyInfo.F_MobilePhone ?? ""; } }
        public string F_Address { get { return CompanyInfo.F_Address ?? ""; } }
        public string F_Account { get { return DefaultUser.F_Account ?? ""; } }
        public bool? F_EnabledMark { get { return CompanyInfo.F_EnabledMark; } }
        public string F_Description { get { return CompanyInfo.F_Description ?? ""; } }
        public string F_CreatorUserId { get { return CompanyInfo.F_CreatorUserId ?? ""; } }
        public DateTime? F_CreatorTime { get { return CompanyInfo.F_CreatorTime; } }
        public string F_LastModifyUserId { get { return CompanyInfo.F_LastModifyUserId ?? ""; } }
        public DateTime? F_LastModifyTime { get { return CompanyInfo.F_LastModifyTime; } }
        public OrganizeEntity CompanyInfo { get; set; }
        public UserEntity DefaultUser { get; set; }
        public RoleEntity DefaultRole { get; set; }
    }
}
