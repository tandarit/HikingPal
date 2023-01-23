namespace HikingPal.Models
{
    public sealed class HashingOptions
    {
        public int IterationsMin { get; set; } = 10000;
        public int IterationsMax { get; set; } = 1000000;
        public int SaltSize { get; set; } = 16;
        public int KeySize { get; set; } = 32;
    }
}
