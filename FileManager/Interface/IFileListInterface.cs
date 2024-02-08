namespace Project8.FileManager.Interface;

public interface IFileListInterface<T>
{
    void UserListManager_SaveNewList(string filename, T obj);
    void UpdateJsons();
    void LoadUserList();
    bool AddUserToList(T obj);
    bool CheckIfUserExsists(string usr);
    T GetUser(int id);
    T GetUser(string str);
    ICollection<T> GetUsers();
}