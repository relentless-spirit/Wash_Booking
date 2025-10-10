using System;
using System.Collections.Generic;

namespace WashBooking.Domain.Common
{
    /// <summary>
    /// Lớp chứa kết quả của một truy vấn đã được phân trang và hiện thực hóa.
    /// </summary>
    /// <typeparam name="T">Kiểu dữ liệu của các mục.</typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// Danh sách các mục trên trang hiện tại (đã được tải vào bộ nhớ).
        /// </summary>
        public List<T> Items { get; set; } = new List<T>();

        /// <summary>
        /// Tổng số mục trong toàn bộ tập dữ liệu (sau khi đã lọc).
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Số của trang hiện tại (bắt đầu từ 1).
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Kích thước của trang (số mục mỗi trang).
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Tổng số trang.
        /// </summary>
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;

        /// <summary>
        /// Cho biết có trang trước đó hay không.
        /// </summary>
        public bool HasPreviousPage => PageNumber > 1;

        /// <summary>
        /// Cho biết có trang tiếp theo hay không.
        /// </summary>
        public bool HasNextPage => PageNumber < TotalPages;
    }
}