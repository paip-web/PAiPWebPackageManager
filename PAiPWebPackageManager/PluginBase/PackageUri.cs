namespace PAiPWebPackageManager.PluginBase;

public class PackageUri
{
    private string _packageUri;

    public PackageUri(string packageUri)
    {
        _packageUri = packageUri;
    }

    public string GetUri()
    {
        return _packageUri;
    }

    public string? GetManagerUri()
    {
        if (_packageUri.Contains(":"))
        {
            return _packageUri.Split(":")[0];
        }
        else
        {
            return null;
        }
    }

    public string GetPackage()
    {
        var ignoreString = $"{GetManagerUri()}:";
        int index = _packageUri.IndexOf(ignoreString, StringComparison.Ordinal);
        return (index < 0) ? _packageUri : _packageUri.Remove(index, ignoreString.Length);
    }
}
