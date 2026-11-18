namespace DESAFIO_ESTAGIO.Domain;

public sealed class Vote
{
    private Vote() { }

    public Vote(Guid movieId, Guid userId, int score)
    {
        MovieId = movieId;
        UserId = userId;
        ChangeScore(score);
    }

    public Guid MovieId { get; private set; }
    public Guid UserId { get; private set; }
    public int Score { get; private set; }

    public void ChangeScore(int score)
    {
        if (score is < 0 or > 4)
            throw new ArgumentOutOfRangeException(nameof(score), "O voto deve estar entre 0 e 4.");
        Score = score;
    }
}
