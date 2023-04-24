namespace PAiPWebPackageManager.PluginBase;

public class PackageUri
{
    private readonly string _packageUri;

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
        return !_packageUri.Contains(":") ? null : _packageUri.Split(":")[0];
    }

    public string GetPackage()
    {
        var ignoreString = $"{GetManagerUri()}:";
        var index = _packageUri.IndexOf(ignoreString, StringComparison.Ordinal);
        return (index < 0) ? _packageUri : _packageUri.Remove(index, ignoreString.Length);
    }
}
