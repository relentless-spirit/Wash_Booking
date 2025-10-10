namespace WashBooking.Domain.Interfaces.Repositories;

public interface ISoftDeletable
{
    /// <summary>
    /// Lấy hoặc đặt giá trị cho biết thực thể có đang hoạt động hay không.
    /// </summary>
    public bool IsActive { get; set; }
}