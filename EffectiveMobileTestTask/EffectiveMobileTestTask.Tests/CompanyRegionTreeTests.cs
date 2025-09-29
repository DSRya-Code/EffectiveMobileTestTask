using EffectiveMobileTestTask.BLL;

namespace EffectiveMobileTestTask.Tests
{
    public class CompanyRegionTreeTests : IClassFixture<CompanyRegionTreeTestContext>
    {

        private readonly CompanyRegionTreeTestContext _context;

        public CompanyRegionTreeTests(CompanyRegionTreeTestContext context)
        {
            _context = context;
        }

        [Fact]
        public void TestFindCompaniesByExactPath()
        {
            var result = _context.Tree.FindCompaniesByPath("/ru/msk");

            Assert.Equal(1, result.Count);
            Assert.Contains(result, c => c.Name == "Газета уральских москвичей");
        }

        [Fact]
        public void TestFindCompaniesByParentPath()
        {
            var result = _context.Tree.FindCompaniesByPath("/ru");

            Assert.Equal(4, result.Count); // Все компании находятся под /ru
            Assert.Contains(result, c => c.Name == "Яндекс.Директ");
            Assert.Contains(result, c => c.Name == "Ревдинский рабочий");
            Assert.Contains(result, c => c.Name == "Газета уральских москвичей");
            Assert.Contains(result, c => c.Name == "Крутая реклама");
        }

        [Fact]
        public void TestFindCompaniesByRegionPath()
        {
            var result = _context.Tree.FindCompaniesByPath("/ru/svrd");

            Assert.Equal(2, result.Count); // Ревдинский рабочий и Крутая реклама
            Assert.Contains(result, c => c.Name == "Ревдинский рабочий");
            Assert.Contains(result, c => c.Name == "Крутая реклама");
        }

        [Fact]
        public void TestFindCompaniesByCityPath()
        {
            var result = _context.Tree.FindCompaniesByPath("/ru/svrd/revda");

            Assert.Single(result);
            Assert.Contains(result, c => c.Name == "Ревдинский рабочий");
        }

        [Fact]
        public void TestFindCompaniesByNonExistentPath()
        {
            var result = _context.Tree.FindCompaniesByPath("/ru/tatarstan/kazan");

            Assert.Empty(result);
        }
    }
}
