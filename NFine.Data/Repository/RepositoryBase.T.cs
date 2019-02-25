/*******************************************************************************
 * Copyright © 2017 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/
using NFine.Code;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace NFine.Data
{
    /// <summary>
    /// 仓储实现
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : class,new()
    {
        public NFineDbContext Dbcontext = new NFineDbContext();
        public string conn = DBConnection.connectionString;
        public int Insert(TEntity entity)
        {
           
            Dbcontext.Entry(entity).State = EntityState.Added;
            return Dbcontext.SaveChanges();
        }
        public int Insert(List<TEntity> entitys)
        {
            foreach (var entity in entitys)
            {
                Dbcontext.Entry(entity).State = EntityState.Added;
            }
            return Dbcontext.SaveChanges();
        }
        public int Update(TEntity entity)
        {
            Dbcontext.Set<TEntity>().Attach(entity);
            PropertyInfo[] props = entity.GetType().GetProperties();
            foreach (PropertyInfo prop in props)
            {
                if (prop.GetValue(entity, null) != null)
                {
                    if (prop.GetValue(entity, null).ToString() == "&nbsp;")
                        Dbcontext.Entry(entity).Property(prop.Name).CurrentValue = null;
                    Dbcontext.Entry(entity).Property(prop.Name).IsModified = true;
                }
            }
            return Dbcontext.SaveChanges();
        }
        public int Delete(TEntity entity)
        {
            Dbcontext.Set<TEntity>().Attach(entity);
            Dbcontext.Entry(entity).State = EntityState.Deleted;
            return Dbcontext.SaveChanges();
        }
        public int Delete(Expression<Func<TEntity, bool>> predicate)
        {
            var entitys = Dbcontext.Set<TEntity>().Where(predicate).ToList();
            entitys.ForEach(m => Dbcontext.Entry(m).State = EntityState.Deleted);
            return Dbcontext.SaveChanges();
        }
        public TEntity FindEntity(object keyValue)
        {
            return Dbcontext.Set<TEntity>().Find(keyValue);
        }
        public TEntity FindEntity(Expression<Func<TEntity, bool>> predicate)
        {
            return Dbcontext.Set<TEntity>().FirstOrDefault(predicate);
        }
        public IQueryable<TEntity> IQueryable()
        {
            return Dbcontext.Set<TEntity>();
        }
        public IQueryable<TEntity> IQueryable(Expression<Func<TEntity, bool>> predicate)
        {
            return Dbcontext.Set<TEntity>().Where(predicate);
        }
        public List<TEntity> FindList(string strSql)
        {
            return Dbcontext.Database.SqlQuery<TEntity>(strSql).ToList();
        }
        public List<TEntity> FindList(string strSql, DbParameter[] dbParameter)
        {
            return Dbcontext.Database.SqlQuery<TEntity>(strSql, dbParameter).ToList<TEntity>();
        }
        public List<TEntity> FindList(Pagination pagination)
        {
            bool isAsc = pagination.sord.ToLower() == "asc" ? true : false;
            string[] _order = pagination.sidx.Split(',');
            MethodCallExpression resultExp = null;
            var tempData = Dbcontext.Set<TEntity>().AsQueryable();
            foreach (string item in _order)
            {
                string _orderPart = item;
                _orderPart = Regex.Replace(_orderPart, @"\s+", " ");
                string[] _orderArry = _orderPart.Split(' ');
                string _orderField = _orderArry[0];
                bool sort = isAsc;
                if (_orderArry.Length == 2)
                {
                    isAsc = _orderArry[1].ToUpper() == "ASC" ? true : false;
                }
                var parameter = Expression.Parameter(typeof(TEntity), "t");
                var property = typeof(TEntity).GetProperty(_orderField);
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var orderByExp = Expression.Lambda(propertyAccess, parameter);
                resultExp = Expression.Call(typeof(Queryable), isAsc ? "OrderBy" : "OrderByDescending", new Type[] { typeof(TEntity), property.PropertyType }, tempData.Expression, Expression.Quote(orderByExp));
            }
            tempData = tempData.Provider.CreateQuery<TEntity>(resultExp);
            pagination.records = tempData.Count();
            tempData = tempData.Skip<TEntity>(pagination.rows * (pagination.page - 1)).Take<TEntity>(pagination.rows).AsQueryable();
            return tempData.ToList();
        }
        public List<TEntity> FindList(string strSql, Pagination pagination)
        {
            bool isAsc = pagination.sord.ToLower() == "asc";
            string[] _order = pagination.sidx.Split(',');
            MethodCallExpression resultExp = null;
            var tempData = Dbcontext.Database.SqlQuery<TEntity>(strSql).AsQueryable();
            foreach (string item in _order)
            {
                string orderPart = item;
                orderPart = Regex.Replace(orderPart, @"\s+", " ");
                string[] orderArry = orderPart.Split(' ');
                string orderField = orderArry[0];
                if (orderArry.Length == 2)
                {
                    isAsc = orderArry[1].ToUpper() == "ASC";
                }
                var parameter = Expression.Parameter(typeof(TEntity), "t");
                var property = typeof(TEntity).GetProperty(orderField);
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var orderByExp = Expression.Lambda(propertyAccess, parameter);
                resultExp = Expression.Call(typeof(Queryable), isAsc ? "OrderBy" : "OrderByDescending", new Type[] { typeof(TEntity), property.PropertyType }, tempData.Expression, Expression.Quote(orderByExp));
            }
            tempData = tempData.Provider.CreateQuery<TEntity>(resultExp);
            pagination.records = tempData.Count();
            tempData = tempData.Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows).AsQueryable();
            return tempData.ToList();

        }
        public List<TEntity> FindList(Expression<Func<TEntity, bool>> predicate, Pagination pagination)
        {
            bool isAsc = pagination.sord.ToLower() == "asc";
            string[] _order = pagination.sidx.Split(',');
            MethodCallExpression resultExp = null;
            var tempData = Dbcontext.Set<TEntity>().Where(predicate);
            foreach (string item in _order)
            {
                string orderPart = item;
                orderPart = Regex.Replace(orderPart, @"\s+", " ");
                string[] orderArry = orderPart.Split(' ');
                string orderField = orderArry[0];
                if (orderArry.Length == 2)
                {
                    isAsc = orderArry[1].ToUpper() == "ASC";
                }
                var parameter = Expression.Parameter(typeof(TEntity), "t");
                var property = typeof(TEntity).GetProperty(orderField);
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var orderByExp = Expression.Lambda(propertyAccess, parameter);
                resultExp = Expression.Call(typeof(Queryable), isAsc ? "OrderBy" : "OrderByDescending", new Type[] { typeof(TEntity), property.PropertyType }, tempData.Expression, Expression.Quote(orderByExp));
            }
            tempData = tempData.Provider.CreateQuery<TEntity>(resultExp);
            pagination.records = tempData.Count();
            tempData = tempData.Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows).AsQueryable();
            return tempData.ToList();
        }
        public List<TEntity> FindList(string strSql, Expression<Func<TEntity, bool>> predicate, Pagination pagination)
        {
            bool isAsc = pagination.sord.ToLower() == "asc";
            string[] _order = pagination.sidx.Split(',');
            MethodCallExpression resultExp = null;
            var tempData = Dbcontext.Database.SqlQuery<TEntity>(strSql).AsQueryable().Where(predicate);
            foreach (string item in _order)
            {
                string orderPart = item;
                orderPart = Regex.Replace(orderPart, @"\s+", " ");
                string[] orderArry = orderPart.Split(' ');
                string orderField = orderArry[0];
                if (orderArry.Length == 2)
                {
                    isAsc = orderArry[1].ToUpper() == "ASC";
                }
                var parameter = Expression.Parameter(typeof(TEntity), "t");
                var property = typeof(TEntity).GetProperty(orderField);
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var orderByExp = Expression.Lambda(propertyAccess, parameter);
                resultExp = Expression.Call(typeof(Queryable), isAsc ? "OrderBy" : "OrderByDescending", new Type[] { typeof(TEntity), property.PropertyType }, tempData.Expression, Expression.Quote(orderByExp));
            }
            tempData = tempData.Provider.CreateQuery<TEntity>(resultExp).AsQueryable();
            pagination.records = tempData.Count();
            tempData = tempData.Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows).AsQueryable();
            return tempData.ToList();
        }
    }
}