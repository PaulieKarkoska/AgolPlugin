namespace AgolPlugin.Models.Common
{
    public interface IContextIsViewModel
    {
        ViewModelBase VM { get; }
        bool IsViewModelBusy { get; }
    }
}