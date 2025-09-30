namespace EffectiveMobileTestTask.BLL
{
    public class CompanyRegionTree
    {
        private RegionPathNode _root = new RegionPathNode("");

        public RegionPathNode GetRootNode()
        {
            return _root;
        }

        /// <summary>
        /// Добавить компанию в дерево путей
        /// </summary>
        /// <param name="company"></param>
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

        /// <summary>
        /// Ищем компании по пути
        /// </summary>
        /// <param name="path"> путь формата "/ru/msk/..."</param>
        /// <returns></returns>
        public List<Company> FindCompaniesByPath(string path)
        {
            var result = new List<Company>();
            var regions = GetNestedPaths(path); // получить все родительские пути

            foreach (var region in regions)
            {
                var parts = region.Split('/', StringSplitOptions.RemoveEmptyEntries);
                var current = _root;

                foreach (var part in parts)
                {
                    if (!current.Children.ContainsKey(part))
                        break;
                    current = current.Children[part];
                }

                // Добавляем **только компании из текущего узла**, а не поддерева
                result.AddRange(current.Companies);
            }

            return result.Distinct().ToList();
        }

        private static List<string> GetNestedPaths(string path)
        {
            if (string.IsNullOrEmpty(path) || !path.StartsWith("/"))
                return new List<string>();

            var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var result = new List<string>();
            var currentPath = "";

            for (int i = 0; i < parts.Length; i++)
            {
                currentPath += "/" + parts[i];
                result.Add(currentPath);
            }

            return result;
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

        /// <summary>
        /// Чистим дерево
        /// </summary>
        public void Clear()
        {
            _root = new RegionPathNode("");
        }
    }
}
