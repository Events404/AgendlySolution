using Agendly.Data;
using DataAccess.Repository.IRepository;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataAccess.Repository.EventRepository;

namespace DataAccess.Repository
{
  
        public class EventRepository : Repository<Event>, EventIRepository
         {
            private readonly ApplicationDbContext dbContext;

            public EventRepository(ApplicationDbContext dbContext) : base(dbContext)
            {
                this.dbContext = dbContext;
            }
        }
}
