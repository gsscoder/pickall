using System;
using System.Collections.Generic;
using PickAll;

public class MarkerSettings
{
    public string Stamp;
}

public class Marker : PostProcessor
{
    readonly MarkerSettings _settings;

    public Marker(object settings) : base(settings)
    {
        _settings = Settings as MarkerSettings;
        if (_settings == null) {
            throw new NotImplementedException();
        }
    }

    public override bool PublishEvents { get { return true; } }

    public override IEnumerable<ResultInfo> Process(IEnumerable<ResultInfo> results)
    {
        foreach (var result in results) {
            var marked =  new ResultInfo(result.Originator, result.Index, result.Url,
                $"{_settings.Stamp}|{result.Description}", null);
            yield return marked;
        }
    }
}
