using Agendly.Data;
using DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
   
    public class LikeDisLikeRepository : Repository<LikeDislike>, LikeDisLikeIRepository
    {
        private readonly ApplicationDbContext dbContext;

        public LikeDisLikeRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public LikeDislike GetByUserAndEvent(string userId, int eventId)
        {
            return dbContext.LikeDislikes.FirstOrDefault(ld => ld.UserId == userId && ld.EventId == eventId);
        }
    }
}
