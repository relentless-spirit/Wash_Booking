using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WashBooking.Domain.Common;

namespace WashBooking.Domain.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        /// <summary>
        /// Lấy một đối tượng theo khóa chính (ID).
        /// </summary>
        /// <param name="id">Khóa chính của đối tượng.</param>
        /// <returns>Trả về đối tượng nếu tìm thấy, ngược lại trả về null.</returns>
        Task<T?> GetByIdAsync(Guid id);

        /// <summary>
        /// Lấy tất cả các đối tượng của một bảng.
        /// </summary>
        /// <returns>Danh sách tất cả các đối tượng.</returns>
        Task<List<T>> GetAllAsync();

        /// <summary>
        /// Lấy một danh sách các đối tượng đã được phân trang.
        /// </summary>
        /// <param name="pageIndex">Chỉ số của trang (bắt đầu từ 1).</param>
        /// <param name="pageSize">Số lượng mục trên mỗi trang.</param>
        /// <returns>Kết quả phân trang chứa danh sách các mục và thông tin phân trang.</returns>
        Task<PagedResult<T>> GetPagedAsync(int pageIndex, int pageSize, Expression<Func<T, bool>>? filter = null);

        /// <summary>
        /// Thêm một đối tượng mới.
        /// </summary>
        /// <param name="entity">Đối tượng cần thêm.</param>
        Task AddAsync(T entity);

        /// <summary>
        /// Đánh dấu một đối tượng là đã bị thay đổi.
        /// </summary>
        /// <param name="entity">Đối tượng cần cập nhật.</param>
        void Update(T entity);

        /// <summary>
        /// Đánh dấu một đối tượng sẽ bị xóa.
        /// </summary>
        /// <param name="entity">Đối tượng cần xóa.</param>
        void Remove(T entity);
    }
}
