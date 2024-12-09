using Domain.Category;
using Domain.History;
using Domain.Parcels;

namespace Application.Historyy.Exceptionn;

public abstract class HistoryException : Exception
{
    public CategoryId? CategoryId { get; }
    public HistoryId? HistoryId { get; }
    public ParcelId? ParcelId { get; }
    protected HistoryException(CategoryId? categoryId, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        CategoryId = categoryId;
    }
    protected HistoryException(HistoryId? historyId, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        HistoryId = historyId;
    }
    protected HistoryException(ParcelId? parcelId, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        ParcelId = parcelId;
    }
}
public class HistoryNotFoundException(HistoryId id) : HistoryException(id, $"Course under id: {id} not found");

public class HistoryAlreadyExistsException(HistoryId id) : HistoryException(id, $"Course already exists: {id}");

public class HistoryUnknownException(HistoryId id, Exception innerException)
    : HistoryException(id, $"Unknown exception for the Course under id: {id}", innerException);

public class ParceclNooFoundedException(ParcelId id) : HistoryException(id, $"Course no founded");
public class InvalidHistoryDateException(ParcelId id) : HistoryException(id, $"Course already exists: {id}");