/*******************************************************************************
 * Copyright © 2017 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/
using NFine.Application.SystemManage;
using NFine.Code;
using NFine.Domain.Entity.SystemManage;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using NFine.Domain.Enum;

namespace NFine.Web.Areas.SystemManage.Controllers
{
    public class RoleAuthorizeController : ControllerBase
    {
        private RoleAuthorizeApp roleAuthorizeApp = new RoleAuthorizeApp();
        private RoleApp roleApp = new RoleApp();
        private ModuleApp moduleApp = new ModuleApp();
        private ModuleButtonApp moduleButtonApp = new ModuleButtonApp();

        public ActionResult GetPermissionTree(string roleId, CustomerTypeEnum customerType)
        {
            var moduledata = customerType != CustomerTypeEnum.Both
                ? moduleApp.GetList(customerType)
                : OperatorProvider.Provider.GetCurrent().IsSystem
                    ? moduleApp.GetList(CustomerTypeEnum.System)
                    : OperatorProvider.Provider.GetCurrent().IsPlatform
                        ? moduleApp.GetList(CustomerTypeEnum.Platform)
                        : moduleApp.GetList(CustomerTypeEnum.Organization);
            var buttondata = moduleButtonApp.GetList();
            var authorizedata = new List<RoleAuthorizeEntity>();
            if (!string.IsNullOrEmpty(roleId))
            {
                authorizedata = roleAuthorizeApp.GetList(roleId);
            }
            var defaultauthorizedata = new List<RoleAuthorizeEntity>();
            if (!string.IsNullOrEmpty(OperatorProvider.Provider.GetCurrent().CompanyId))
            {
                var role = roleApp.GetDefaultCompanyRole(OperatorProvider.Provider.GetCurrent().CompanyId);
                if (role != null)
                    defaultauthorizedata = roleAuthorizeApp.GetList(role.F_Id);
            }
            var treeList = (from item in moduledata
                            where defaultauthorizedata.Count(t => t.F_ItemId == item.F_Id) > 0 || OperatorProvider.Provider.GetCurrent().IsSystem || customerType != CustomerTypeEnum.Both
                            select new TreeViewModel
                            {
                                id = item.F_Id,
                                text = item.F_FullName,
                                value = item.F_EnCode,
                                parentId = item.F_ParentId,
                                isexpand = true,
                                complete = true,
                                showcheck = true,
                                checkstate = authorizedata.Count(t => t.F_ItemId == item.F_Id),
                                hasChildren = true,
                                img = item.F_Icon == "" ? "" : item.F_Icon
                            }).ToList();

            treeList.AddRange(from item in buttondata
                              where defaultauthorizedata.Count(t => t.F_ItemId == item.F_Id) > 0 || OperatorProvider.Provider.GetCurrent().IsSystem || customerType != CustomerTypeEnum.Both
                              select new TreeViewModel
                              {
                                  id = item.F_Id,
                                  text = item.F_FullName,
                                  value = item.F_EnCode,
                                  parentId = item.F_ParentId == "0" ? item.F_ModuleId : item.F_ParentId,
                                  isexpand = true,
                                  complete = true,
                                  showcheck = true,
                                  checkstate = authorizedata.Count(t => t.F_ItemId == item.F_Id),
                                  hasChildren = buttondata.Count(t => t.F_ParentId == item.F_Id) != 0,
                                  img = item.F_Icon == "" ? "" : item.F_Icon
                              });
            return Content(treeList.TreeViewJson());
        }
    }
}