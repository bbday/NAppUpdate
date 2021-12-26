using NAppUpdateR.Tasks;

namespace NAppUpdateR.FeedReaders
{
    public interface IUpdateFeedReader
    {
        IList<IUpdateTask> Read(string feed);
    }
}
