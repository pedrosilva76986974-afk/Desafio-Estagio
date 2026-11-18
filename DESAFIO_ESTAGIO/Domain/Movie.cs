namespace DESAFIO_ESTAGIO.Domain;

public sealed class Movie
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Synopsis { get; set; } = string.Empty;
    public string Director { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
    public ICollection<MovieActor> Actors { get; set; } = new List<MovieActor>();
    public ICollection<Vote> Votes { get; set; } = new List<Vote>();
}

public sealed class MovieActor
{
    public Guid MovieId { get; set; }
    public string Name { get; set; } = string.Empty;
}
