using System.Collections.Generic;
using System.Threading.Tasks;
using destNotes.Model;

namespace destNotes.ViewModel.Interface
{
    public interface ISettingsController
    {
        Task<Setting> LoadSettings();

        Task SaveSettings(Setting setting);

        Task<IEnumerable<Theme>> LoadThemes();

        Task SaveThemes(IEnumerable<Theme> themes);

        Task SaveTheme(Theme theme);

        Task AddTheme(Theme theme);
    }
}