


namespace OMS.AlgoManager.Filters;

public interface IAlgoFilter
{
    public void SetupAlgoFilter(bool debugging);
    public int IsInAlgoFilter(int traderId, int groupNumber);
}
