namespace EffectiveMobileTestTask.BLL
{
    public class RegionPathNode
    {
        public string Name { get; set; }
        public List<Company> Companies { get; set; } = new List<Company>();
        public Dictionary<string, RegionPathNode> Children { get; set; } = new Dictionary<string, RegionPathNode>();

        public RegionPathNode(string name)
        {
            Name = name;
        }
    }
}
