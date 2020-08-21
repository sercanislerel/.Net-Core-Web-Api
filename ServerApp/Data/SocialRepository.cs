using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ServerApp.Models;
using System.Linq;

namespace ServerApp.Data
{
    public class SocialRepository : ISocialRepository
    {   private readonly SocialContext _context;
        public SocialRepository(SocialContext context)
        {
            _context=context;
        }

        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);  
            


        }

        public void Delete<T>(T entity) where T : class
        {   
            _context.Remove(entity);
        }

        public async Task<User> GetUser(int id)
        {
            var user= await _context.Users.Include(i=>i.Images).FirstOrDefaultAsync(i=>i.Id==id);
            return user;
        }

        public async Task<IEnumerable<User>> GetUsers(UserQueryParams userQueryParams)
        {
            var users =  _context.Users.Where(i =>i.Id !=userQueryParams.UserId).Include(i=>i.Images).AsQueryable();

            if(userQueryParams.Followers)
            {
                var result = await GetFollows(userQueryParams.UserId,false);
                users =users.Where(u => result.Contains(u.Id));
            }
            if(userQueryParams.Followings)
            {
                    
                var result = await GetFollows(userQueryParams.UserId,true);
                users =users.Where(u => result.Contains(u.Id));
            }

           
            return await  users.ToListAsync();
        }
         
        public async Task<bool> IsAlreadyFollowed(int followerUserId, int userId)
        {
            return await _context.UserToUser.AnyAsync(i => i.FollowerId ==followerUserId &&  i.UserId ==userId);
            
        }

        public async Task<bool> SaveChanges()
        {
            return await _context.SaveChangesAsync()>0;
        }

        private async Task<IEnumerable<int>> GetFollows(int userId, bool isFollowers){

            var user =await _context.Users.Include(i => i.Followers).Include( i =>i.Followings).FirstOrDefaultAsync(i => i.Id == userId);
            if(isFollowers)
            {
                return user.Followers.Where(i => i.FollowerId == userId).Select(i =>i.UserId);

            }

            else{
                return user.Followers.Where(i => i.UserId == userId).Select(i =>i.FollowerId);
            }

        }
    }
}