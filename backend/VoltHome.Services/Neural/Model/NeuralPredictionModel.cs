using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Text;
using VoltHome.Domain.dbo;
using VoltHome.Infrastructure;

namespace VoltHome.Services.Neural.Model;

public class NeuralPredictionModel
{
    public float Score { get; set; }
}