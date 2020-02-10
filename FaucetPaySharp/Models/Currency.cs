namespace FaucetPaySharp.Models
{
    public class Currency
    {
        public string Name { get; set; }
        public string Acronym { get; set; }

        public override string ToString() => Name;
    }
}