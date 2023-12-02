using AuthenticatorAPI.Models;
using Project8.FileManager;

namespace AuthenticatorAPI.Data;

public class UserManager : IListManager<List<User>>
{
    private readonly FileManager<List<User>> _fileManager = new FileManager<List<User>>();
    private readonly string _fileName = "Users.json";
    private static List<User> _userList = new List<User>();
    private static event FileManager<List<User>>.SaveToFileDelegate? SaveNewList;

    public UserManager()
    {
        LoadList();
        SaveNewList += SaveUserList;
        Console.WriteLine($"Loaded {_userList.Count} Users");
    }

    private void SaveUserList(string fileName, List<User> users)
    {
        _fileManager.SaveToFile(fileName, users);
    }
    
    public void UpdateJsons()
    {
        SaveNewList?.Invoke(_fileName, _userList);
    }

    public async void LoadList()
    {
        _userList = await _fileManager.LoadFromFile(_fileName) ?? new List<User>();
    }

    public bool AddToList(User user)
    {
        if (_userList.FirstOrDefault(u => u.Email == user.Email) == null)
        {
            _userList.Add(user);
            UpdateJsons();
            return true;
        }

        return false;
    }

    public User?GetById(string email)
    {
        return _userList.FirstOrDefault(u => u.Email == email);
    }
}