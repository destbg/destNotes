using System.Collections.Generic;
using System.Threading.Tasks;
using destNotes.Model;

namespace destNotes.ViewModel.Interface
{
    public interface ITaskController
    {
        Task<IEnumerable<TaskModel>> LoadTasks();

        Task<TaskModel> LoadTask(string id);

        Task AddTask(TaskModel task);

        Task SaveTask(TaskModel task);

        Task DeleteTask(string id);

        Task DeleteTaskText(string id);
    }
}