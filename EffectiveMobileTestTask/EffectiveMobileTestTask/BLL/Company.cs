namespace EffectiveMobileTestTask.BLL
{
    public class Company
    {
        public string Name { get; set; }
        public HashSet<string> Paths { get; set; } = new HashSet<string>();
    }
}
