namespace UserState
{
    public interface IUserStateEntry
    {
        int Id { get; set; }
        ulong OwnerId { get; set; }
    }
}