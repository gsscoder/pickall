using System;
using System.Collections.Generic;
using PickAll;

public class MarkerSettings
{
    public string Stamp;
}

public class Marker : PostProcessor
{
    private readonly MarkerSettings _settings;

    public Marker(object settings) : base(settings)
    {
        _settings = Settings as MarkerSettings;
        if (_settings == null) {
            throw new NotImplementedException();
        }
    }

    public override IEnumerable<ResultInfo> Process(IEnumerable<ResultInfo> results)
    {
        foreach (var result in results) {
            yield return new ResultInfo(result.Originator, result.Index, result.Url,
                $"{_settings.Stamp}|{result.Description}", null);
        }
    }
}