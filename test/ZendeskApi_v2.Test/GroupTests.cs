using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using ZendeskApi_v2;
using ZendeskApi_v2.Models.Constants;
using ZendeskApi_v2.Models.Groups;
using ZendeskApi_v2.Models.Users;

namespace Tests
{
    [TestFixture]
    public class GroupTests
    {
        private readonly ZendeskApi api = new ZendeskApi(Settings.Site, Settings.AdminEmail, Settings.AdminPassword);

        [OneTimeSetUp]
        public async Task CleanUp()
        {
            var resp = await api.Search.SearchForAsync<User>("test133@test.com");

            foreach (var user in resp.Results)
            {
                await api.Users.DeleteUserAsync(user.Id.Value);
            }
        }

        [Test]
        public void CanGetGroups()
        {
            var res = api.Groups.GetGroups();
            Assert.Greater(res.Count, 0);
        }

        [Test]
        public void CanGetAssignableGroups()
        {
            var res = api.Groups.GetAssignableGroups();
            Assert.Greater(res.Count, 0);
        }

        [Test]
        public void CanGetGroup()
        {
            var res = api.Groups.GetGroups();
            var res1 = api.Groups.GetGroupById(res.Groups[0].Id.Value);

            Assert.AreEqual(res1.Group.Id.Value, res.Groups[0].Id.Value);
        }

        [Test]
        public void CanCreateUpdateAndDeleteGroup()
        {
            var res = api.Groups.CreateGroup("Test Group");
            Assert.True(res.Group.Id > 0);

            res.Group.Name = "Updated Test Group";
            var res1 = api.Groups.UpdateGroup(res.Group);
            Assert.AreEqual(res1.Group.Name, res.Group.Name);

            Assert.True(api.Groups.DeleteGroup(res.Group.Id.Value));
        }

        [Test]
        public void CanGetGroupMemberships()
        {
            var res = api.Groups.GetGroupMemberships();
            Assert.That(res.Count, Is.GreaterThan(0));

            var res1 = api.Groups.GetGroupMembershipsByUser(Settings.UserId);
            Assert.That(res1.Count, Is.GreaterThan(0));

            var groups = api.Groups.GetGroups();
            var res2 = api.Groups.GetGroupMembershipsByGroup(groups.Groups[0].Id.Value);
            Assert.That(res2.Count, Is.GreaterThan(0));

            int count = 2;
            int page = 2;

            // Verify that first entry on page 2 is the same as 3rd entry on non-paginated request.
            var res1Paged = api.Groups.GetGroupMembershipsByUser(Settings.UserId, count, page);
            Assert.That(res1Paged.Count, Is.GreaterThan(0));
            Assert.That(res1Paged.GroupMemberships[0].Id, Is.EqualTo(res1.GroupMemberships[2].Id));

            // Verify that first entry on page 2 is the same as 3rd entry on non-paginated request.
            var res2Paged = api.Groups.GetGroupMembershipsByGroup(groups.Groups[0].Id.Value, count, page);
            Assert.That(res2Paged.Count, Is.GreaterThan(0));
            Assert.That(res2Paged.GroupMemberships[0].Id, Is.EqualTo(res2.GroupMemberships[2].Id));
        }

        [Test]
        public void CanGetAssignableGroupMemberships()
        {
            var res = api.Groups.GetAssignableGroupMemberships();
            Assert.Greater(res.Count, 0);

            var groups = api.Groups.GetGroups();
            var res1 = api.Groups.GetAssignableGroupMembershipsByGroup(groups.Groups[0].Id.Value);
            Assert.Greater(res1.Count, 0);
        }

        [Test]
        public void CanGetIndividualGroupMemberships()
        {
            var res = api.Groups.GetGroupMemberships();

            var res1 = api.Groups.GetGroupMembershipsByMembershipId(res.GroupMemberships[0].Id.Value);
            Assert.AreEqual(res1.GroupMembership.Id, res.GroupMemberships[0].Id);

            var res2 = api.Groups.GetGroupMembershipsByUserAndMembershipId(res1.GroupMembership.UserId, res.GroupMemberships[0].Id.Value);
            Assert.AreEqual(res2.GroupMembership.UserId, res1.GroupMembership.UserId);
        }

        [Test]
        public void CanCreateUpdateAndDeleteMembership()
        {
            var group = api.Groups.CreateGroup("Test Group 2").Group;
            var user = api.Users.CreateUser(new User()
            {
                Name = "test user133",
                Email = "test133@test.com",
                Role = UserRoles.Agent
            }).User;

            var res = api.Groups.CreateGroupMembership(new GroupMembership()
            {
                UserId = user.Id.Value,
                GroupId = group.Id.Value
            });

            Assert.Greater(res.GroupMembership.Id, 0);

            var res2 = api.Groups.SetGroupMembershipAsDefault(user.Id.Value, res.GroupMembership.Id.Value);
            Assert.True(res2.GroupMemberships.First(x => x.Id == res.GroupMembership.Id).Default);

            Assert.True(api.Groups.DeleteGroupMembership(res.GroupMembership.Id.Value));
            Assert.True(api.Users.DeleteUser(user.Id.Value));
            Assert.True(api.Groups.DeleteGroup(group.Id.Value));
        }

        [Test]
        public void CanGetGroupMembershipsAsync()
        {
            var res = api.Groups.GetGroupMembershipsAsync().Result;
            Assert.That(res.Count, Is.GreaterThan(0));

            var res1 = api.Groups.GetGroupMembershipsByUserAsync(Settings.UserId).Result;
            Assert.That(res1.Count, Is.GreaterThan(0));

            var groups = api.Groups.GetGroupsAsync().Result;
            var res2 = api.Groups.GetGroupMembershipsByGroupAsync(groups.Groups[0].Id.Value).Result;
            Assert.That(res2.Count, Is.GreaterThan(0));

            int count = 2;
            int page = 2;

            // Verify that first entry on page 2 is the same as 3rd entry on non-paginated request.
            var res1Paged = api.Groups.GetGroupMembershipsByUserAsync(Settings.UserId, count, page).Result;
            Assert.That(res1Paged.Count, Is.GreaterThan(0));
            Assert.That(res1Paged.GroupMemberships[0].Id, Is.EqualTo(res1.GroupMemberships[2].Id));

            // Verify that first entry on page 2 is the same as 3rd entry on non-paginated request.
            var res2Paged = api.Groups.GetGroupMembershipsByGroupAsync(groups.Groups[0].Id.Value, count, page).Result;
            Assert.That(res2Paged.Count, Is.GreaterThan(0));
            Assert.That(res2Paged.GroupMemberships[0].Id, Is.EqualTo(res2.GroupMemberships[2].Id));
        }
    }
}
