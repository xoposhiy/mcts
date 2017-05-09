namespace MctsLib
{
	public interface INode
	{
		INode Parent { get; }
		int TotalPlays { get; }
	}
}