namespace AdminUI.Models.User
{
    public class UsersListResponseModel
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public IEnumerable<UserManagementViewModel> Data { get; set; }
        public int TotalCount { get; set; }
    }
}
