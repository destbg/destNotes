using System.Threading.Tasks;
using destNotes.Model;

namespace destNotes.ViewModel.Interface
{
    public interface ISettingsController
    {
        Task<Setting> LoadSetting();

        Task SaveSetting(Setting setting);
    }
}