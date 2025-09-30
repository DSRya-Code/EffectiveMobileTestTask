using EffectiveMobileTestTask.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EffectiveMobileTestTask.Tests
{
    public class RegionPathTreeControllerTests : IClassFixture<RegionPathTreeControllerTestContext>
    {
        private readonly RegionPathTreeControllerTestContext _context;

        public RegionPathTreeControllerTests(RegionPathTreeControllerTestContext context)
        {
            _context = context;
        }

        [Fact]
        public async Task TestFindCompaniesByExactPath()
        {
            var result = await _context.Controller.FindCompaniesByPath("/ru/msk");

            Assert.Equal(2, result.Count);
            Assert.Contains(result, c => c.Name == "Газета уральских москвичей");
            Assert.Contains(result, c => c.Name == "Яндекс.Директ");
        }

        [Fact]
        public async Task TestFindCompaniesByParentPath()
        {
            var result = await _context.Controller.FindCompaniesByPath("/ru");

            Assert.Equal(1, result.Count);
            Assert.Contains(result, c => c.Name == "Яндекс.Директ");
        }

        [Fact]
        public async Task TestFindCompaniesByRegionPath()
        {
            var result = await _context.Controller.FindCompaniesByPath("/ru/svrd");

            Assert.Equal(2, result.Count);
            Assert.Contains(result, c => c.Name == "Крутая реклама");
            Assert.Contains(result, c => c.Name == "Яндекс.Директ");
        }

        [Fact]
        public async Task TestFindCompaniesByCityPath()
        {
            var result = await _context.Controller.FindCompaniesByPath("/ru/svrd/revda");

            Assert.Equal(3, result.Count);
            Assert.Contains(result, c => c.Name == "Яндекс.Директ");
            Assert.Contains(result, c => c.Name == "Крутая реклама");
            Assert.Contains(result, c => c.Name == "Ревдинский рабочий");
        }

        [Fact]
        public async Task TestFindCompaniesByNonExistentPath()
        {
            var result = await _context.Controller.FindCompaniesByPath("/rus/tatarstan/kazan");

            Assert.Empty(result);
        }

        [Fact]
        public async Task TestReplaceAllCompanies()
        {
            var newCompanies = new List<Company>
            {
                new Company { Name = "Новая компания", Paths = { "/new/region" } }
            };

            await _context.Controller.ReplaceAllCompaniesAsync(newCompanies);

            var result = await _context.Controller.FindCompaniesByPath("/new/region");

            Assert.Single(result);
            Assert.Contains(result, c => c.Name == "Новая компания");
        }
    }
}
