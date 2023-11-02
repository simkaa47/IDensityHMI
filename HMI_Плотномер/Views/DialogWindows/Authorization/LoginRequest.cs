using IDensity.DataAccess;

namespace IDensity.Core.Views.DialogWindows.Authorization
{
    public class LoginRequest:PropertyChangedBase
    {
        public string Password { get; set; }
        public string Login { get; set; }
        
        private bool _isError;
        public bool IsError 
        { 
            get =>_isError;
            set => Set(ref _isError, value);
        }
    }
}
