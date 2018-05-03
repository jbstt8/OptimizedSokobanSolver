namespace SokobanSolver.Interfaces
{
    /// <summary>
    /// The Interface for all Sokoban Crawlers
    /// </summary>
    public interface ISokobanCrawler
    {
        double ElapsedTime { get; }

        string[] Crunch();
    }
}
