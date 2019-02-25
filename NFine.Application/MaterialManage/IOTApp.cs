/*******************************************************************************
 * Copyright © 2017 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/
using NFine.Code;
using NFine.Domain.Entity.MaterialManage;
using NFine.Domain.IRepository.MaterialManage;
using NFine.Repository.MaterialManage;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using NFine.Code.Excel;

namespace NFine.Application.MaterialManage
{
    public class IOTApp
    {
        private readonly IIotRepository _service = new IotRepository();
        public List<IOTEntity> GetList(Pagination pagination, string queryJson)
        {
            var expression = ExtLinq.True<IOTEntity>();
            var queryParam = queryJson.ToJObject();
            if (!queryParam["keyword"].IsEmpty())
            {
                string keyword = queryParam["keyword"].ToString();
                expression = expression.And(t => t.M_ICCID.Contains(keyword));
            }
            return _service.FindList(expression, pagination);
        }

        public void DownLoad()
        {
            var expression = ExtLinq.True<IOTEntity>();
            var iotList = _service.IQueryable();
            var dt = new DataTable();
            if (iotList != null && iotList.Any())
            {
                dt.Columns.AddRange(new[]
                {
                    new DataColumn("序号"),
                    new DataColumn("批次"),
                    new DataColumn("采购日期"),
                    new DataColumn("IEMI"),
                    new DataColumn("SN"),
                    new DataColumn("IMSI"),
                    new DataColumn("ICCID"),
                    new DataColumn("卡号"),
                    new DataColumn("开户日期"),
                    new DataColumn("开户经办人"),
                    new DataColumn("使用硬件"),
                    new DataColumn("货号"),
                    new DataColumn("表类型"),
                    new DataColumn("使用客户"),
                    new DataColumn("模组所在地"),
                    new DataColumn("余额"),
                    new DataColumn("客户使用数量")
                });
                foreach (var en in iotList)
                {
                    var row = dt.NewRow();
                    row[0] = en.M_ID;
                    row[1] = en.M_Batch;
                    row[2] = en.M_PurchaseTime;
                    row[3] = en.M_IMEI;
                    row[4] = en.M_SN;
                    row[5] = en.M_IMSI;
                    row[6] = en.M_ICCID;
                    row[7] = en.M_CardID;
                    row[8] = en.M_OpenAccountTime;
                    row[9] = en.M_OpenAccountUserName;
                    row[10] = en.M_HardwareName;
                    row[11] = en.M_ArtNo;
                    row[12] = en.M_MeterTypeName;
                    row[13] = en.M_CustomerName;
                    row[14] = en.M_Location;
                    row[15] = en.M_Balance;
                    row[16] = en.M_Remarks;
                    dt.Rows.Add(row);
                }
            }
            NPOIExcel.ExportByWeb(dt, "M6310模组记录", "M6310模组记录.xls");
        }

        public bool UpLoad(Stream stream)
        {
            var count = 0;
            var list = new List<IOTEntity>();
            var ds = NPOIExcel.ImportExceltoDs(stream);
            if (ds == null) return false;
            var dt = ds.Tables[0];
            if (dt == null || dt.Rows.Count <= 0) return false;
            foreach (DataRow item in dt.Rows)
            {
                DateTime? purchaseTime = null;
                DateTime? openAccountTime = null;
                var row = item;
                if (!string.IsNullOrEmpty(row[2] == null ? "" : row[2].ToString()))
                    purchaseTime = DateTime.FromOADate(double.Parse(row[2] == null ? "" : row[2].ToString()));
                if (!string.IsNullOrEmpty(row[2] == null ? "" : row[8].ToString()))
                    openAccountTime = DateTime.FromOADate(double.Parse(row[8] == null ? "" : row[8].ToString())); ;
                var iccid = (string)row[6];
                var en = _service.FindEntity(t => t.M_ICCID == iccid);
                if (en == null)
                {
                    list.Add(new IOTEntity
                    {
                        M_IMEI = row[3] == null ? "" : row[3].ToString(),
                        M_SN = row[4] == null ? "" : row[4].ToString(),
                        M_IMSI = row[5] == null ? "" : row[5].ToString(),
                        M_ICCID = row[6] == null ? "" : row[6].ToString(),
                        M_CardID = row[7] == null ? "" : row[7].ToString(),
                        M_HardwareName = row[10] == null ? "" : row[10].ToString(),
                        M_ArtNo = row[11] == null ? "" : row[11].ToString(),
                        M_MeterTypeName = row[12] == null ? "" : row[12].ToString(),
                        M_CustomerName = row[13] == null ? "" : row[13].ToString(),
                        M_Location = row[14] == null ? "" : row[14].ToString(),
                        M_Balance = row[15] == null ? "" : row[15].ToString(),
                        M_Remarks = row[16] == null ? "" : row[16].ToString(),
                        M_Batch = row[1] == null ? "" : row[1].ToString(),
                        M_PurchaseTime = purchaseTime,
                        M_OpenAccountTime = openAccountTime,
                        M_OpenAccountUserName = row[9] == null ? "" : row[9].ToString(),
                    });
                }
                else
                {
                    en.M_IMEI = row[3] == null ? "" : row[3].ToString();
                    en.M_SN = row[4] == null ? "" : row[4].ToString();
                    en.M_IMSI = row[5] == null ? "" : row[5].ToString();
                    en.M_ICCID = row[6] == null ? "" : row[6].ToString();
                    en.M_CardID = row[7] == null ? "" : row[7].ToString();
                    en.M_HardwareName = row[10] == null ? "" : row[10].ToString();
                    en.M_ArtNo = row[11] == null ? "" : row[11].ToString();
                    en.M_MeterTypeName = row[12] == null ? "" : row[12].ToString();
                    en.M_CustomerName = row[13] == null ? "" : row[13].ToString();
                    en.M_Location = row[14] == null ? "" : row[14].ToString();
                    en.M_Balance = row[15] == null ? "" : row[15].ToString();
                    en.M_Remarks = row[16] == null ? "" : row[16].ToString();
                    en.M_Batch = row[1] == null ? "" : row[1].ToString();
                    en.M_PurchaseTime = purchaseTime;
                    en.M_OpenAccountTime = openAccountTime;
                    en.M_OpenAccountUserName = row[9] == null ? "" : row[9].ToString();
                    count += _service.Update(en);
                }
            }
            count += _service.Insert(list);
            return count > 0;
        }

        public IOTEntity GetFormByIccid(string iccid)
        {
            var data = _service.FindEntity(x => x.M_ICCID == iccid);
            return data;
        }
        public IOTEntity GetFormByImei(string imei)
        {
            var data = _service.FindEntity(x => x.M_IMEI == imei);
            return data;
        }
    }
}