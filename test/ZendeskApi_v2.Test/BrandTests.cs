using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ZendeskApi_v2;
using ZendeskApi_v2.Models.Brands;
using System;

namespace Tests
{
    [TestFixture]
    public class BrandTests
    {
        private readonly ZendeskApi api = new ZendeskApi(Settings.Site, Settings.AdminEmail, Settings.AdminPassword);

        [OneTimeSetUp]
        public void Init()
        {
            var brands = api.Brands.GetBrands();
            if (brands != null)
            {
                foreach (var brand in brands.Brands.Where(o => o.Name.Contains("Test Brand")))
                {
                    api.Brands.DeleteBrand(brand.Id.Value);
                }
            }
        }

        [Test]
        public void CanGetBrands()
        {
            var res = api.Brands.GetBrands();
            Assert.Greater(res.Count, 0);

            var ind = api.Brands.GetBrand(res.Brands[0].Id.Value);
            Assert.AreEqual(ind.Brand.Id, res.Brands[0].Id);
        }

        [Test]
        public void CanCreateUpdateAndDeleteBrand()
        {
            var brand = new Brand()
            {
                Name = "Test Brand",
                Active = true,
                Subdomain = string.Format("test-{0}", Guid.NewGuid())
            };

            var res = api.Brands.CreateBrand(brand);

            Assert.Greater(res.Brand.Id, 0);

            res.Brand.Name = "Test Brand Updated";
            var update = api.Brands.UpdateBrand(res.Brand);
            Assert.AreEqual(update.Brand.Name, res.Brand.Name);

            Assert.True(api.Brands.DeleteBrand(res.Brand.Id.Value));
        }

    }
}