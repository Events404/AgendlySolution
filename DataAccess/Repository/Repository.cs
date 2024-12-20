﻿using Agendly.Data;
using DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class

    {

        //ApplicationDbContext dbContext = new ApplicationDbContext();
        private readonly ApplicationDbContext dbContext;
        private DbSet<T> dbSet;

        public Repository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
            dbSet = dbContext.Set<T>();
        }

        public List<T> GatAll(string? include = null)
        {
            return include == null ? dbSet.ToList() : dbSet.Include(include).ToList();
        }

        // CRUD operations
        public IEnumerable<T> Get(Expression<Func<T, object>>[]? includeProp = null, Expression<Func<T, bool>> expression = null)
        {
            IQueryable<T> query = dbSet;

            if (expression != null)
            {
                query = query.Where(expression);
            }

            if (includeProp != null)
            {
                foreach (var prop in includeProp)
                {
                    query = query.Include(prop);
                }
            }

            return query.ToList();
        }

        public IEnumerable<T> GetO(Expression<Func<T, object>>[]? includeProp = null, string? includeString = null, Expression<Func<T, bool>>? expression = null)
        {
            IQueryable<T> query = dbSet;

            if (expression != null)
            {
                query = query.Where(expression);
            }

            // تضمين خصائص متعددة باستخدام Expressions (مثل Include Prop)
            if (includeProp != null)
            {
                foreach (var prop in includeProp)
                {
                    query = query.Include(prop);
                }
            }

            // تضمين خاصية باستخدام string
            if (!string.IsNullOrEmpty(includeString))
            {
                query = query.Include(includeString);
            }

            return query.ToList();
        }

        public T? GetOne(Expression<Func<T, bool>> expression)
        {
            return dbSet.Where(expression).FirstOrDefault();
        }

        public T? GetFirstOrDefault(Expression<Func<T, bool>> expression, Expression<Func<T, object>>[]? includeProp = null)
        {
            IQueryable<T> query = dbSet;

            if (expression != null)
            {
                query = query.Where(expression);
            }

            if (includeProp != null)
            {
                foreach (var prop in includeProp)
                {
                    query = query.Include(prop);
                }
            }

            return query.FirstOrDefault();
        }


        public T? GetById(int entityId)
        {
            return dbSet.Find(entityId);
        }

        public void create(T entity)
        {
            dbSet.Add(entity);
        }

        public void Edit(T entity)
        {
            dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            dbSet.Remove(entity);
        }
        public void commit()
        {
            dbContext.SaveChanges();

        }
        public int Count(Expression<Func<T, bool>>? expression = null)
        {
            if (expression != null)
            {
                return dbSet.Count(expression);
            }
            return dbSet.Count();
        }
    }
}
