using Backend.Classes;
using Backend.Helpers;
using Backend.Models;
using Domain;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using PagedList;

namespace Backend.Controllers
{
    public class UsersController : Controller
    {
        private DataContextLocal db = new DataContextLocal();

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteUser(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var groupUser = await db.GroupUsers.FindAsync(id);

            if (groupUser == null)
            {
                return HttpNotFound();
            }

            db.GroupUsers.Remove(groupUser);
            await db.SaveChangesAsync();
            return RedirectToAction(string.Format("DetailsGroup/{0}", groupUser.GroupId));
        }


        public async Task<ActionResult> EditUser(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var groupUser = await db.GroupUsers.FindAsync(id);

            if (groupUser == null)
            {
                return HttpNotFound();
            }

            ViewBag.UserId = new SelectList(db.Users.OrderBy(u => u.FirstName).ThenBy(u => u.LastName), "UserId", "FullName", groupUser.UserId);
            return View(groupUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> EditUser(GroupUser groupUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(groupUser).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction(string.Format("DetailsGroup/{0}", groupUser.GroupId));
            }

            ViewBag.UserId = new SelectList(db.Users.OrderBy(u => u.FirstName).ThenBy(u => u.LastName), "UserId", "FullName", groupUser.UserId);
            return View(groupUser);
        }

        public async Task<ActionResult> AddUser(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var group = await db.Groups.FindAsync(id);

            if (group == null)
            {
                return HttpNotFound();
            }

            ViewBag.UserId = new SelectList(db.Users.OrderBy(u => u.FirstName).ThenBy(u => u.LastName), "UserId", "FullName");
            var view = new GroupUser { GroupId = group.GroupId, Points = 0, };
            return View(view);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddUser(GroupUser groupUser)
        {
            if (ModelState.IsValid)
            {
                db.GroupUsers.Add(groupUser);
                await db.SaveChangesAsync();
                return RedirectToAction(string.Format("DetailsGroup/{0}", groupUser.GroupId));
            }

            ViewBag.UserId = new SelectList(db.Users.OrderBy(u => u.FirstName).ThenBy(u => u.LastName), "UserId", "FullName", groupUser.UserId);
            return View(groupUser);
        }

        public async Task<ActionResult> DetailsGroup(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var group = await db.Groups.FindAsync(id);

            if (group == null)
            {
                return HttpNotFound();
            }

            return View(group);
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteGroup(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var group = await db.Groups.FindAsync(id);

            if (group == null)
            {
                return HttpNotFound();
            }

            db.Groups.Remove(group);
            await db.SaveChangesAsync();
            return RedirectToAction(string.Format("Details/{0}", group.OwnerId));
        }

        public async Task<ActionResult> EditGroup(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var group = await db.Groups.FindAsync(id);

            if (group == null)
            {
                return HttpNotFound();
            }

            var view = ToView(group);
            return View(view);
        }

        private GroupView ToView(Group group)
        {
            return new GroupView
            {
                GroupId = group.GroupId,
                GroupUsers = group.GroupUsers,
                Logo = group.Logo,
                Name = group.Name,
                Owner = group.Owner,
                OwnerId = group.OwnerId,
            };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> EditGroup(GroupView view)
        {
            if (ModelState.IsValid)
            {
                var pic = view.Logo;
                var folder = "~/Content/Groups";

                if (view.LogoFile != null)
                {
                    pic = FilesHelper.UploadPhoto(view.LogoFile, folder);
                    pic = string.Format("{0}/{1}", folder, pic);
                }

                var group = ToGroup(view);
                group.Logo = pic;
                db.Entry(group).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction(string.Format("Details/{0}", view.OwnerId));
            }

            return View(view);
        }

        public async Task<ActionResult> CreateGroup(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = await db.Users.FindAsync(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            var view = new GroupView { OwnerId = user.UserId, };
            return View(view);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CreateGroup(GroupView view)
        {
            if (ModelState.IsValid)
            {
                var pic = string.Empty;
                var folder = "~/Content/Groups";

                if (view.LogoFile != null)
                {
                    pic = FilesHelper.UploadPhoto(view.LogoFile, folder);
                    pic = string.Format("{0}/{1}", folder, pic);
                }

                var group = ToGroup(view);
                group.Logo = pic;
                db.Groups.Add(group);
                await db.SaveChangesAsync();
                return RedirectToAction(string.Format("Details/{0}", view.OwnerId));
            }

            return View(view);
        }

        private Group ToGroup(GroupView view)
        {
            return new Group
            {
                GroupId = view.GroupId,
                GroupUsers = view.GroupUsers,
                Logo = view.Logo,
                Name = view.Name,
                Owner = view.Owner,
                OwnerId = view.OwnerId,
            };
        }

        public ActionResult Index(int? page = null)
        {
            page = (page ?? 1);
            var users = db.Users
                .Include(u => u.FavoriteTeam)
                .Include(u => u.UserType)
                .OrderByDescending(u => u.Points)
                .ThenBy(u => u.FirstName)
                .ThenBy(u => u.LastName);
            return View(users.ToPagedList((int)page, 10));
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = await db.Users.FindAsync(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            var view = ToView(user);
            view.Groups = new List<Group>();

            var groups = await db.Groups.ToListAsync();
            foreach (var group in groups)
            {
                var userGroup = group.GroupUsers.Where(gu => gu.UserId == id).FirstOrDefault();
                if (userGroup != null)
                {
                    view.Groups.Add(new Group
                    {
                        GroupId = group.GroupId,
                        GroupUsers = group.GroupUsers,
                        Logo = group.Logo,
                        Name = group.Name,
                        Owner = group.Owner,
                        OwnerId = group.OwnerId,
                    });
                }
            }

            return View(view);
        }

        public ActionResult Create()
        {
            ViewBag.FavoriteLeagueId = new SelectList(db.Leagues.OrderBy(l => l.Name), "LeagueId", "Name");
            ViewBag.FavoriteTeamId = new SelectList(db.Teams.Where(t => t.LeagueId == db.Leagues.FirstOrDefault().LeagueId).OrderBy(t => t.Name), "TeamId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create(UserView view)
        {
            if (ModelState.IsValid)
            {
                var pic = string.Empty;
                var folder = "~/Content/Users";

                if (view.PictureFile != null)
                {
                    pic = FilesHelper.UploadPhoto(view.PictureFile, folder);
                    pic = string.Format("{0}/{1}", folder, pic);
                }

                var user = ToUser(view);
                user.Picture = pic;
                user.Points = 0;
                user.UserTypeId = 1;
                db.Users.Add(user);
                await db.SaveChangesAsync();
                UsersHelper.CreateUserASP(view.Email, "User", view.Password);
                return RedirectToAction("Index");
            }

            ViewBag.FavoriteLeagueId = new SelectList(db.Leagues.OrderBy(l => l.Name), "LeagueId", "Name", view.FavoriteLeagueId);
            ViewBag.FavoriteTeamId = new SelectList(db.Teams.Where(t => t.LeagueId == view.FavoriteLeagueId).OrderBy(t => t.Name), "TeamId", "Name", view.FavoriteTeamId);
            return View(view);
        }

        private User ToUser(UserView view)
        {
            return new User
            {
                Email = view.Email,
                FavoriteTeamId = view.FavoriteTeamId,
                FirstName = view.FirstName,
                LastName = view.LastName,
                NickName = view.NickName,
                Picture = view.Picture,
                Points = view.Points,
                UserId = view.UserId,
                UserTypeId = view.UserTypeId,
            };
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = await db.Users.FindAsync(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            ViewBag.FavoriteLeagueId = new SelectList(db.Leagues.OrderBy(l => l.Name), "LeagueId", "Name", user.FavoriteTeamId);
            ViewBag.FavoriteTeamId = new SelectList(db.Teams.Where(t => t.LeagueId == user.FavoriteTeamId).OrderBy(t => t.Name), "TeamId", "Name", user.FavoriteTeamId);
            var view = ToView(user);
            return View(view);
        }

        private UserView ToView(User user)
        {
            return new UserView
            {
                Email = user.Email,
                FavoriteTeamId = user.FavoriteTeamId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                NickName = user.NickName,
                Picture = user.Picture,
                Points = user.Points,
                UserId = user.UserId,
                UserTypeId = user.UserTypeId,
                FavoriteTeam = user.FavoriteTeam,
                GroupUsers = user.GroupUsers,
                UserGroups = user.UserGroups,
                UserType = user.UserType,
            };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(UserView view)
        {
            if (ModelState.IsValid)
            {
                var pic = view.Picture;
                var folder = "~/Content/Users";

                if (view.PictureFile != null)
                {
                    pic = FilesHelper.UploadPhoto(view.PictureFile, folder);
                    pic = string.Format("{0}/{1}", folder, pic);
                }

                var user = ToUser(view);
                user.Picture = pic;
                db.Entry(user).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.FavoriteLeagueId = new SelectList(db.Leagues.OrderBy(l => l.Name), "LeagueId", "Name", view.FavoriteTeamId);
            ViewBag.FavoriteTeamId = new SelectList(db.Teams.Where(t => t.LeagueId == view.FavoriteTeamId).OrderBy(t => t.Name), "TeamId", "Name", view.FavoriteTeamId);
            return View(view);
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = await db.Users.FindAsync(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            db.Users.Remove(user);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}