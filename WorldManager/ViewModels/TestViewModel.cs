using Newtonsoft.Json;
using ReactiveUI;
using VRChat.API.Api;
using VRChat.API.Client;

namespace WorldManager.ViewModels;

public class TestViewModel : ViewModelBase
{
    private string _search = "";

    private string _sort = "popularity";
    
    private int _n = 10;
    
    private int _offset;

    private string _response = "";

    private readonly WorldsApi _worldsApi;

    public TestViewModel()
    {
        _worldsApi = new WorldsApi();
    }
    
    public TestViewModel(Configuration apiConfig)
    {
        _worldsApi = new WorldsApi(apiConfig);
    }
    
    public string Search
    {
        get => _search;
        set => this.RaiseAndSetIfChanged(ref _search, value);
    }
    
    public string Sort
    {
        get => _sort;
        set => this.RaiseAndSetIfChanged(ref _sort, value);
    }
    
    public int N
    {
        get => _n;
        set => this.RaiseAndSetIfChanged(ref _n, value);
    }
    
    public int Offset
    {
        get => _offset;
        set => this.RaiseAndSetIfChanged(ref _offset, value);
    }

    public string Response
    {
        get => _response;
        set => this.RaiseAndSetIfChanged(ref _response, value);
    }

    public void Find()
    {
        var response = _worldsApi.SearchWorldsWithHttpInfo(search: Search, sort: Sort, n: N, offset: Offset);
        Response = JsonConvert.SerializeObject(JsonConvert.DeserializeObject("{\"response\": " + response.RawContent + "}"), Formatting.Indented);
    }

    public void FindItems()
    {
        var items = _worldsApi.SearchWorlds(search: Search, sort: Sort, n: N, offset: Offset);

        if (items == null)
        {
            Response = "";
            return;
        }

        Response = "";
        foreach (var item in items)
        {
            Response += item.ToJson() + "\n";
        }
    }
}