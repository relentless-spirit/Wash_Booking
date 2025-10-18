using WashBooking.Domain.Enums;

namespace WashBooking.Application.Common.Settings.StateManagement;

 public static class BookingStateTransitions
    {
        // Định nghĩa các đường đi hợp lệ
        private static readonly Dictionary<BookingStatus, List<BookingStatus>> ValidTransitions = new()
        {
            // Từ Scheduled chỉ có thể đến CheckedIn hoặc bị hủy
            {
                BookingStatus.Scheduled,
                new List<BookingStatus> { BookingStatus.CheckedIn, BookingStatus.Cancelled }
            },
            // Từ CheckedIn có thể bắt đầu dịch vụ hoặc bị hủy
            {
                BookingStatus.CheckedIn,
                new List<BookingStatus> { BookingStatus.ServiceInProgress, BookingStatus.Cancelled }
            },
            // Từ ServiceInProgress có thể đi kiểm tra chất lượng, báo cáo vấn đề, hoặc bị hủy
            {
                BookingStatus.ServiceInProgress,
                new List<BookingStatus> { BookingStatus.QualityCheck, BookingStatus.IssueReported, BookingStatus.Cancelled }
            },
            // Từ QualityCheck có thể sẵn sàng giao xe, hoặc quay lại làm tiếp nếu có vấn đề
            {
                BookingStatus.QualityCheck,
                new List<BookingStatus> { BookingStatus.ReadyForPickup, BookingStatus.ServiceInProgress, BookingStatus.IssueReported }
            },
            // Từ ReadyForPickup chỉ có thể hoàn thành
            {
                BookingStatus.ReadyForPickup,
                new List<BookingStatus> { BookingStatus.Completed }
            },
            // Khi có vấn đề, có thể quay lại làm hoặc hủy luôn
            {
                BookingStatus.IssueReported,
                new List<BookingStatus> { BookingStatus.ServiceInProgress, BookingStatus.Cancelled }
            },
            // Trạng thái cuối cùng, không thể đi đâu nữa
            {
                BookingStatus.Completed,
                new List<BookingStatus>() // Danh sách rỗng
            },
            {
                BookingStatus.Cancelled,
                new List<BookingStatus>() // Danh sách rỗng
            }
        };

        /// <summary>
        /// Kiểm tra xem việc chuyển từ trạng thái hiện tại sang trạng thái mới có hợp lệ không.
        /// </summary>
        public static bool CanTransitionTo(BookingStatus currentStatus, BookingStatus newStatus)
        {
            // Nếu trạng thái hiện tại có trong danh sách và trạng thái mới nằm trong list các trạng thái hợp lệ của nó
            return ValidTransitions.ContainsKey(currentStatus) && ValidTransitions[currentStatus].Contains(newStatus);
        }
    }