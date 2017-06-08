using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Domain;
using API.Models;
using API.Classes;

namespace API.Controllers
{
    public class GroupsController : ApiController
    {
        private DataContext db = new DataContext();

        // GET: api/Groups
        public IQueryable<Group> GetGroups()
        {
            return db.Groups;
        }

        // GET: api/Groups/5
        [ResponseType(typeof(Group))]
        public async Task<IHttpActionResult> GetGroup(int id)
        {
            var groups = await db.Groups.ToListAsync();
            var list = new List<GroupResponse>();
            foreach (var group in groups)
            {
                var userGroup = group.GroupUsers.Where(gu => gu.UserId == id).FirstOrDefault();
                if (userGroup != null)
                {
                    list.Add(new GroupResponse
                    {
                        GroupId = group.GroupId,
                        GroupUsers = ToGroupUserResponse(group.GroupUsers),
                        Logo = group.Logo,
                        Name = group.Name,
                        Owner = group.Owner,
                        OwnerId = group.OwnerId,
                    });
                }
            }

            return Ok(list);
        }

        private List<GroupUserResponse> ToGroupUserResponse(ICollection<GroupUser> groupUsers)
        {
            var list = new List<GroupUserResponse>();
            foreach (var groupUser in groupUsers.OrderByDescending(gu => gu.Points))
            {
                list.Add(new GroupUserResponse
                {
                    GroupId = groupUser.GroupId,
                    GroupUserId = groupUser.GroupUserId,
                    IsAccepted = groupUser.IsAccepted,
                    IsBlocked = groupUser.IsBlocked,
                    Points = groupUser.Points,
                    User = ToUserResponse(groupUser.User),
                    UserId = groupUser.UserId,
                });
            }

            return list;
        }

        private UserResponse ToUserResponse(User user)
        {
            return new UserResponse
            {
                Email = user.Email,
                FavoriteTeam = user.FavoriteTeam,
                FavoriteTeamId = user.FavoriteTeamId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                NickName = user.NickName,
                Picture = user.Picture,
                Points = user.Points,
                UserId = user.UserId,
                UserType = user.UserType,
                UserTypeId = user.UserTypeId,
            };
        }

        // PUT: api/Groups/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutGroup(int id, Group group)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != group.GroupId)
            {
                return BadRequest();
            }

            db.Entry(group).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroupExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Groups
        [ResponseType(typeof(Group))]
        public async Task<IHttpActionResult> PostGroup(Group group)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Groups.Add(group);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = group.GroupId }, group);
        }

        // DELETE: api/Groups/5
        [ResponseType(typeof(Group))]
        public async Task<IHttpActionResult> DeleteGroup(int id)
        {
            Group group = await db.Groups.FindAsync(id);
            if (group == null)
            {
                return NotFound();
            }

            db.Groups.Remove(group);
            await db.SaveChangesAsync();

            return Ok(group);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool GroupExists(int id)
        {
            return db.Groups.Count(e => e.GroupId == id) > 0;
        }
    }
}