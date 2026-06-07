//using Microsoft.EntityFrameworkCore;
//using Microsoft.ML;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using VoltHome.Infrastructure;
//using VoltHome.Services.Interfaces;
//using VoltHome.Services.Neural.Model;

//namespace VoltHome.Services.Neural;

//public class RnnNetworkService : IRnnNetworkService
//{
//    private readonly AppDbContext _context;
//    private readonly MlModelProvider _ml;

//    public RnnNetworkService(AppDbContext context, MlModelProvider ml)
//    {
//        _context = context;
//        _ml = ml;
//    }

//    public async Task<float> PredictNextHourAsync(
//        Guid stationId,
//        CancellationToken ct)
//    {
//        var last = await _context.SolarStationEnergySnapshots
//            .Where(x => x.SolarStationId == stationId)
//            .OrderByDescending(x => x.HourStartUtc)
//            .Take(3)
//            .OrderBy(x => x.HourStartUtc)
//            .ToListAsync(ct);

//        if (last.Count < 3)
//            return 0;

//        var input = new RnnInputModel
//        {
//            Sequence = last
//                .Select(x => (float)x.GeneratedKwh)
//                .ToArray()
//        };

//        try
//        {
//            return _ml.RnnEngine.Predict(input).Score;
//        }
//        catch
//        {
//            return last.Average(x => x.GeneratedKwh);
//        }
//    }
//}