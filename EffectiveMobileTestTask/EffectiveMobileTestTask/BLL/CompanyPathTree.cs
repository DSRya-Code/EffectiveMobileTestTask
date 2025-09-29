namespace EffectiveMobileTestTask.BLL
{
    public class CompanyRegionTree
    {
        private RegionPathNode _root = new RegionPathNode("");

        public void AddCompany(Company company)
        {
            foreach (var path in company.Paths)
            {
                var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
                var current = _root;

                foreach (var part in parts)
                {
                    if (!current.Children.ContainsKey(part))
                    {
                        current.Children[part] = new RegionPathNode(part);
                    }
                    current = current.Children[part];
                }

                // Добавляем компанию в конечный узел
                current.Companies.Add(company);
            }
        }

        public List<Company> FindCompaniesByPath(string path)
        {
            var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var current = _root;

            foreach (var part in parts)
            {
                if (!current.Children.ContainsKey(part))
                    return new List<Company>();
                current = current.Children[part];
            }

            // Возвращаем все компании, находящиеся в поддереве
            return GetAllCompaniesInSubtree(current).Distinct().ToList();
        }

        private List<Company> GetAllCompaniesInSubtree(RegionPathNode node)
        {
            var result = new List<Company>(node.Companies);

            foreach (var child in node.Children.Values)
            {
                result.AddRange(GetAllCompaniesInSubtree(child));
            }

            return result;
        }
    }
}
